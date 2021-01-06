using System;
using System.Collections;
using System.Collections.Generic;
/* 
     //冷却时间x回合 
    //在一回合内重复x次 

    //附加编号为x的buff，持续x回合
    ADD_TARGET_BUFF,
    //获得编号为x的buff，持续x回合
    ADD_USER_BUFF,
 */


public class SkillCfgNode
{
    public Dictionary<SkillJudge,Value> skillJudge;
    public Dictionary<SkillCycleJudge,Value> skillCycleJudge;

    public CycleType cycleType;
    
    public Dictionary<SkillAction,Value> skillAction;
    //技能 编号  回合数
    public Dictionary<int, int> userBuff;
    public Dictionary<int, int> targetBuff;
    public string effectName;
    public float effectTime;
    // 释放方式
    public ReleaseType releaseType;
     
    public SkillCfgNode()
    {
        this.skillJudge = new Dictionary<SkillJudge, Value>();
        this.skillCycleJudge = new Dictionary<SkillCycleJudge, Value>();
        this.skillAction = new Dictionary<SkillAction, Value>();
        this.userBuff = new Dictionary<int, int>();
        this.targetBuff = new Dictionary<int, int>();
        this.cycleType = CycleType.None;
        this.releaseType = ReleaseType.Missile;
        this.effectName = null;
        this.effectTime = 0;
    }
};
/// <summary>
/// 技能条件判定
/// </summary>
public enum SkillJudge
{
    // x点生命值 会扣除
    X_HEALTH_COST = 1001,
    // x点法力值 会扣除
    X_MANA_COST,
    //若现在是第x回合
    IF_IT_IS_THE_XTH_ROUND,
    //若现在不是第x回合
    IF_IT_IS_NOT_THE_XTH_ROUND,
    // x点生命值 仅仅做检查
    X_HEALTH_CHECK,
    // x点法力值 仅仅做检查
    X_MANA_CHECK,
};

/// <summary>
/// 技能循环条件判定
/// </summary>
public enum SkillCycleJudge {

    // 每失去x%生命值
    EVERY_X_PERCENT_OF_HEALTH_LOST_PCT = 2001,
    //每失去x%法力值
    EVERY_X_PERCENT_OF_MANA_LOST_PCT,
    //在一回合内重复x次 Repeat x times in a round
    REPEAT_X_TIMES_IN_A_ROUND,
};

public enum CycleType {
    // 重复施法 Repeat cast
    REPEAT_CAST = 3001,
    // 效果叠加  Effect stacking
    EFFECT_STACKING,
    // 无
    None,
};

/// <summary>
/// 技能行为
/// </summary>
public enum SkillAction
{
    //造成x点物理伤害 Inflict x physical damage
    INFLICT_X_PHYSICAL_DAMAGE = 4001,
    //造成x点魔法伤害
    INFLICT_X_MAGIC_DAMAGE,
    //受到x点物理伤害
    TAKE_X_PHYSICAL_DAMAGE,
    //受到x点魔法伤害
    TAKE_X_MAGIC_DAMAGE,
    //消耗x点生命值 Consume x points of health
    ADJUST_X_HEALTH,
    //消耗x点法力值
    ADJUST_X_MANA,

    //造成物理伤害的x%
    INFLICT_X_PERCENT_PHYSICAL_DAMAGE_PCT,
    //受到物理伤害的x%
    TAKE_X_PERCENT_PHYSICAL_DAMAGE_PCT,

    //回复x点生命值 (obsolete)
    [Obsolete]
    RESTORE_X_POINTS_OF_HEALTH,
    //回复x点法力值
    [Obsolete]
    RESTORE_X_POINTS_OF_MANA,
    //调整x%生命值
    ADJUST_X_PERCENT_OF_HEALTH_PCT = 4011,
    //调整x%法力值
    ADJUST_X_PERCENT_OF_MANA_PCT = 4012,

    //消除所有debuff Eliminate all Debuff
    ELIMINATE_ALL_DEBUFF,
     
