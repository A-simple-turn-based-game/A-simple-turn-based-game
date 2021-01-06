 
using System;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
 

public enum BattleProgress { 
    STARTFIGHT, // 战斗开始
    ROUNDTIPS,  // 回合提示
    HALFSTART,  // 半回合开始
    HALFMID,    // 半回合中
    HALFEND,    // 半回合结束
    ENDFIGHT,    // 战斗结束  
    NONE,
};

public class BattleBuffCalInfo {
    public Value val;
    public Value pct;
    public Value offset; 
    public int skillId;
    public HashSet<BuffJudge> judgeSet;
    public BattleBuffCalInfo() {
        val = new Value();
        pct = new Value(100,valueType: ValueType.PERCENT);
        offset = new Value(); 
        skillId = -1;
        judgeSet = null;
    }
    public Value RefreshValue() {
        val = (val + offset) * pct;
        pct = new Value(100, valueType: ValueType.PERCENT);
        offset = new Value();
        skillId = -1;
        judgeSet = null;
        return val;
    }
    public BattleBuffCalInfo(HashSet<BuffJudge> judge)
    { 
        judgeSet = judge;
    }
};

public class BattleSystem
{
    /// <summary>
    /// 用于buff的判断与数据计算
    /// </summary>
    public Player player;
    public Monster monster; 
    // 战斗界面
    private BattlePanel m_BattlePanel;

    // 双方状态  
    private Dictionary<ICharacter, Dictionary<int, List<IBuff>>> m_CharacterBuffRoundInfo = new Dictionary<ICharacter, Dictionary<int, List<IBuff>>>();

    // 记录部分buff带来的收益
    private Dictionary<IBuff, Value> m_BuffValue = new Dictionary<IBuff, Value>();
     
    // 查询角色包含哪些buff 
    private Dictionary<ICharacter, Dictionary<int,int>> m_CharacterBuffSet = new Dictionary<ICharacter, Dictionary<int,int>>();

    // 技能CD 
    private Dictionary<ICharacter, Dictionary<ISkill, int>> m_SkillRecorder = new Dictionary<ICharacter, Dictionary<ISkill, int>>();

    // 装备CD 
    private Dictionary<int, int> m_EquipmentRecorder = new Dictionary<int, int>();

    // 当前回合
    private int m_TurnNum = 0;

    // 是否回合结束
    private bool m_IsPlayerTurn = true;

    // 是否战斗开始了
    private bool m_IsBatttleStart = false;

    // 是否正在执行某个阶段
    private bool m_IsExecutedProgress = false;

    // 当前战斗状态
    public BattleProgress battleProgress { private set; get; }

    // 失败者
    private ICharacter m_Loser = null;

    public void OnInit(Player player, Monster monster, BattlePanel battlePanel) {
        this.player = player;
        this.monster = monster;
        this.m_BattlePanel = battlePanel;
        this.battleProgress = BattleProgress.NONE;
        this.m_Loser = null;
         

        battlePanel.endFightCallBack = EndFightCharacter;
        battlePanel.attackCallBack = Attack;
        battlePanel.useSkillCallBack = UseSkill;
        battlePanel.useEquipmentCallBack = UseEquipment;
        battlePanel.usePropCallBack = UseProp;
      
        player.RefreshState();
        monster.RefreshState();
        battlePanel.InitCharacter(player, monster);

        this.battleProgress = BattleProgress.STARTFIGHT;
        this.m_IsBatttleStart = true;
        this.m_IsExecutedProgress = false;

         
        m_CharacterBuffRoundInfo.Add(player, new Dictionary<int, List<IBuff>>());
        m_CharacterBuffSet.Add(player,new Dictionary<int, int>());
        m_CharacterBuffRoundInfo.Add(monster, new Dictionary<int, List<IBuff>>());
        m_CharacterBuffSet.Add(monster,new Dictionary<int, int>());

        InitSkillRecorder(player);
        InitEquipmentRecorder(player);

        InitSkillRecorder(monster);
    }

    public Dictionary<ICharacter, Dictionary<ISkill, int>> GetSklllRecorder() { return m_SkillRecorder; }

