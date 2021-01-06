using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public abstract class ICharacter
{
    #region 事件
    public delegate void GoldHandler(int gold);
    public delegate void ExpHandler(int lv, int currExp, int maxExp);
    public delegate void StateHandler(StateSystem stateSystem);
    public GoldHandler OnGoldChanged;
    public ExpHandler OnExpChanged;
    public StateHandler OnStateChanged;
    #endregion

    [JsonProperty]
    public string name;
    [JsonProperty]
    public string description;
    [JsonProperty]
    public int lv;
    [JsonProperty]
    public int gold;
    [JsonProperty]
    public int currExp;
    [JsonProperty]
    public int actionPower = 1;
    [JsonProperty]
    public List<int> exps;

    // 死亡后会触发的效果
    public List<List<Value>> deadEventEffect;

    public GameObject gameObject;

    // 是否结束当前行动
    public bool isEndMapRound;
    // 是否能使用技能
    public bool canUseSkill;
    // 是否能普通攻击
    public bool canUseAttack;
    // 是否处于眩晕状态
    public bool canMove;
     
    // 在地图上能否移动 
    protected bool m_CanMapMove; 
    
    // 角色AI
    protected ICharacterAI m_CharacterAI;
    

    // 音效管理 
    protected AudioManager m_AudioManager;

    // 技能管理  
    [JsonProperty]
    protected SkillSystem m_SkillSystem;

    // 控制管理 (点击移动攻击)  
    protected ControllerSystem m_ControllerSystem;

    // 状态管理
    [JsonProperty]
    protected StateSystem m_StateSystem;

    // 道具管理
    [JsonProperty]
    protected PropSystem m_PropSystem;

    // 装备管理
    [JsonProperty]
    protected EquipmentSystem m_EquipmentSystem;
     

    public abstract void RefreshState();

    public abstract void OnClicked();

    public abstract void OnSelected(MapCeilController mapController, Ceil ceil);

    public abstract void EncounterEvent(ICharacter character,Action endCallBack);
     
    public void RegisterSkillSystem(SkillSystem skillSystem) { this.m_SkillSystem = skillSystem; }

    public void RegisterStateSystem(StateSystem stateSystem) { this.m_StateSystem = stateSystem; }

    public void RegisterControllerSystem(ControllerSystem controllerSystem) { this.m_ControllerSystem = controllerSystem; }

    public void RegisterEquipmentSystem(EquipmentSystem equipmentSystem) { this.m_EquipmentSystem = equipmentSystem; }

    public void RegisterPropSystem(PropSystem propSystem) { this.m_PropSystem = propSystem; }

    public StateSystem GetStateSystem() { return m_StateSystem; }
    public EquipmentSystem GetEquipmentSystem() { return m_EquipmentSystem; }
    public PropSystem GetPropSystem() { return m_PropSystem; }
    public ControllerSystem GetControllerSystem() { return m_ControllerSystem; }
    public void SetAI(ICharacterAI characterAI) {
        this.m_CharacterAI = characterAI;
        characterAI.character = this;
        characterAI.controllerSystem = m_ControllerSystem;
        characterAI.equipmentSystem = m_EquipmentSystem;
        characterAI.propSystem = m_PropSystem;
        characterAI.skillSystem = m_SkillSystem;
        characterAI.stateSystem = m_StateSystem;
    }
     

    public List<ISkill> GetAllSkills() {
        return m_SkillSystem.skills;
    } 
    public Dictionary<int, List<SkillUpNode>> GetSkillUpInfo() { return m_SkillSystem.skillUp; }

    [Obsolete]
    public void ShowValue(Value value) {
        m_ControllerSystem.ShowValue(value);
    }
    public Transform GetValuePos()
    {
        return m_ControllerSystem.GetValuePos();
    }
    public void EnableMapMove() { this.m_CanMapMove = true; }
    public void DisableMapMove() { this.m_CanMapMove = false; }


    // 战斗之前生效的基础状态
    public virtual void GetInitSkill(ref List<ISkill> passiveSkill) {}
     
    public virtual void BatttleAI(BattleSystem battleSystem) {
       
        m_CharacterAI.BattleAI(battleSystem);
    }

    public virtual void BatttleLose(){}
    public virtual void AddSkill(ISkill skill) { }
    public virtual void MapRound(MapSystem mapSystem){}
}