    //获得x点护盾
    GET_X_POINTS_SHIELD,

    //造成法术伤害的x%
    INFLICT_X_PERCENT_MAGIC_DAMAGE_PCT,
    //受到法术伤害的x%
    TAKE_X_PERCENT_MAGIC_DAMAGE_PCT,
};
public class SkillParser
{
    // 逻辑 + 特效 + 提示 + 数值显示

    /***
    1. 对于无法施展的技能，只显示最前面条件不足信息


     ***/
    private static string msg = "";
    private static float m_MutiTime = 0.8f;
    public static bool Parser(BattleSystem battleSystem,ICharacter user,ICharacter target,ISkill skill,
        out float skillDelay, 
        CharacterState characterState = CharacterState.Skill) {

        msg = "";
        bool canUse = false;
        float delay = 0;
        List<SkillCfgNode> cfgNodes = skill.skillCfgNodes;

        // 获取主要技能节点
        SkillCfgNode mainNode = cfgNodes[0];

        // 判断释放主条件
        if (!JudgeParser(battleSystem, user, target, mainNode.skillJudge) && user is Player)
        {
            battleSystem.ShowTips(msg);
            skillDelay = 0f;
            return canUse;
        }
        else { 
            canUse = true;
        }


        // 判断循环次数
        int cycle = CycleJudgeParser(battleSystem, user, target, mainNode.skillCycleJudge);


        // 执行动作
        Action action = () =>
        {
            // 主技能 特效触发与伤害计算
            EffectAndDemage(battleSystem, user, target, skill, mainNode, cycle);
            // 额外附加技能
            for (int i = 1; i < cfgNodes.Count; ++i)
            {
                SkillCfgNode node = cfgNodes[i];
                // 判断释放条件
                if (!JudgeParser(battleSystem, user, target, node.skillJudge)) { continue; }

                // 判断循环次数
                int additionalCycle = CycleJudgeParser(battleSystem, user, target, node.skillCycleJudge);

                EffectAndDemage(battleSystem, user, target, skill, node, additionalCycle);

            }
        };
        if (characterState == CharacterState.Skill)
        {
            delay += user.GetControllerSystem().Skill(action);
        }
        else if (characterState == CharacterState.Command)
        {
            delay += user.GetControllerSystem().Command(action);
        }
        else {
            action.Invoke();
        } 

        if (mainNode.cycleType == CycleType.REPEAT_CAST)
            delay += (mainNode.effectTime + m_MutiTime) * cycle  + 0.1f;
        else
            delay += mainNode.effectTime + 0.1f;
    
        skillDelay = delay;
        if(skill.passive == false)battleSystem.SwitchBattleProgress(delay);
        return canUse;
    }

    private static void EffectAndDemage(BattleSystem battleSystem,ICharacter user,ICharacter target,ISkill skill,SkillCfgNode node,int cycle) { 
        
            if (node.cycleType == CycleType.REPEAT_CAST)
            { 
                // 多段施法
                for (int i = 0; i < cycle; i++)
                {
                    if (string.IsNullOrEmpty(node.effectName))
                    {
                        ActionParser(battleSystem, user, target, node.skillAction,skill.id);
                        SkillBuffParser(battleSystem, user, node.userBuff);
                        SkillBuffParser(battleSystem, target, node.targetBuff);
                    }
                    else {
                        ZTimerSvc.AddTask(node.effectTime * i + m_MutiTime * i, () =>
                        {
                            battleSystem.LoadEffect(user,target,node.effectName,node.releaseType);
                        });
                        ZTimerSvc.AddTask(node.effectTime * (i + 1) + m_MutiTime * (i + 1), () =>
                        {
                            ActionParser(battleSystem, user, target, node.skillAction, skill.id);
                            SkillBuffParser(battleSystem, user, node.userBuff);
                            SkillBuffParser(battleSystem, target, node.targetBuff);
                        });
                    }
                }
            }
            else {
                // 效果叠加
                if (string.IsNullOrEmpty(node.effectName))
                {
                    ActionParser(battleSystem, user, target, node.skillAction, skill.id, cycle);
                    SkillBuffParser(battleSystem, user, node.userBuff);
                    SkillBuffParser(battleSystem, target, node.targetBuff);
                }
                else {
                    battleSystem.LoadEffect(user,target,node.effectName,node.releaseType); 
                    ZTimerSvc.AddTask(node.effectTime , () =>
                    {
                        ActionParser(battleSystem, user, target, node.skillAction , skill.id, cycle);
                        SkillBuffParser(battleSystem, user, node.userBuff);
                        SkillBuffParser(battleSystem, target, node.targetBuff);
                    });
                }
            }
    }