    private void InitSkillRecorder(ICharacter character) {
        m_SkillRecorder.Add(character,new Dictionary<ISkill, int>());
        foreach (ISkill skill in character.GetAllSkills())
        {
            if (m_SkillRecorder[character].ContainsKey(skill)) LogTool.LogWarning("不允许出现重复的技能");
            else m_SkillRecorder[character].Add(skill,0);
        }
    }
    private void InitEquipmentRecorder(ICharacter character)
    { 
        foreach (int equipId in character.GetEquipmentSystem().GetUseableEquipment())
        {
            if (m_EquipmentRecorder.ContainsKey(equipId)) LogTool.LogWarning("不允许出现重复的技能");
            else m_EquipmentRecorder.Add(equipId, 0);
        }
    }

    public void Clear() {
        this.player = null;
        this.monster = null; 
        this.battleProgress = BattleProgress.NONE; 
        this.m_CharacterBuffRoundInfo.Clear();
        this.m_SkillRecorder.Clear();
        this.m_EquipmentRecorder.Clear();
        this.m_CharacterBuffRoundInfo.Clear();
        this.m_CharacterBuffSet.Clear();
        this.m_BuffValue.Clear();
        this.m_TurnNum = 0;
        this.m_IsPlayerTurn = true;
        this.m_Loser = null; 
        this.m_IsBatttleStart = false;
        this.m_IsExecutedProgress = false; 
        ZTimerSvc.ClearAllTask();
    }

    public int GetRoundNum() { return m_TurnNum; }

    public Dictionary<ICharacter, Dictionary<int, List<IBuff>>> GetCharacterBuffRoundInfo() { return m_CharacterBuffRoundInfo; }
    public Dictionary<ICharacter, Dictionary<int, int>> GetCharacterBuffSet() { return m_CharacterBuffSet; }

    private HashSet<int> m_ExecuteBuffSet = new HashSet<int>();
    
    
    private void ExecuteBuff(ICharacter character,BattleBuffCalInfo buffCalInfo = null) {
        // 如果已经分出胜负，停止buff计算
        if (m_Loser != null) return;
        Dictionary<int, List<IBuff>> buffs = m_CharacterBuffRoundInfo[character];
        foreach (KeyValuePair<int,List<IBuff>> kvp in buffs)
        {
            foreach (IBuff buff in kvp.Value)
            {
                if (m_ExecuteBuffSet.Contains(buff.id)) continue;
                m_ExecuteBuffSet.Add(buff.id);
                BuffParser.Parser(this,character,buff, buffCalInfo,m_CharacterBuffSet[character][buff.id]);
                m_ExecuteBuffSet.Remove(buff.id);
            }
        } 
    }

    private void RecordValue(IBuff buff,Value value,string sign) {

        if (!m_BuffValue.ContainsKey(buff))
        {
            m_BuffValue.Add(buff,value);
        }
        else { 
            switch (sign)
            {
                case "+":  
                    m_BuffValue[buff] += value;
                    break;
                case "*":  
                    m_BuffValue[buff] *= value;
                    break;
                default:
                    break;
            }
        }
    }


    #region 战斗消耗增益结算
    
    HashSet<BuffJudge> _physicalTakingJudge = new HashSet<BuffJudge>(){BuffJudge.TAKING_PHYSICAL_DAMAGE,BuffJudge.TAKING_FULL_DAMAGE };
    HashSet<BuffJudge> _physicalInflictJudge = new HashSet<BuffJudge>() { BuffJudge.INFLICT_PHYSICAL_DAMAGE };
    HashSet<BuffJudge> _physicalCritJudge = new HashSet<BuffJudge>() { BuffJudge.DURING_NORMAL_ATTACK_CRITICAL_STRIKE };
    HashSet<BuffJudge> _physicalInflictBeforeJudge = new HashSet<BuffJudge>() { BuffJudge.INFLICT_PHYSICAL_DAMAGE_BEFORE };

