
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResFactory {

    public static ResFactory instance = null;

    private Dictionary<int, ISkill> m_SkillCfg;
    private Dictionary<int, IBuff> m_BuffCfg;
    private Dictionary<int, IEvent> m_EventCfg;
    private Dictionary<int, CharacterCfg> m_MonsterCfg;
    private Dictionary<int, CharacterCfg> m_PlayerCfg;
    private Dictionary<int, Dictionary<int, SkillUpNode>> m_SkillUpCfg;
    private Dictionary<int, MapCfg> m_MapCfg;
    private Dictionary<int, IProp> m_PropCfg;
    private Dictionary<int, IEquipment> m_EquipCfg;


    private ResLoadSystem m_ResLoadSystem;
    private ObjectPool m_ObjectPool;
     
    private ResFactory()  
    {
        m_ObjectPool = new ObjectPool();
        m_ResLoadSystem = new ResLoadSystem(m_ObjectPool);

    }
    public static void OnInit()
    {
        instance = new ResFactory();
    }

    internal bool InitCfg()
    {
        //try
        //{
            m_SkillCfg = m_ResLoadSystem.LoadSkillCfg();
            m_BuffCfg = m_ResLoadSystem.LoadBuffCfg();
            m_EventCfg = m_ResLoadSystem.LoadEventCfg();
            m_MonsterCfg = m_ResLoadSystem.LoadMonsterCfg();
            m_PlayerCfg = m_ResLoadSystem.LoadPlayerCfg();
            m_SkillUpCfg = m_ResLoadSystem.LoadSkillUpCfg();
            m_MapCfg = m_ResLoadSystem.LoadMapCfg();
            m_EquipCfg = m_ResLoadSystem.LoadEquipmentCfg();
            m_PropCfg = m_ResLoadSystem.LoadPropCfg();

        //}
        //catch (Exception e)
        //{
        //    LogTool.LogError(e.Message);
        //    return false;
        //}
        return true;
    }



    public void RemoveObjectFromPool(RecoverableObject recoverableObject)
    {
        m_ObjectPool.Remove(recoverableObject);
    }
     
    public void OnDestroy()
    {
        m_ObjectPool.Clear();
    }


    #region 具体加载类型

    public AudioClip LoadAudioClip(string name) {
        return m_ResLoadSystem.LoadAudioClip(name);
    }

    public Sprite LoadCharacterIcon(string name) {
        return m_ResLoadSystem.LoadCharacterIcon(name);  
    }
    public Sprite LoadItemIcon(string name)
    {
        return m_ResLoadSystem.LoadItemIcon(name);
    }
    public GameObject LoadMonster(string name)
    {
        return m_ResLoadSystem.LoadMoster(name);
    }
    public GameObject LoadMonsterBattleModel(string name)
    {
        return m_ResLoadSystem.LoadMosterBattleModel(name);
    } 
    public GameObject LoadPlayer(string name)
    {
        return m_ResLoadSystem.LoadPlayer(name);
    }
    public GameObject LoadPlayerBattleModel(string name)
    {
        return m_ResLoadSystem.LoadPlayerBattleModel(name);
    }
    public GameObject LoadFloorBlock(string name)
    { 
        return m_ResLoadSystem.LoadFloorBlock(name);
    }
    public GameObject LoadBuilding(string name) {
        return m_ResLoadSystem.LoadBuilding(name);
    }
    public GameObject LoadWallBlock(string name)
    {

        return m_ResLoadSystem.LoadWallBlock(name);
    }
    public GameObject LoadUIPrefabs(string name)
    {
        return m_ResLoadSystem.LoadUIPrefabs(name);
    }
    public GameObject LoadBattleEffect(string name) { 
        return m_ResLoadSystem.LoadBattleEffect(name);
    }
    internal Sprite LoadEventIcon(string icon)
    {
        return m_ResLoadSystem.LoadEventIcon(icon);
    }
    public Sprite LoadSkillIcon(string name)
    {
        return m_ResLoadSystem.LoadSkillIcon(name);
    } 
    public Sprite LoadStateIcon(string name)
    {
        return m_ResLoadSystem.LoadStateIcon(name);
    }
    #endregion

    #region xml 文件
    public ISkill GetSkillById(int id) {
        if (m_SkillCfg.ContainsKey(id)) 
            return QTool.DeepCopy(m_SkillCfg[id]) as ISkill;
        else
            return null;
    } 
    public List<ISkill> GetSkills() {
        List<ISkill> skills = new List<ISkill>();
        foreach (KeyValuePair<int,ISkill> skill in m_SkillCfg)
        {
            skills.Add(skill.Value);
        }
        return skills;
    }

    public List<ISkill> GetRandomSkills(int cnt,SkillType skillType,HashSet<int> except)
    {
        List<ISkill> skills = new List<ISkill>();
        foreach (KeyValuePair<int, ISkill> skill in m_SkillCfg)
        {
            if (except.Contains(skill.Value.id) || skill.Value.skillType != skillType) continue;
            skills.Add(skill.Value);
        }
        if (skills.Count < cnt) { 
            Debug.LogError("无法获取更多的技能");
            return null;
        }
        
        HashSet<int> resIdSet = new HashSet<int>();
        List<ISkill> res = new List<ISkill>();
        for (int i = 0; i < cnt; i++)
        {
            int skillId;
            do{
                int randIdx = QTool.GetRandomInt(0, skills.Count-1);
                skillId = skills[randIdx].id;
            } while (resIdSet.Contains(skillId));
            resIdSet.Add(skillId);
            ISkill skill = GetSkillById(skillId);
            res.Add(skill);
        } 
        return res;
    }
    public IBuff GetBuffById(int id)
    {
        if (m_BuffCfg.ContainsKey(id))
            return m_BuffCfg[id];
        else
            return null;
    }
    public MapCfg GetMapCfgById(int id) {
        if (m_MapCfg.ContainsKey(id)) {
            return m_MapCfg[id];
        }
        else {
            return null;
        }
    }
    public CharacterCfg GetMonsterCfgById(int id) {
        if (m_MonsterCfg.ContainsKey(id)) {
            return m_MonsterCfg[id];
        }
        else {
            return null;
        }
    }
    public CharacterCfg GetPlayerCfgById(int id)
    {
        if (m_PlayerCfg.ContainsKey(id))
        {
            return m_PlayerCfg[id];
        }
        else
        {
            return null;
        }
    }
    public IEvent GetEventCfgById(int id)
    {
        if (m_EventCfg.ContainsKey(id))
        {
            return m_EventCfg[id];
        }
        else
        {
            return null;
        }
    }
    public Dictionary<int, SkillUpNode> GetSkillUpCfgById(int id)
    {
        if (m_SkillUpCfg.ContainsKey(id))
        {
            return m_SkillUpCfg[id];
        }
        else
        {
            return null;
        }
    }
    public IEquipment GetEquipmentCfgById(int id)
    {
        if (m_EquipCfg.ContainsKey(id))
        {
            return m_EquipCfg[id];
        }
        else
        {
            return null;
        }
    }
    public Dictionary<int,IEquipment> GetAllEquipmentCfg()
    {
        return m_EquipCfg;
    }
    public IProp GetPropCfgById(int id)
    {
        if (m_PropCfg.ContainsKey(id))
        {
            return m_PropCfg[id];
        }
        else
        {
            return null;
        }
    }
    #endregion
}