    private static bool JudgeParser(BattleSystem battleSystem, ICharacter user, ICharacter target,Dictionary<SkillJudge, Value> skillJudge) {
        if (skillJudge.Count == 0) return true;
        bool flag = false;
        foreach (KeyValuePair<SkillJudge,Value> judge in skillJudge)
        {
            Value val = judge.Value;
            switch (judge.Key)
            {
                case SkillJudge.X_HEALTH_COST:
                case SkillJudge.X_HEALTH_CHECK:
                    flag = user.GetStateSystem().hp >= val;
                    if (flag == false) SetMsg("生命值不足");
                    break;
                case SkillJudge.X_MANA_COST:
                case SkillJudge.X_MANA_CHECK:
                    flag = user.GetStateSystem().mp >= val;
                    if (flag == false) SetMsg("法力不足");
                    break;
                case SkillJudge.IF_IT_IS_THE_XTH_ROUND:
                    flag = battleSystem.GetRoundNum() == val.value;
                    break;
                case SkillJudge.IF_IT_IS_NOT_THE_XTH_ROUND:
                    flag = battleSystem.GetRoundNum() != val.value;
                    break;
                default:
                    flag = true;
                    LogTool.LogError("正在进行不存在的判断：" + judge.Key);
                    break;
            }
            if (!flag) return false;
        }
        // 如果可行 开始 扣 
        foreach (KeyValuePair<SkillJudge, Value> judge in skillJudge)
        {
            Value val = judge.Value;
            val.value *= -1;
            switch (judge.Key)
            {
                case SkillJudge.X_HEALTH_COST:
                    battleSystem.ChangeHP(user, val);
                    break;
                case SkillJudge.X_MANA_COST:
                    battleSystem.ChangeMP(user, val);
                    break; 
                default: 
                    break;
            } 
        }

        return flag;
    }
    private static int CycleJudgeParser(BattleSystem battleSystem, ICharacter user, ICharacter target, Dictionary<SkillCycleJudge, Value> skillCycleJudge)
    {
        if (skillCycleJudge.Count == 0) return 1;
        // 一般来说只会进行一次类似判断倍数，多种情况下选取最小值
        Value time = new Value(int.MaxValue);
        Value minTime = new Value(int.MaxValue); 
        foreach (KeyValuePair<SkillCycleJudge,Value> cycle in skillCycleJudge)
        {
            Value val = cycle.Value;
            switch (cycle.Key)
            {
                case SkillCycleJudge.EVERY_X_PERCENT_OF_HEALTH_LOST_PCT:
                    val.valueType = ValueType.PERCENT;
                    time = (user.GetStateSystem().maxHp - user.GetStateSystem().hp) / (user.GetStateSystem().maxHp * val);
                    break;
                case SkillCycleJudge.EVERY_X_PERCENT_OF_MANA_LOST_PCT:
                    val.valueType = ValueType.PERCENT;
                    time = (user.GetStateSystem().maxMp - user.GetStateSystem().mp) / (user.GetStateSystem().maxMp * val);
                    break;
                case SkillCycleJudge.REPEAT_X_TIMES_IN_A_ROUND:
                    time = val;
                    break;
                default:
                    LogTool.LogError("正在进行不存在的循环判断" + cycle.Key);
                    break;
            }
            minTime.value = Math.Min(minTime.value,time.realVal);
        }
        return minTime.value == int.MaxValue ? 0 : minTime.value;
    }
    private static void ActionParser(BattleSystem battleSystem, ICharacter user, ICharacter target, Dictionary<SkillAction, Value> skillAction,int skillId,int cycle = 1)
    {
        foreach (KeyValuePair<SkillAction,Value> action in skillAction)
        {
            Value val = action.Value ;
            val.value *= cycle;
            Value num = new Value();
            switch (action.Key)
            {
                case SkillAction.INFLICT_X_PHYSICAL_DAMAGE:
                    battleSystem.PhysicalDamage(user,target,val,skillId:skillId);
                    break;
                case SkillAction.INFLICT_X_MAGIC_DAMAGE:
                    battleSystem.MagicDamage(user,target, val, skillId: skillId);
                    break;
                case SkillAction.TAKE_X_PHYSICAL_DAMAGE:
                    battleSystem.PhysicalDamage(null,user,val, skillId: skillId);
                    break;
                case SkillAction.TAKE_X_MAGIC_DAMAGE:
                    battleSystem.MagicDamage(null,target,val, skillId: skillId);
                    break;
                case SkillAction.ADJUST_X_HEALTH:
                case SkillAction.RESTORE_X_POINTS_OF_HEALTH:
                    battleSystem.ChangeHP(user,val);
                    break;
                case SkillAction.ADJUST_X_MANA:
                case SkillAction.RESTORE_X_POINTS_OF_MANA:
                    battleSystem.ChangeMP(user,val);
                    break; 
                case SkillAction.INFLICT_X_PERCENT_PHYSICAL_DAMAGE_PCT:
                    val.valueType = ValueType.PERCENT;
                    num = user.GetStateSystem().atk * val;
                    num.ConvertToIntValue();
                    battleSystem.PhysicalDamage(user,target, num, skillId: skillId);
                    break;
                case SkillAction.TAKE_X_PERCENT_PHYSICAL_DAMAGE_PCT:
                    val.valueType = ValueType.PERCENT;
                    num = user.GetStateSystem().atk * val;
                    num.ConvertToIntValue();
                    battleSystem.PhysicalDamage(null,user, num, skillId: skillId);
                    break;
                case SkillAction.ELIMINATE_ALL_DEBUFF:
                    battleSystem.EliminateAllDebuff(user);
                    break; 
                case SkillAction.ADJUST_X_PERCENT_OF_HEALTH_PCT:
                    val.valueType = ValueType.PERCENT;
                    num = user.GetStateSystem().maxHp * val;
                    num.ConvertToIntValue();
                    battleSystem.ChangeHP(user,num);
                    break;
                case SkillAction.ADJUST_X_PERCENT_OF_MANA_PCT:
                    val.valueType = ValueType.PERCENT;
                    num = user.GetStateSystem().maxMp * val;
                    num.ConvertToIntValue();
                    battleSystem.ChangeMP(user, num);
                    break;
                case SkillAction.GET_X_POINTS_SHIELD:
                    battleSystem.AddShield();
                    break;
                case SkillAction.INFLICT_X_PERCENT_MAGIC_DAMAGE_PCT:
                    LogTool.LogError("有问题");
                    break;
                case SkillAction.TAKE_X_PERCENT_MAGIC_DAMAGE_PCT:
                    LogTool.LogError("有问题");
                    break;
                default:
                    LogTool.LogError("正在进行不存在的动作：" + action.Key);
                    break;
            }
        } 
    }

    private static void SkillBuffParser(BattleSystem battleSystem, ICharacter target, Dictionary<int, int> buffList) {
        if (buffList.Count == 0) return;
        foreach (KeyValuePair<int,int> kvp in buffList)
        { 
            IBuff buff = ResFactory.instance.GetBuffById(kvp.Key); 
            battleSystem.AddBuff(target,buff,kvp.Value);

        }
    }
    private static void SetMsg(string m) { if (msg == "") msg = m; }
}