    HashSet<BuffJudge> _physicalBeforeTakingJudge = new HashSet<BuffJudge>() { BuffJudge.BEFORE_FULL_DAMAGE,BuffJudge.BEFORE_PHYSICAL_DAMAGE };
    public void PhysicalDamage(ICharacter user ,ICharacter target,Value damage,bool isNormalAttack = false,int skillId = -1) {

        BattleBuffCalInfo battleBuffCalInfo = new BattleBuffCalInfo();
        battleBuffCalInfo.skillId = skillId;

        int rn = QTool.GetRandomInt(0,99); 
        damage.meaningType = ValueMeaningType.PHYSICAL;
        // 注册战斗信息
        battleBuffCalInfo.val = damage;

        battleBuffCalInfo.val -= target.GetStateSystem().def;
        if (battleBuffCalInfo.val.realVal < 0) { battleBuffCalInfo.val.value = 0; }
        
        // 触发暴击
        if (user != null && rn <= user.GetStateSystem().crit.percentVal)
        {
            battleBuffCalInfo.val *= user.GetStateSystem().criticalDamage;
            battleBuffCalInfo.val.meaningType = ValueMeaningType.CRITICAL_DAMAGE; 
        }

        //---------------------------- 前 --------------------- 
        if (user != null) { 
            battleBuffCalInfo.judgeSet = _physicalInflictBeforeJudge;
            ExecuteBuff(user, battleBuffCalInfo);
        }
        if (target != null)
        {
            battleBuffCalInfo.judgeSet = _physicalBeforeTakingJudge;
            ExecuteBuff(target, battleBuffCalInfo);
        }

        battleBuffCalInfo.val = (battleBuffCalInfo.val + battleBuffCalInfo.offset) * battleBuffCalInfo.pct;
        battleBuffCalInfo.val.ConvertToIntValue();
         
        //-----------------------------------------------------

        // TODO 修改为观察者
        target.GetStateSystem().hp -= battleBuffCalInfo.val;

        UpdateHp(target);

        target.GetControllerSystem().Flinch();
        m_BattlePanel.ShowDamge(target, battleBuffCalInfo.val);

        //----------------------------- 后 ---------------------

        // 触发普通攻击暴击buff
        if (isNormalAttack && battleBuffCalInfo.val.meaningType == ValueMeaningType.CRITICAL_DAMAGE) {
            battleBuffCalInfo.judgeSet = _physicalCritJudge;
            ExecuteBuff(user,battleBuffCalInfo);
        } 
        // 触发造成收到buff 
        if (user != null) {
            battleBuffCalInfo.judgeSet = _physicalInflictJudge;
            ExecuteBuff(user, battleBuffCalInfo); 
        }
        battleBuffCalInfo.judgeSet = _physicalTakingJudge;
        ExecuteBuff(target, battleBuffCalInfo);

    }
    HashSet<BuffJudge> _magicTakingJudge = new HashSet<BuffJudge>() { BuffJudge.TAKEING_MAGIC_DAMAGE, BuffJudge.TAKING_FULL_DAMAGE };
    HashSet<BuffJudge> _magicInflictJudge = new HashSet<BuffJudge>() { BuffJudge.INFLICT_MAGIC_DAMAGE };
    HashSet<BuffJudge> _magicInflictBeforeJudge = new HashSet<BuffJudge>() { BuffJudge.INFLICT_MAGIC_DAMAGE_BEFORE };
    HashSet<BuffJudge> _magicTakingBeforeJudge = new HashSet<BuffJudge>() { BuffJudge.BEFORE_FULL_DAMAGE,BuffJudge.BEFORE_MAGIC_DAMAGE };

    public void MagicDamage(ICharacter user,  ICharacter target, Value value,int skillId = -1)
    {
        BattleBuffCalInfo battleBuffCalInfo = new BattleBuffCalInfo();
        battleBuffCalInfo.skillId = skillId;
        value.meaningType = ValueMeaningType.MAGIC;
        battleBuffCalInfo.val = value;
        // --------------------------- 前 ---------------------
        if (user != null) { 
            battleBuffCalInfo.judgeSet = _magicInflictBeforeJudge;
            ExecuteBuff(user, battleBuffCalInfo);
        }
        if (target != null)
        {
            battleBuffCalInfo.judgeSet = _magicTakingBeforeJudge;
            ExecuteBuff(target, battleBuffCalInfo);
        }
        LogTool.Log(">>>>>>>>>>>>>>>>>" );
        // ----------------------------------------------------
        battleBuffCalInfo.val = (battleBuffCalInfo.val + battleBuffCalInfo.offset) * battleBuffCalInfo.pct;
        battleBuffCalInfo.val.ConvertToIntValue();
        target.GetStateSystem().hp -= battleBuffCalInfo.val;
        UpdateHp(target);
        target.GetControllerSystem().Flinch();
        m_BattlePanel.ShowDamge(target, battleBuffCalInfo.val);


        //----------------------------- 后 ---------------------

        if (user != null) {
            battleBuffCalInfo.judgeSet = _magicInflictJudge;
            ExecuteBuff(user, battleBuffCalInfo);
        }
        battleBuffCalInfo.judgeSet = _magicTakingJudge;
        ExecuteBuff(target, battleBuffCalInfo);
    }
    public void ChangeHP(ICharacter character,Value value) {

        // 如果已经分出胜负，停止伤害计算
        if (m_Loser != null) return;

        StateSystem stateSystem = character.GetStateSystem();
        stateSystem.hp += value;
        
        if (value.realVal < 0) {
            character.GetControllerSystem().Flinch();
        }
        else {
            value.meaningType = ValueMeaningType.HEALING; 
            stateSystem.hp.value = Mathf.Min(stateSystem.maxHp.value, stateSystem.hp.value);
        }
        UpdateHp(character);
        m_BattlePanel.ShowDamge(character,value );
    }
    public void ChangeHPPCT(ICharacter character, Value value)
    {

        // 如果已经分出胜负，停止伤害计算
        if (m_Loser != null) return;

        StateSystem stateSystem = character.GetStateSystem();
        Value tmp = stateSystem.hp;
        Value val = value;
        value = stateSystem.hp * value;
        value.ConvertToIntValue();
        stateSystem.hp.value = value.value;
        if (val.value < 10000)
        {
            value = tmp - value;
            value.meaningType = ValueMeaningType.MAGIC;
            character.GetControllerSystem().Flinch();
        }
        else
        {
            value.meaningType = ValueMeaningType.HEALING; 
            stateSystem.hp.value = Mathf.Min(stateSystem.maxHp.value, stateSystem.hp.value); 
        }
        UpdateHp(character);
        m_BattlePanel.ShowDamge(character, value);
    }

