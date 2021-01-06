using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 

public class Player : ICharacter
{ 
    [JsonProperty]
    public int tp = 0;

    private AStar m_AStar = new AStar();
    public override void GetInitSkill(ref List<ISkill> passiveSkill)
    {
        // 获取被动技能
        foreach (ISkill skill in m_SkillSystem.skills)
        {
            if (skill.passive)
            {
                passiveSkill.Add(skill);
            }
        }
        foreach (int equipIdx in m_EquipmentSystem.GetUseableEquipment())
        {
            IEquipment equipment = ResFactory.instance.GetEquipmentCfgById(equipIdx);
             
            if (equipment.Skill.passive)
            {
                passiveSkill.Add(equipment.Skill);
            }
            
        }
    }
    public override void EncounterEvent(ICharacter character,Action endCallBack)
    {
 
        m_ControllerSystem.ClearMoveBuffer();
        m_ControllerSystem.DisableLine();

        Vector3 target = character.gameObject.transform.position;
        Vector3 dir = new Vector3(target.x - gameObject.transform.position.x, 0, target.z - gameObject.transform.position.z);
        float angle = Vector3.Angle(gameObject.transform.forward, dir);
        if (angle > 0.1f)
        {
            int sign = Vector3.Cross(gameObject.transform.forward, dir).y >= 0 ? 1 : -1;
            gameObject.transform.Rotate(Vector3.up, Mathf.Abs(angle) * sign);
        }

        GameRoot.instance.Fight(this,character);    
    }


    public override void OnClicked()
    {
    }


    public override void RefreshState()
    {
        this.canMove = true;
        this.canUseAttack = true;
        this.canUseSkill = true;
        m_StateSystem.Clear();

        // 重新计算 装备buff加成
        m_EquipmentSystem.ReLoad(m_StateSystem);

        m_StateSystem.Refresh();
    }

    public override void AddSkill(ISkill skill)
    { 
        Dictionary<int, SkillUpNode> skillUpCfg = ResFactory.instance.GetSkillUpCfgById(skill.id);
        List<SkillUpNode> nodes = new List<SkillUpNode>();
        if (skillUpCfg != null && skillUpCfg.Count != 0) { 
            foreach (KeyValuePair<int, SkillUpNode> kvp in skillUpCfg)
            {
                nodes.Add(kvp.Value);
            }
            m_SkillSystem.skillUp.Add(skill.id, nodes);
        }
        m_SkillSystem.skills.Add(skill);
    }

    public void AddEquipment(IEquipment equipment) {
        m_EquipmentSystem.AddEquipment(m_StateSystem,equipment);
    }

    public void AddProp(IProp prop)
    {
        m_PropSystem.AddProp(prop.id);
    }
    public int UseProp(IProp prop)
    {
        return m_PropSystem.UseProp(prop.id);
    }

    public void AddExp(int exp) {

        currExp += exp;

        while (lv < exps.Count - 1 && currExp >= exps[lv])
        {
            currExp -= exps[lv];
            ++lv;
            ++tp;
        }
        if (currExp > exps[lv]) 
            currExp = exps[lv];  
        OnExpChanged?.Invoke(lv, currExp, exps[lv]);
      
        //EventCenter.Broadcast<int,int,int>(EventType.PLAYER_EXP_UPDATE,lv,currExp,exps[lv]);
    }

    public void ChangeGold(int _gold) {
        gold += _gold;
        if (gold < 0) gold = 0;
        OnGoldChanged?.Invoke(gold);
    } 

    public override void BatttleLose()
    {
        // 销毁物品
        GameObject.Destroy(gameObject);
    }

    /*
        如果有缓存，从缓存取一个进行移动，否则等待选取
     */
    public override void MapRound(MapSystem mapSystem)
    {
        if (m_CanMapMove == false) {
            isEndMapRound = true;
        } 
        m_ControllerSystem.StartMoving();       
    }
    /* 
         仅仅作为路径预测，将以后要走的路径存入缓存
     */
    public override void OnSelected(MapCeilController mapController,Ceil ceil)
    { 
        //如果正在移动，直接停止 

        MapSystem mapSystem = mapController.GetMapSystem();

        /* 
            寻路的时机： 如果人物正在移动，对下一个移动格子进行判断，如果格子不包含事件，起点将会是nextceil 
            如果格子包含事件，起点将会是现在位置
         */
        Ceil nextCeil = m_ControllerSystem.GetNextMoveCeil();
        Stack<Ceil> path;
        List<Vector3> pathList;
        if (m_ControllerSystem.isMoving && nextCeil.CanBePutOnNow)
        {
            Debug.Log("1");
            path = m_AStar.GetPath(mapSystem.ceils, nextCeil, ceil,out pathList);
        }
        else {
            Debug.Log("2");
            path = m_AStar.GetPath(mapSystem.ceils, mapSystem.characterCeilDict[this], ceil, out pathList); 
        }

        if (path == null) {
            EventCenter.Broadcast(EventType.TIPS,"目标无法到达");
            return;
        }

        m_ControllerSystem.RegisterMoveCeilBuffer(path);
        m_ControllerSystem.ShowPathLine(pathList);
    }
     

}
