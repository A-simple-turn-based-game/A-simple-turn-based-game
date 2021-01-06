using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSystem : MonoBehaviour
{ 
    public Action<string> MapUpdateCallBack;
    // 地图回合控制器
    private MapRoundController m_MapRoundController;
    // 用于生成地图
    private MapGenerator m_MapGenerator;
    // 用于控制地图
    private MapCeilController m_MapController;

    public int currMapIdx; 
    private int m_Row;
    private int m_Col;
    private bool m_IsMapRoundStart = false;
    private List<int> m_NextMap;

    public Ceil[,] ceils;
    public Player player = null;
    public Dictionary<ICharacter, Ceil> characterCeilDict = new Dictionary<ICharacter, Ceil>();
    public Dictionary<IBuilding, Ceil> buildingCeilDict = new Dictionary<IBuilding, Ceil>();
     
    private GameObject m_Content = null;
    private GameObject m_MonsterContent = null;
    private GameObject m_EventContent = null;
     

    public void OnInit()
    { 
        m_MapGenerator = new MapGenerator(this);
        m_MapController = new MapCeilController(this);
        m_MapRoundController = new MapRoundController(this);
    }
     
    private void Update()
    {
        if(m_IsMapRoundStart) m_MapRoundController.OnUpdate();
    }

    private void StartMapRound() {
        m_MapRoundController.OnInit();
        m_IsMapRoundStart = true;
    }

    public void LoadMap(int id)
    {
        currMapIdx = id;
        // TODO 加载地图过场动画
        ClearMap();

        m_Content = new GameObject("Content");
        m_Content.transform.SetParent(transform, false);

        m_MonsterContent = new GameObject("MosterContent");
        m_MonsterContent.transform.SetParent(transform, false);

        m_EventContent = new GameObject("EventContent");
        m_EventContent.transform.SetParent(transform, false);

        MapCfg cfg = ResFactory.instance.GetMapCfgById(id);
        m_Row = cfg.row;
        m_Col = cfg.col;
        m_NextMap = cfg.nextMap;


        ceils = m_MapGenerator.Generate(m_Row, m_Col, Vector3.zero, m_Content.transform, AlgoType.CELLULAR_AUTOMATA);

        m_MapGenerator.LoadMonster(ceils,cfg.monster,m_MonsterContent.transform);

        m_MapGenerator.LoadShop(ceils, m_EventContent.transform);

        m_MapGenerator.LoadEvent(ceils,cfg.@event,m_EventContent.transform);

        m_MapController.RegisterCeils(ceils);

        MapUpdateCallBack?.Invoke(cfg.name);
        
        Global.cfg = cfg;
        GameRoot.instance.PlayBgSound(ResFactory.instance.LoadAudioClip(Global.cfg.bgm));
    }
     
    public void CeilCharacterUpdate(Ceil ceil,ICharacter oldValue,ICharacter newValue) {
        if (newValue != null)
        {
            if (characterCeilDict.ContainsKey(newValue))
            {
                Ceil tmp = characterCeilDict[newValue];
                tmp.ForceCharacterToNull();
                characterCeilDict[newValue] = ceil; 
            }
            else
            {
                characterCeilDict.Add(newValue, ceil);
            }
        }
        else if (characterCeilDict.ContainsKey(oldValue)) { 
            characterCeilDict.Remove(oldValue);
        }
    }

    public void CeilBuildingUpdate(Ceil ceil,IBuilding oldValue, IBuilding newValue) {
        if (newValue != null) {
            buildingCeilDict.Add(newValue,ceil);
        }
        else if(buildingCeilDict.ContainsKey(oldValue)){ 
            buildingCeilDict.Remove(oldValue);
        }
    }
     
    public void GenerateAnEventNearTheCharacter(int eventId,ICharacter character) {
        Ceil target = characterCeilDict[character];
        target = m_MapController.FindTheAvailableCeilsNearby(ceils,target);
        IBuilding baseBuilding = m_MapGenerator.LoadEvent(eventId,target,m_EventContent.transform);

        // TODO 任务系统
        if (eventId == 1003) {
            EventCenter.Broadcast(EventType.FINISH_TASK, 0);
            EventCenter.Broadcast(EventType.ADD_TASK, 1,"进入传送点");
        }

        buildingCeilDict.Add(baseBuilding,target);
    }
  
    private void LoadPlayer(Player player) {
        this.player = player;
        player.GetControllerSystem().OnInit();
        m_MapGenerator.LoadPlayer(ceils,player);
        GameRoot.instance.SetCameraFollow(player.gameObject.transform.Find("LookPos"));
        this.player.EnableMapMove();
    } 

    public void ClearMap() {

        if (m_Content != null) {
            DestroyImmediate(m_Content); 
        }
        if (m_EventContent != null) {
            Destroy(m_EventContent);
        }
        if (m_MonsterContent != null) {
            Destroy(m_MonsterContent);
        }
        m_MapController.Clear();
        if (characterCeilDict != null) characterCeilDict.Clear();
   
        if(buildingCeilDict != null) buildingCeilDict.Clear();
    }
     
    public void RemoveCharacter(ICharacter character){
        //TODO 描述不准确
        characterCeilDict[character].Character = null;
        m_MapRoundController.RemoveCharacter(character);
        character.BatttleLose();
    }

    public void GoToNextMapRandomly() {
        player.DisableMapMove();
 
        if (m_NextMap == null || m_NextMap.Count == 0) { 
            LogTool.LogError("没有下一个地图可以前往"); 
        }
        int idx = QTool.GetRandomInt(0,m_NextMap.Count-1);
        ClearMap();
        LoadMap(m_NextMap[idx]);
        LoadPlayer(player);


        EventCenter.Broadcast(EventType.CLEAR_TASK);
        if (Global.cfg.nextMap.Count == 0) {
            EventCenter.Broadcast(EventType.ADD_TASK, 0, "恭喜通关");
        }
        else
        { 
            EventCenter.Broadcast(EventType.ADD_TASK,0,"找到并击杀BOSS");
        }

        StartMapRound();

        if (Global.isOnlineLogin == false) return;
        SendSaveInfoMsg sendSaveInfoMsg = new SendSaveInfoMsg()
        {
            _lastMapIdx = currMapIdx,
            _lastPlayerInfo = player
        };
        NetService.SendMsg(SendMsgType.SEND_SAVE_INFO,sendSaveInfoMsg);
    }

    public void GoToMap(int id)
    {
        player.DisableMapMove();
        ClearMap();
        LoadMap(id);
        LoadPlayer(player);

        EventCenter.Broadcast(EventType.CLEAR_TASK);
        if (Global.cfg.nextMap.Count == 0)
        {
            EventCenter.Broadcast(EventType.ADD_TASK, 0, "恭喜通关");
        }
        else
        {
            EventCenter.Broadcast(EventType.ADD_TASK, 0, "找到并击杀BOSS");
        }

        StartMapRound();

        if (Global.isOnlineLogin == false) return;
        SendSaveInfoMsg sendSaveInfoMsg = new SendSaveInfoMsg()
        {
            _lastMapIdx = currMapIdx,
            _lastPlayerInfo = player
        };
        Debug.Log( sendSaveInfoMsg.LastPlayerInfo);
        NetService.SendMsg(SendMsgType.SEND_SAVE_INFO, sendSaveInfoMsg);
    }
}