    HashSet<BuffJudge> _MPChangeBeforeJudge = new HashSet<BuffJudge>() { BuffJudge.BEFORE_MANA_IS_CONSUMED };
    public void ChangeMP(ICharacter character,Value value) {
        
        BattleBuffCalInfo battleBuffCalInfo = new BattleBuffCalInfo();
         
        StateSystem stateSystem = character.GetStateSystem();
        if (value.realVal < 0) { 
            battleBuffCalInfo.val = value;
            battleBuffCalInfo.judgeSet = _MPChangeBeforeJudge;
            ExecuteBuff(character,battleBuffCalInfo);
            battleBuffCalInfo.RefreshValue();
            battleBuffCalInfo.val.ConvertToIntValue();
            stateSystem.mp += battleBuffCalInfo.val;
        }
        else {
            value.meaningType = ValueMeaningType.MANA;
            stateSystem.mp += value;
            stateSystem.mp.value = Mathf.Min(stateSystem.maxMp.value, stateSystem.mp.value);
            m_BattlePanel.ShowDamge(character, value);
        } 
        m_BattlePanel.UpdateMp();
    }
    public void ChangeMPPCT(ICharacter character, Value value)
    {

        BattleBuffCalInfo battleBuffCalInfo = new BattleBuffCalInfo();

        StateSystem stateSystem = character.GetStateSystem();
        Value tmp = stateSystem.mp;
        Value val = value;
        value = value * stateSystem.mp;
        value.ConvertToIntValue();
        if (val.value < 10000)
        {

            battleBuffCalInfo.val = value - tmp;
            battleBuffCalInfo.judgeSet = _MPChangeBeforeJudge;
            ExecuteBuff(character, battleBuffCalInfo);
            battleBuffCalInfo.RefreshValue();
            battleBuffCalInfo.val.ConvertToIntValue();

            stateSystem.mp += battleBuffCalInfo.val;
        
        }
        else
        {
            value.meaningType = ValueMeaningType.MANA;
            stateSystem.mp = value;
            stateSystem.mp.value = Mathf.Min(stateSystem.maxMp.value, stateSystem.mp.value);
            
            value = tmp - value;
            m_BattlePanel.ShowDamge(character, value);
        }
        m_BattlePanel.UpdateMp();
    }

    public void ChangeCriticalDamage(ICharacter character,IBuff buff, Value value ) {
 
        character.GetStateSystem().criticalDamage_pct *= value; 
        RecordValue(buff,value,"*");
        UpdateCharacterInfo();
    }
    public void ChangeCrit(ICharacter character, IBuff buff, Value value ) { 
        character.GetStateSystem().crit_pct *= value; 
        RecordValue(buff, value, "*");
        UpdateCharacterInfo();
    }
    public void ChangeDefense(ICharacter character, IBuff buff, Value value) {
  
        character.GetStateSystem().def_offset += value; 
        RecordValue(buff, value, "+");
        UpdateCharacterInfo();
    }
    public void ChangeAttack(ICharacter character, IBuff buff, Value value ) {
  
        character.GetStateSystem().atk_offset += value;   
        RecordValue(buff, value, "+");
        UpdateCharacterInfo();
    }
    public void ChangeAttackPCT(ICharacter character, IBuff buff, Value value )
    { 
        character.GetStateSystem().atk_pct *= value; 
        RecordValue(buff, value, "+");
        UpdateCharacterInfo();
    }

