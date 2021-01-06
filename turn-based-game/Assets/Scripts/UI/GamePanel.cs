using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    private MapSystem m_MapSystem;
    private FingerListener m_FingerListener;

    #region test
    private Button m_DemoRefresh;
    private Dropdown m_DemoSelectSkill1;
    private Dropdown m_DemoSelectSkill2;
    private Dropdown m_DemoSelectSkill3;

    private Button m_NetTest;

    #endregion
    private Button m_LevelUpBtn;
    private Button m_BackBtn;
    private Button m_EquipmentBtn;
    private Button m_PropBtn;
    private Button m_PictorialBtn;
    private Button m_SkillsBtn;
    private Button m_PlayerBtn;

    private Text m_NameText;
    private Text m_LVText;
    private Slider m_HPSlider;
    private Text m_HPText;
    private Slider m_MPSlider;
    private Text m_MPText;

    private Text m_LVScore;
    private Text m_Gold;
    private Text m_LevelName;
    private Slider m_LVScroll;
    private SkillUpGroup m_SkillUpGroup;
    private EventGroup m_EventPanel;
    private GameInfoGroup m_GameInfoGroup;
    private ShopGroup m_ShopGroup;
    private GameTaskGroup m_GameTaskGroup;
    private Player m_Player;
     

    private void Awake()
    {
        GameNetSys.instance.OnInit(this);
        EventCenter.AddListener<ICharacter>(EventType.MONSTERDIE, MonsterDie);
        EventCenter.AddListener<ICharacter>(EventType.PLAYERDIE, PlayerDie);
        EventCenter.AddListener<ICharacter, IEvent, IBuilding>(EventType.EVENT, TriggeringEvent); 

    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<ICharacter>(EventType.MONSTERDIE, MonsterDie);
        EventCenter.RemoveListener<ICharacter>(EventType.PLAYERDIE, PlayerDie);
        EventCenter.RemoveListener<ICharacter, IEvent, IBuilding>(EventType.EVENT, TriggeringEvent); 

    }

    private void OnInit()
    {

        m_HPSlider = transform.Find("State/HpInfo/HpBox").GetComponent<Slider>();
        m_HPText = transform.Find("State/HpInfo/HpText").GetComponent<Text>();
        m_MPSlider = transform.Find("State/MpInfo/MpBox").GetComponent<Slider>();
        m_MPText = transform.Find("State/MpInfo/MpText").GetComponent<Text>(); 
        m_LVText = transform.Find("State/PlayerInfo/LV").GetComponent<Text>();
        m_NameText = transform.Find("State/PlayerInfo/Name").GetComponent<Text>();

        m_NetTest = transform.Find("Test/NetTest").GetComponent<Button>();
        m_DemoRefresh = transform.Find("Test/DemoRefresh").GetComponent<Button>();
        m_DemoSelectSkill1 = transform.Find("Test/DemoSkillSelect1").GetComponent<Dropdown>();
        m_DemoSelectSkill2 = transform.Find("Test/DemoSkillSelect2").GetComponent<Dropdown>();
        m_DemoSelectSkill3 = transform.Find("Test/DemoSkillSelect3").GetComponent<Dropdown>();

        m_MapSystem = GameObject.Find("MapGenerator").GetComponent<MapSystem>();
        m_MapSystem.OnInit();
        m_MapSystem.MapUpdateCallBack = MapUpdate;

        m_BackBtn = transform.Find("BackBtn").GetComponent<Button>();
        m_Gold = transform.Find("Resources/Gold/Text").GetComponent<Text>();

        m_LevelName = transform.Find("LevelName").GetComponent<Text>();
       
        m_EventPanel = transform.Find("EventPanel").GetComponent<EventGroup>();
        m_EventPanel.OnInit();
        m_LVScroll = transform.Find("LVScroll").GetComponent<Slider>();
        m_LVScore = transform.Find("LVScroll/LVText").GetComponent<Text>();
        m_LevelUpBtn = transform.Find("LevelUpBtn").GetComponent<Button>();
        m_LevelUpBtn.gameObject.SetActive(false);
        m_SkillUpGroup = transform.Find("SkillUpGroup").GetComponent<SkillUpGroup>();
        m_SkillUpGroup.OnInit();
        m_SkillUpGroup.gameObject.SetActive(false);

        m_GameInfoGroup = transform.Find("GameInfoGroup").GetComponent<GameInfoGroup>();
        m_GameInfoGroup.OnInit(this);
        m_GameInfoGroup.gameObject.SetActive(false);

        m_ShopGroup = transform.Find("ShopGroup").GetComponent<ShopGroup>();
        m_ShopGroup.OnInit(this);
        m_GameInfoGroup.gameObject.SetActive(false);

        m_GameTaskGroup = transform.Find("Task").GetComponent<GameTaskGroup>();
        m_GameTaskGroup.OnInit();

        m_PlayerBtn = transform.Find("State/PlayerBtn").GetComponent<Button>();
        m_EquipmentBtn = transform.Find("EquipmentBtn").GetComponent<Button>();
        m_PropBtn = transform.Find("PropBtn").GetComponent<Button>();
        m_PictorialBtn = transform.Find("PictorialBtn").GetComponent<Button>();
        m_SkillsBtn = transform.Find("SkillsBtn").GetComponent<Button>();

        m_FingerListener = gameObject.AddComponent<FingerListener>();
        m_FingerListener.onFingerSwipe = GameRoot.instance.RoateLens;
        m_FingerListener.onFingerZoom = GameRoot.instance.Zoom;
           
         
    }

    private void Start()
    {
        InitDropDown();
        m_DemoRefresh.onClick.AddListener(() =>
        {
            m_MapSystem.GoToMap(1001); 
            m_DemoSelectSkill1.value = 0;
            m_DemoSelectSkill2.value = 0;
            m_DemoSelectSkill3.value = 0;
        });

        m_LevelUpBtn.onClick.AddListener(()=> {
            m_SkillUpGroup.gameObject.SetActive(true);
            m_SkillUpGroup.SkillUpByTp(m_Player);
            m_LevelUpBtn.gameObject.SetActive(false);
        });

        m_NetTest.onClick.AddListener(() =>
        {
            SendSaveInfoMsg sendSaveInfoMsg = new SendSaveInfoMsg
            {
                _lastMapIdx = m_MapSystem.currMapIdx,
                _lastPlayerInfo = m_Player
            };
            string m = Newtonsoft.Json.JsonConvert.SerializeObject(sendSaveInfoMsg);
            Debug.Log(m);
            //NetService.SendMsg(SendMsgType.SEND_SAVE_INFO, sendSaveInfoMsg); 
        });

        m_BackBtn.onClick.AddListener(()=> PlayerDie(null));

        m_PlayerBtn.onClick.AddListener(()=> {
            m_GameInfoGroup.gameObject.SetActive(true);
            m_GameInfoGroup.SelectPlayerTab();
        });

        m_EquipmentBtn.onClick.AddListener(()=> {
            m_GameInfoGroup.gameObject.SetActive(true);
            m_GameInfoGroup.SelectEquipmentTab();
        });

        m_PropBtn.onClick.AddListener(()=> {
            m_GameInfoGroup.gameObject.SetActive(true);
            m_GameInfoGroup.SelectPropTab();
        });

        m_SkillsBtn.onClick.AddListener(()=> {
            m_GameInfoGroup.gameObject.SetActive(true);
            m_GameInfoGroup.SelectSkillsTab();
        });

        m_PictorialBtn.onClick.AddListener(()=> {
            m_GameInfoGroup.gameObject.SetActive(true);
            m_GameInfoGroup.SelectMonsterTab();
        });
    }
  
    private void TriggeringEvent(ICharacter character,IEvent @event,IBuilding baseBuilding) {

        switch (@event.type)
        {
            case GameEventType.SelectEvents:
                m_EventPanel.RegisterEvent(this,character,@event,baseBuilding);
                m_EventPanel.gameObject.SetActive(true);
                break;
            case GameEventType.TriggeringEvent:
                foreach (List<Value> effect in @event.effects[0])
                { 
                    EventEffectParser.Parser(this,character,effect); 
                } 
                break;
            default:
                break;
        }
 
    }

    public void ShowShop() { 
        m_ShopGroup.gameObject.SetActive(true);
    }

    public List<int> GetMonsters() {
        List<int> monsters = new List<int>();
        MapCfg cfg = ResFactory.instance.GetMapCfgById(m_MapSystem.currMapIdx);
        foreach (int idx in cfg.monster.Keys) {
            monsters.Add(idx);
        }
        return monsters;
    }
    public Player GetPlayer() { return m_Player; }

    public MapSystem GetMapSystem() { return m_MapSystem; }

    public void StartGame() { 

        // TODO 角色有待调整
        m_Player = CharacterFactory.instance.GeneratePlayer(1001);
        
        // 注册事件  
        m_Player.OnExpChanged += PlayerExpUpdate;
        m_Player.OnGoldChanged += PlayerGoldUpdate;
        m_Player.OnStateChanged += PlayerStateUpdate;

        m_NameText.text = m_Player.name;
        PlayerGoldUpdate(m_Player.gold);
        PlayerExpUpdate(m_Player.lv,m_Player.currExp,m_Player.exps[m_Player.lv]);
        PlayerStateUpdate(m_Player.GetStateSystem());

        m_MapSystem.player = m_Player;
        m_MapSystem.GoToMap(1001);
        m_ShopGroup.InitShop(m_Player);



    }
    public void ContinueGame(int mapIdx, Player player)
    {  
        m_Player = CharacterFactory.instance.CompletionPlayer(player);

        // 注册事件  
        m_Player.OnExpChanged += PlayerExpUpdate;
        m_Player.OnGoldChanged += PlayerGoldUpdate;
        m_Player.OnStateChanged += PlayerStateUpdate;

        m_NameText.text = m_Player.name;
        PlayerGoldUpdate(m_Player.gold);
        PlayerExpUpdate(m_Player.lv, m_Player.currExp, m_Player.exps[m_Player.lv]);
        PlayerStateUpdate(m_Player.GetStateSystem());

        m_MapSystem.player = m_Player;
        m_MapSystem.GoToMap(mapIdx);
        m_ShopGroup.InitShop(m_Player);
         
    }


    #region 消息

    private void MapUpdate(string name) { 
        m_LevelName.text = name; 
    }
     
    private void PlayerExpUpdate(int lv,int currExp,int maxExp) {
        m_LVText.text = "LV " + lv;
        m_LVScore.text = currExp +" / " + maxExp;
        m_LVScroll.value = (float)currExp / maxExp;
        Debug.Log(currExp + "   " + maxExp + "   " + (float)currExp / maxExp);
        if (m_Player.tp != 0) {
            m_LevelUpBtn.gameObject.SetActive(true);
        }
    }

    private void PlayerGoldUpdate(int gold) {
        m_Gold.text =  "" + gold;
    
    }

    public void PlayerStateUpdate(StateSystem stateSystem) {
        m_HPText.text = stateSystem.hp.realVal + " / " + stateSystem.maxHp.realVal;
        m_MPText.text = stateSystem.mp.realVal + " / " + stateSystem.maxMp.realVal;
        m_HPSlider.value = (float)stateSystem.hp.realVal / stateSystem.maxHp.realVal;
        m_MPSlider.value = (float)stateSystem.mp.realVal / stateSystem.maxMp.realVal;
    }


    private void PlayerDie(ICharacter character) {
        // TODO 镜头特写  
        Debug.Log("PlayerDie");
        GameRoot.instance.LoadSceneAsync((int)Config.SCENETYPE.Lobby,()=> {
            Debug.Log("GamePanel");
            m_MapSystem.ClearMap();
            uiManager.PopPanel();
        }); 
    }
     

    private void MonsterDie(ICharacter monster) {
        GameRoot.instance.PlayBgSound(ResFactory.instance.LoadAudioClip(Global.cfg.bgm));
        Debug.Log("monster die");
        //m_MapSystem.characterCeilDict[monster].character = null;
        // 触发死亡效果
        if (monster.deadEventEffect != null) { 
            foreach (List<Value> item in monster.deadEventEffect)
            {
                EventEffectParser.Parser(this,monster,item);
            }
        }
        m_MapSystem.RemoveCharacter(monster); 
    }

    #endregion
     
    #region UI 生命周期
    public override void OnEnter()
    {
        base.OnEnter(); 
        gameObject.SetActive(true);
        OnInit(); 
        GameRoot.instance.OpenRaycasterLayer("Block");

    }

    public override void OnExit()
    {
        base.OnExit();

        gameObject.SetActive(false);
        m_MapSystem.ClearMap();
        GameRoot.instance.CloseRaycasterLayer("Block");
    }

    public override void OnPuase()
    {
        base.OnPuase();
        gameObject.SetActive(false);
        GameRoot.instance.CloseRaycasterLayer("Block");
    }

    public override void OnResume()
    {
        base.OnResume();
        gameObject.SetActive(true);
        GameRoot.instance.OpenRaycasterLayer("Block");
    }
    #endregion
    
    #region 测试
    List<ISkill> _skills;
    public void InitDropDown()
    { 
        List<Dropdown.OptionData> listOptions = new List<Dropdown.OptionData>();
        _skills = ResFactory.instance.GetSkills();
        listOptions.Add(new Dropdown.OptionData("默认"));
        foreach (ISkill skill in _skills)
        { 
            listOptions.Add(new Dropdown.OptionData(skill.name));
        }
        m_DemoSelectSkill1.AddOptions(listOptions);
        m_DemoSelectSkill2.AddOptions(listOptions);
        m_DemoSelectSkill3.AddOptions(listOptions);
        m_DemoSelectSkill1.onValueChanged.AddListener((id) =>
        {
            if (id == m_DemoSelectSkill2.value || id == m_DemoSelectSkill3.value) m_DemoSelectSkill1.value = 0;
            if (id == 0) return; 
            m_Player.GetAllSkills()[0] = _skills[id-1];
            Debug.Log(m_Player.GetAllSkills()[0].name);
        });
        m_DemoSelectSkill2.onValueChanged.AddListener((id) =>
        {
            if (id == m_DemoSelectSkill1.value || id == m_DemoSelectSkill3.value) m_DemoSelectSkill2.value = 0;
            if (id == 0) return;
            m_Player.GetAllSkills()[1] = _skills[id-1];
            Debug.Log(m_Player.GetAllSkills()[1].name);
        });
        m_DemoSelectSkill3.onValueChanged.AddListener((id) =>
        {
            if (id == m_DemoSelectSkill1.value || id == m_DemoSelectSkill2.value) m_DemoSelectSkill3.value = 0;
            if (id == 0) return;
            m_Player.GetAllSkills()[2] = _skills[id-1];
            Debug.Log(m_Player.GetAllSkills()[2].name);
        });
    }
    #endregion
}