    public void EliminateAllDebuff(ICharacter character) {
        Dictionary<int, List<IBuff>> buffs = m_CharacterBuffRoundInfo[character];
        foreach (KeyValuePair<int, List<IBuff>> kvp in buffs)
        {
            int cnt = kvp.Value.Count;
            for(int i = 0; i < kvp.Value.Count; ++i)
            {
                if (kvp.Value[i].canBeDispelled == false) continue;
                if (kvp.Value[i].isDebuff) {
                    m_BattlePanel.RemoveState(character, kvp.Value[i]);
                    kvp.Value.RemoveAt(i);
                    --i;
                }
            }
        }
    }
    public void AddShield() { 
        
    }
    public void DisableSkill(ICharacter character) {

        // UI
        character.canUseSkill = false;
    }
    public void DisableAttack(ICharacter character) {
        // UI
        character.canUseAttack = false;

    } 
    public void Dizziness(ICharacter character) {
        // UI
        character.GetControllerSystem().Dizziness();
        character.canMove = false;
    }
    HashSet<BuffJudge> _addBuffJudge = new HashSet<BuffJudge>() { BuffJudge.EFFECTIVE_IMMEDIATELY_ONLY_TRIGGERED_ONCE };
    public void AddBuff(ICharacter character, IBuff buff , int round) {
   
         
        int targetTurn = round;
        if (round > 0 && m_TurnNum != 0) {
            if (!(character is Player && m_IsPlayerTurn == false))
            {
                targetTurn += m_TurnNum - 1; // 本回合算入持续回合时间
            }
            else {
                targetTurn += m_TurnNum;
            }
        }

        // 如果包含立即触发的特点,就立刻触发
        if (buff.buffCfgNode[0].buffJudge.ContainsKey(BuffJudge.EFFECTIVE_IMMEDIATELY_ONLY_TRIGGERED_ONCE))
        {
            BuffParser.Parser(this, character, buff, new BattleBuffCalInfo(_addBuffJudge), 1);
        }

        // 判断buff叠加方式
        if (m_CharacterBuffSet[character].ContainsKey(buff.id))
        {
            int currRound = 0;
            IBuff currBuff = null;

            Dictionary<int, List<IBuff>> buffs = m_CharacterBuffRoundInfo[character];
            foreach (KeyValuePair<int, List<IBuff>> kvp in buffs)
            {
                foreach (IBuff bf in kvp.Value)
                {
                    if (bf.id == buff.id) {
                        currRound = kvp.Key;
                        currBuff = bf;
                        break;
                    }
                }
                if (currBuff != null) break;
            }
            buffs[currRound].Remove(currBuff);
             

            // 执行效果叠加
            if (buff.effectMode != BuffEffectMode.EFFECT_UNCHANGED) {
                m_CharacterBuffSet[character][buff.id] += 1; 
            }

            // 回合数刷新
            if (buff.roundMode == BuffRoundMode.ROUND_REFRESH) {
                currRound = Mathf.Max(targetTurn, currRound);
            } else if (buff.roundMode == BuffRoundMode.ROUND_STACKING) {
                currRound += round;
            }
            targetTurn = currRound;
            buff = currBuff;
        }
        else {
            m_CharacterBuffSet[character].Add(buff.id,1);
        }
         
        if (!m_CharacterBuffRoundInfo[character].ContainsKey(targetTurn)) {
            m_CharacterBuffRoundInfo[character].Add(targetTurn,new List<IBuff>());
        }
        m_CharacterBuffRoundInfo[character][targetTurn].Add(buff);
         
        m_BattlePanel.AddBuff(character, buff,round);

          


    }

    public void EnableAttack(ICharacter target)
    {
        target.canUseAttack = true;
    }

    public void EnableSkill(ICharacter target)
    {
        target.canUseSkill = true;
    }

    public void CancelDizziness(ICharacter target)
    {
        target.GetControllerSystem().Idel();
        target.canMove = true;
    }



    public void UpdateCharacterInfo()
    {
        player.GetStateSystem().Refresh();

        monster.GetStateSystem().Refresh();
        m_BattlePanel.UpdateCharacterAtrribute();
    }

    #endregion
    public void LoadEffect(ICharacter user, ICharacter target, string effectName ,ReleaseType releaseType)
    { 
        m_BattlePanel.LoadEffect( user, target, effectName,releaseType);
    }

    #region UI 更新

    public void ShowTips(string msg) { EventCenter.Broadcast<string>(EventType.TIPS, msg);}


    public void UpdateHp(ICharacter character) {
        if (m_Loser != null) return;
        m_BattlePanel.UpdateHp(character,()=> { 
            if (character.GetStateSystem().hp.value <= 0) { 
                EndFightCharacter(character);
            }
        });
    }

    #endregion

    #region 战斗选择 

    public bool UsePassiveSkill(ICharacter user, ICharacter target, ISkill skill)
    {
        // ui 表现技能特效
        bool canUse = true;
        float delay = 0;
 
        canUse = SkillParser.Parser(this, user, target, skill, out delay, CharacterState.Idel);
 
        if (canUse)
        {
            m_BattlePanel.ShowSkillNameEffect(skill);   
            ZTimerSvc.AddTask(delay, () =>
            {
                BattleBuffCalInfo battleBuffCalInfo = new BattleBuffCalInfo(_skillJudge);
                battleBuffCalInfo.skillId = skill.id;
                ExecuteBuff(user, battleBuffCalInfo);
            });

            EventCenter.Broadcast<string>(EventType.BATTLEINFO, "使用技能 ：" + skill.name);
        }
        return canUse;
    }


    HashSet<BuffJudge> _skillJudge = new HashSet<BuffJudge>() { BuffJudge.AFTER_USING_SKILL_X , BuffJudge.WHEN_THE_SKILL_IS_RELEASED};
    public bool UseSkill(ICharacter user, ICharacter target, ISkill skill) {
        // ui 表现技能特效
        bool canUse = true;
        float delay = 0;
        if (skill.passive == true)
        {
            canUse = false;
            EventCenter.Broadcast<string>(EventType.TIPS, "无法使用被动技能");
        }
        else if (user.canUseSkill== false)
        {
            canUse = false;
            EventCenter.Broadcast<string>(EventType.TIPS, "沉默中，无法使用技能");
        }
        else
        {
            canUse = SkillParser.Parser(this, user, target, skill, out delay); 
            if (canUse)
            {
                m_BattlePanel.ShowSkillNameEffect(skill);
                m_SkillRecorder[user][skill] = skill.cd + 1;
                if (user == player) m_BattlePanel.UpdateSkillCD(skill, skill.cd + 1);

                ZTimerSvc.AddTask(delay, () =>
                {
                    BattleBuffCalInfo battleBuffCalInfo = new BattleBuffCalInfo(_skillJudge);
                    battleBuffCalInfo.skillId = skill.id;
                    ExecuteBuff(user, battleBuffCalInfo);
                });

                EventCenter.Broadcast<string>(EventType.BATTLEINFO, "使用技能 ：" + skill.name);
            }
        }
        return canUse ;
    }
     
    public bool UseProp(ICharacter user, ICharacter target, IProp prop) {
        Debug.Log(">>>>>>>>>>prop");
        float delay = 0;
        ISkill skill = prop.Skill;
        bool canUse = true;
        if (skill.passive == true)
        {
            canUse = false;
            EventCenter.Broadcast<string>(EventType.TIPS, "无法使用被动技能");
        }
        else
        {
            canUse = SkillParser.Parser(this, user, target, skill, out delay,CharacterState.Command);
            if (canUse)
            {
                //m_BattlePanel.ShowSkillNameEffect(skill);
                int remain = player.UseProp(prop);
                m_BattlePanel.UpdatePropCnt(prop.id, remain);

                ZTimerSvc.AddTask(delay, () =>
                {
                    BattleBuffCalInfo battleBuffCalInfo = new BattleBuffCalInfo(_skillJudge);
                    battleBuffCalInfo.skillId = skill.id;
                    ExecuteBuff(user, battleBuffCalInfo);
                });

                EventCenter.Broadcast<string>(EventType.BATTLEINFO, "使用技能 ：" + skill.name);
            }
        }
        return canUse;
    }

    public bool UseEquipment(ICharacter user, ICharacter target, IEquipment equip) {
        Debug.Log(">>>>>>>>>>Equip");
        ISkill skill = equip.Skill;
        float delay = 0;
        bool canUse = true;
        if (skill.passive == true)
        {
            canUse = false;
            EventCenter.Broadcast<string>(EventType.TIPS, "无法使用被动技能");
        }
        else if (user.canUseSkill == false)
        {
            canUse = false;
            EventCenter.Broadcast<string>(EventType.TIPS, "沉默中，无法使用技能");
        }
        else { 
            canUse = SkillParser.Parser(this, user, target, skill, out delay, CharacterState.Command);
            if (canUse)
            {
                m_BattlePanel.ShowSkillNameEffect(skill);
                m_EquipmentRecorder[equip.id] = skill.cd + 1;
                if (user == player) m_BattlePanel.UpdateEquipmentCD(equip.id, skill.cd + 1);

                ZTimerSvc.AddTask(delay, () =>
                {
                    BattleBuffCalInfo battleBuffCalInfo = new BattleBuffCalInfo(_skillJudge);
                    battleBuffCalInfo.skillId = skill.id;
                    ExecuteBuff(user, battleBuffCalInfo);
                });

                EventCenter.Broadcast<string>(EventType.BATTLEINFO, "使用技能 ：" + skill.name);
            }
        }
        return canUse;
    }
    HashSet<BuffJudge> _attackJudge = new HashSet<BuffJudge>() { BuffJudge.DURING_NORMAL_ATTACK };
    public void Attack(ICharacter user, ICharacter target)
    { 
        StateSystem userStateSys = user.GetStateSystem();
        StateSystem targetStateSys = target.GetStateSystem();

        // 攻击动画 
        float delay = user.GetControllerSystem().Atk(target, () => {  
            PhysicalDamage(user,target,user.GetStateSystem().atk);
        }); 
        SwitchBattleProgress(delay);
        ExecuteBuff(user,new BattleBuffCalInfo(_attackJudge));
    }

    public void EndFightCharacter(ICharacter Loser) {
        if (m_Loser != null) return;
        m_Loser = Loser;
        if (Loser == player)
        {
            // 显示失败界面
            m_BattlePanel.ShowLosePanel(() => SwitchBattleProgress(BattleProgress.ENDFIGHT,Config.BATTLE_WAITING_TIME[5]));
        }
        else {
            // 显示成功界面
             
            player.ChangeGold( monster.gold);
            player.AddExp(monster.exps[0]);

            m_BattlePanel.ShowWinPanel(() => SwitchBattleProgress(BattleProgress.ENDFIGHT, Config.BATTLE_WAITING_TIME[5]));
        }
        //Loser.BatttleLose();
    }


    #endregion

    #region 回合进度

    // 修改战斗状态
    public void SwitchBattleProgress(BattleProgress battleProgress,float delay = 0)
    { 
        ZTimerSvc.AddTask(delay, () => {
            this.battleProgress = battleProgress;
            m_IsExecutedProgress = false;
        });
    }
    public void SwitchBattleProgress(float delay = 0)
    {
        if (m_Loser != null) return;
        float time = Config.BATTLE_WAITING_TIME[(int)battleProgress] + delay;
         ZTimerSvc.AddTask(time, () => {
            switch (battleProgress)
            {
                case BattleProgress.STARTFIGHT:this.battleProgress = BattleProgress.ROUNDTIPS;
                    break;
                case BattleProgress.ROUNDTIPS:this.battleProgress = BattleProgress.HALFSTART;
                    break;
                case BattleProgress.HALFSTART:this.battleProgress = BattleProgress.HALFMID;
                    break;
                case BattleProgress.HALFMID:this.battleProgress = BattleProgress.HALFEND;
                    break;
                case BattleProgress.HALFEND:this.battleProgress = m_IsPlayerTurn ? BattleProgress.ROUNDTIPS : BattleProgress.HALFSTART; 
                    break;
                case BattleProgress.ENDFIGHT: this.battleProgress = BattleProgress.NONE ; return;
                default:
                    break;
            }
            m_IsExecutedProgress = false;
        });
    }
    public void OnUpdate(){

        if (!m_IsBatttleStart || m_IsExecutedProgress) { return; }
        m_IsExecutedProgress = true;
        LogTool.Log(battleProgress);
        switch (battleProgress)
        {
            case BattleProgress.STARTFIGHT:StartFight(); 
                break;
            case BattleProgress.ROUNDTIPS: RoundTips(); 
                break;
            case BattleProgress.HALFSTART: HalfRoundStart(); 
                break;
            case BattleProgress.HALFMID: HalfRoundMid();  
                break;
            case BattleProgress.HALFEND: HalfRoundEnd(); 
                break;
            case BattleProgress.ENDFIGHT: EndFight(); 
                break;
            case BattleProgress.NONE:
                break; 
            default:
                break;
        } 
    }
     

    // 战斗开始
    public void StartFight() {

        List<ISkill> pSkill = new List<ISkill>();
        List<ISkill> mSkill = new List<ISkill>();
        player.GetInitSkill(ref pSkill);
        
        foreach (ISkill skill in pSkill) {
            UsePassiveSkill(player,monster,skill);
        }
        monster.GetInitSkill(ref mSkill);
        // ui 表现
        foreach (ISkill skill in mSkill)
        {
            UsePassiveSkill(monster, player, skill);
        }
        SwitchBattleProgress();
    }
    // 战斗回合提示
    public void RoundTips() { 
        if(m_IsPlayerTurn) ++m_TurnNum; // 回合数增加
        m_BattlePanel.ShowRoundTips(m_TurnNum);
        SwitchBattleProgress();
    }

    // 在半回合开始时调用
    public void HalfRoundStart() {

        ICharacter character = m_IsPlayerTurn?(ICharacter)player:monster;
        ExecuteBuff(character);
        SwitchBattleProgress();
    }

    // 在半回合中调用
    public void HalfRoundMid() {

        ICharacter character = m_IsPlayerTurn ? (ICharacter)player : monster;
        ExecuteBuff(character);

        if (m_IsPlayerTurn)
        {
            if (player.canMove == false) {
                SwitchBattleProgress();
                return;
            }
            // 解除 UI 使用 
            m_BattlePanel.PlayerTurnStart(m_TurnNum);
        }
        else {

            if (monster.canMove == false) {
                SwitchBattleProgress();
                return;
            }
            // 调用 monster AI 
            monster.BatttleAI(this);
        } 
    }

    // 在半回合结束时调用
    public void HalfRoundEnd() {


        ICharacter character = m_IsPlayerTurn ? (ICharacter)player : monster;
        ExecuteBuff(character);
         

        m_IsPlayerTurn = !m_IsPlayerTurn; // 切换
         // 技能 结算 
        foreach (ISkill skill in character.GetAllSkills())
        {
            if (m_SkillRecorder[character][skill] > 0)
            {   
                if(character is Player) m_BattlePanel.UpdateSkillCD(skill, m_SkillRecorder[character][skill] - 1);
                m_SkillRecorder[character][skill] -= 1;
            }
        }

        // 装备 结算 
        if (character is Player) { 
            foreach (int equipId in character.GetEquipmentSystem().GetUseableEquipment())
            {
                if (m_EquipmentRecorder[equipId] > 0)
                {
                    m_BattlePanel.UpdateEquipmentCD(equipId, m_EquipmentRecorder[equipId] - 1);
                    m_EquipmentRecorder[equipId] -= 1;
                }
            }
        }

        // buff结算 以及 cd 冷却更新
        if (m_CharacterBuffRoundInfo[character].ContainsKey(m_TurnNum))
        {
            foreach (IBuff buff in m_CharacterBuffRoundInfo[character][m_TurnNum])
            {
                Value val;
                m_BuffValue.TryGetValue(buff,out val);
                BuffParser.Cancel(this,character,buff,val);
                m_BuffValue.Remove(buff);
                m_BattlePanel.RemoveState(character, buff); 
                m_CharacterBuffSet[character].Remove(buff.id);
            } 
            m_CharacterBuffRoundInfo[character].Remove(m_TurnNum);
        }

        foreach (KeyValuePair<int,List<IBuff>> kvp in m_CharacterBuffRoundInfo[character])
        {
            if (kvp.Key < m_TurnNum) continue;
            foreach (IBuff buff in kvp.Value)
            {
                m_BattlePanel.UpdateBuff(character,buff,kvp.Key - m_TurnNum); 
            }
        }

        UpdateCharacterInfo();
        SwitchBattleProgress();
    }



    // 战斗结束
    public void EndFight() {

        LogTool.Log("结束啦！");
          
        // TODO 切换界面
        m_BattlePanel.BattleEnd();
        ICharacter character = m_Loser;
         
        // 打扫战场
        Clear();

        if (character is Player)
        { 
            EventCenter.Broadcast<ICharacter>(EventType.PLAYERDIE, character);
        }
        else {
            EventCenter.Broadcast<ICharacter>(EventType.MONSTERDIE, character);
        }
    }
    #endregion

}
