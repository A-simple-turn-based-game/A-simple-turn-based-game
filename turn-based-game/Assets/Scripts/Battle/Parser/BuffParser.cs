using System;
using System.Collections;
using System.Collections.Generic;


public class BuffCfgNode {

    public Dictionary<BuffJudge,Value> buffJudge;
    public Dictionary<BuffCycleJudge,Value> buffCycleJudge;
    public BuffAction buffAction;
    public Value actionValue;

    public BuffCfgNode() {
        this.buffJudge = new Dictionary<BuffJudge, Value>();
        this.buffCycleJudge = new Dictionary<BuffCycleJudge, Value>();
        buffAction = BuffAction.NONE;
        actionValue = new Value();
    }
};


public enum BuffJudge {

    // 立即生效，只触发一次 Effective immediately, only triggered once
    EFFECTIVE_IMMEDIATELY_ONLY_TRIGGERED_ONCE = 1001,

    // 回合开始
    BATTLE_HALFROUND_START,
    // 回合中
    BATTLE_HALFROUND_MID,
    // 回合结束
    BATTLE_HALFROUND_END, 
    //生命值大于x%时
    HEALTH_IS_GREATER_THAN_X_PCT,
    //生命值小于x%时
    HEALTH_IS_LESS_THAN_X_PCT,
    //法力值大于x%时
    MAGIC_GREATER_THAN_X_PCT,
    //法力值小于x%时
    MAGIC_LESS_THAN_X_PCT,
    //生命值小于X时
    HAELTH_IS_LESS_THAN_X,

    //--------------------------------------------------
    //受到全伤害后
    TAKING_FULL_DAMAGE,
    //受到物理伤害后
    TAKING_PHYSICAL_DAMAGE,
    //受到法术伤害后
    TAKEING_MAGIC_DAMAGE,
    //造成法术伤害后
    INFLICT_MAGIC_DAMAGE,
    //造成物理伤害后
    INFLICT_PHYSICAL_DAMAGE,
    //普通攻击后
    DURING_NORMAL_ATTACK,
    //普通攻击暴击后 During normal attack critical strike
    DURING_NORMAL_ATTACK_CRITICAL_STRIKE,
    //释放技能后 When the skill is released
    WHEN_THE_SKILL_IS_RELEASED,
    //使用x技能时
    WHEN_X_SKILL_IS_RELEASED,
    //造成法术伤害前
    INFLICT_MAGIC_DAMAGE_BEFORE,
    //造成物理伤害前
    INFLICT_PHYSICAL_DAMAGE_BEFORE,

    //法力被消耗前 Before mana is consumed
    BEFORE_MANA_IS_CONSUMED,

    AFTER_USING_SKILL_X = 1022,

    //受到全伤害前
    BEFORE_FULL_DAMAGE = 1023,
    //受到物理伤害前
    BEFORE_PHYSICAL_DAMAGE = 1024,
    //受到法术伤害前
    BEFORE_MAGIC_DAMAGE = 1025,

    //第x次造成伤害时 When the xth damage is dealt
    WHEN_THE_XTH_DAMAGE_IS_DEALT,
    //法力不足以施法时 When the mana is not enough to cast
    WHEN_THE_MANA_IS_NOT_ENOUGH_TO_CAST,
    //获得buff时 When getting buff
    WHEN_GETTING_BUFF,

};


public enum BuffCycleJudge {

    //每失去x%生命值
    EVERY_X_PERCENT_OF_HEALTH_LOST_PCT = 2001,
    //每失去x%法力值
    EVERY_X_PERCENT_OF_MANA_LOST_PCT,
};


public enum BuffAction {

    //造成的全伤害+x% Increase the total damage caused by x%
    INCREASE_THE_TOTAL_DAMAGE_CAUSED_BY_X_PCT = 3001,
    //造成的物理伤害+x%
    INCREASE_THE_PHYSICAL_DAMAGE_CAUSED_BY_X_PCT,
    //造成的魔法伤害+x%
    INCREASE_THE_MAGIC_DAMAGE_CAUSED_BY_X_PCT,
    //受到的物理伤害+x% Physical damage taken increased by x%
    PHYSICAL_DAMAGE_TAKEN_INCREASED_BY_X_PCT,
    //提高X%攻击力 Increase x-point attack
    INCREASE_X_PERCENT_ATTACK,
    //暴击率+x% Critical strike rate increased by X%
    CRITICAL_STRIKE_RATE_INCREASED_BY_X_PCT,
    //暴击伤害+x% Critical strike damage increased by X%
    CRITICAL_STRIKE_DAMAGE_INCREASED_BY_X_PCT,
    //法力消耗+x% Mana cost increased by X%
    MANA_COST_INCREASED_BY_X_PCT,

    //受到x点物理伤害
    TAKE_X_PHYSICAL_DAMAGE,
    //受到x点魔法伤害
    TAKE_X_MAGIC_DAMAGE,
    //调整 X点 HP
    ADJUST_X_POINTS_OF_HEALTH,
    //降低x点DEF Decrease x defense
    DECREASE_X_DEFENSE,

    //攻击力+x Increases attack by X
    INCREASES_ATTACK_BY_X,
    //防御力+x
    INCREASES_DEFENCE_BY_X,


    //造成的全伤害+x Increase the total damage caused by x%
    INCREASE_THE_TOTAL_DAMAGE_CAUSED_X,
    //造成的物理伤害+x
    INCREASE_THE_PHYSICAL_DAMAGE_CAUSED_X,
    //造成的魔法伤害+x
    INCREASE_THE_MAGIC_DAMAGE_CAUSED_X,

    //受到的全伤害+x Increase the total damage caused by x%
    THE_TOTAL_DAMAGE_TAKEN_INCREASE_X,
    //受到的物理伤害+x
    THE_PHYSICAL_DAMAGE_TAKEN_INCREASE_X,
    //受到的魔法伤害+x
    THE_MAGIC_DAMAG_TAKENE_INCREASE_X,

    //受到的全伤害+x% Increase the total damage caused by x%
    THE_TOTAL_DAMAGE_INCREASE_BY_X_PCT,
    //受到的魔法伤害+x%
    THE_MAGIC_DAMAGE_INCREASE_BY_X_PCT,

    //调整 X点 MP
    ADJUST_X_POINTS_OF_MANA = 3023,
    //调整 x% HP 
    ADJUST_X_OF_HEALTH_PCT = 3024,
    //调整 x% MP 
    ADJUST_X_OF_MANA_PCT = 3025,

    //无法行动 Unable to move
    UNABLE_TO_MOVE,
    //无法使用技能 Cannot use skills
    CANNOT_USE_SKILLS,
    // 无法进行普通攻击 
    CANNOT_USE_ATTACK,

    //无敌 Invincible
    INVINCIBLE,
    
    //可抵挡x点伤害 Can withstand x damage
    CAN_WITHSTAND_X_DAMAGE,

    //随机失去技能 Randomly lose x skills
    RANDOMLY_LOSE_X_SKILLS,

    NONE,
};

public class BuffParser
{
    public static void Parser(BattleSystem battleSystem, ICharacter target, IBuff buff , BattleBuffCalInfo judgeInfo ,int cnt)
    {
        bool cu = false;
        foreach (BuffCfgNode buffCfg in buff.buffCfgNode)
        { 
            bool canUse = BuffJudgeParser(battleSystem,target,buffCfg.buffJudge, judgeInfo);
            if (canUse == false) continue;
            cu = true;
            int cycle = BuffCycleJudgeParser(battleSystem,target, buffCfg.buffCycleJudge) ; 
            BuffActionParser(battleSystem,target,buff,buffCfg,judgeInfo,cnt,cycle);
        }
        if (cu) EventCenter.Broadcast<string>(EventType.BATTLEINFO,buff.name + " 生效 ");
         
    }

    public static Value Cancel(BattleSystem battleSystem, ICharacter target, IBuff buff, Value value) {

         
        foreach (BuffCfgNode buffCfg in buff.buffCfgNode)
        { 
            switch (buffCfg.buffAction)
        {
            case BuffAction.UNABLE_TO_MOVE:
                battleSystem.CancelDizziness(target);
                break;
            case BuffAction.CANNOT_USE_SKILLS:
                battleSystem.EnableSkill(target);
                break;
            case BuffAction.CANNOT_USE_ATTACK:
                battleSystem.EnableAttack(target);
                break;
            case BuffAction.INVINCIBLE:
                LogTool.Log("不明确");
                break;
            case BuffAction.CRITICAL_STRIKE_DAMAGE_INCREASED_BY_X_PCT:
                target.GetStateSystem().criticalDamage_pct -= value;
                break;
            case BuffAction.CRITICAL_STRIKE_RATE_INCREASED_BY_X_PCT:
                target.GetStateSystem().crit_pct -= value;
                break;    
            case BuffAction.DECREASE_X_DEFENSE:
                target.GetStateSystem().def_offset -= value;
                break;
            case BuffAction.INCREASES_ATTACK_BY_X:
                target.GetStateSystem().atk_offset -= value;
                break;
            case BuffAction.INCREASES_DEFENCE_BY_X:
                target.GetStateSystem().def_offset -= value;
                break;  
            case BuffAction.RANDOMLY_LOSE_X_SKILLS:
                LogTool.Log("不明确");
                break;
            default:
                break;
        }
        }
    
        return new Value();
    }

    private static bool BuffJudgeParser(BattleSystem battleSystem, ICharacter target, Dictionary<BuffJudge,Value> buffJudge, BattleBuffCalInfo judgeInfo) {

        if (buffJudge == null) return true;
        bool flag = false;
        foreach (KeyValuePair<BuffJudge,Value> judge in buffJudge)
        {
            Value val = judge.Value;
            switch (judge.Key)
            {
                case BuffJudge.BATTLE_HALFROUND_START:
                    flag = battleSystem.battleProgress == BattleProgress.HALFSTART;
                    break;
                case BuffJudge.BATTLE_HALFROUND_MID:
                    flag = battleSystem.battleProgress == BattleProgress.HALFMID;
                    break;
                case BuffJudge.BATTLE_HALFROUND_END:
                    flag = battleSystem.battleProgress == BattleProgress.HALFEND;
                    break;
                case BuffJudge.HEALTH_IS_GREATER_THAN_X_PCT:
                    flag = target.GetStateSystem().hp > target.GetStateSystem().maxHp * val;
                    break;
                case BuffJudge.HEALTH_IS_LESS_THAN_X_PCT:
                    flag = target.GetStateSystem().hp < target.GetStateSystem().maxHp * val;
                    break;
                case BuffJudge.MAGIC_GREATER_THAN_X_PCT:
                    flag = target.GetStateSystem().mp > target.GetStateSystem().maxMp * val;
                    break;
                case BuffJudge.MAGIC_LESS_THAN_X_PCT:
                    flag = target.GetStateSystem().mp < target.GetStateSystem().maxMp * val;
                    break;
                case BuffJudge.HAELTH_IS_LESS_THAN_X:
                    flag = target.GetStateSystem().hp < val;
                    break;
                case BuffJudge.WHEN_X_SKILL_IS_RELEASED:
                    flag = judgeInfo != null ? judgeInfo.skillId == val.value : false;
                    break;
                case BuffJudge.AFTER_USING_SKILL_X:
                    if (judgeInfo != null && judgeInfo.judgeSet != null && judgeInfo.judgeSet.Contains(judge.Key))
                        flag = judgeInfo.skillId == val.value;
                    else flag = false;
                    break;
                default:
                    if (judgeInfo != null && judgeInfo.judgeSet != null) flag = judgeInfo.judgeSet.Contains(judge.Key);
                    else flag = false;
                    break;
            }
            if (flag == false) return false;
        }
        return flag;
    }

    private static int BuffCycleJudgeParser(BattleSystem battleSystem, ICharacter target, Dictionary<BuffCycleJudge, Value> buffCycleJudge)
    {
        if (buffCycleJudge == null || buffCycleJudge.Count == 0) return 1;
        // 一般来说只会进行一次类似判断倍数，多种情况下选取最小值
        Value time = new Value(int.MaxValue);
        Value minTime = new Value(int.MaxValue);
        foreach (KeyValuePair<BuffCycleJudge, Value> cycle in buffCycleJudge)
        {
            Value val = cycle.Value;
            switch (cycle.Key)
            {
                case BuffCycleJudge.EVERY_X_PERCENT_OF_HEALTH_LOST_PCT:
                    val.valueType = ValueType.PERCENT;
                    time = (target.GetStateSystem().maxHp - target.GetStateSystem().hp) / (target.GetStateSystem().maxHp * val);
                    break;
                case BuffCycleJudge.EVERY_X_PERCENT_OF_MANA_LOST_PCT:
                    val.valueType = ValueType.PERCENT;
                    time = (target.GetStateSystem().maxMp - target.GetStateSystem().mp) / (target.GetStateSystem().maxMp * val);
                    break; 
                default:
                    break;
            }
            minTime.value = Math.Min(minTime.value, time.realVal);
        }
        return minTime.value == int.MaxValue ? 0 : minTime.value;
    }
    private static void BuffActionParser(BattleSystem battleSystem, ICharacter target,IBuff buff, BuffCfgNode buffCfg
        , BattleBuffCalInfo buffCalInfo, int cnt, int cycle = 1)
    { 
        BuffAction action = buffCfg.buffAction;
        
        Value actionValue = buffCfg.actionValue;
        actionValue.value *= cycle;

        if (buff.effectMode == BuffEffectMode.EFFECT_STACKING) { actionValue.value *= cnt; }
        else if (buff.effectMode == BuffEffectMode.EFFECT_MUL) {
            if (action.ToString().EndsWith("PCT"))
            {
                actionValue.valueType = ValueType.PERCENT;
                actionValue.value = (int)(Math.Pow(actionValue.floatVal,cnt) * 100);
            }
            else {
                actionValue.value = (int)Math.Pow(actionValue.realVal, cnt);
            }
        }
        switch (action)
        {
            case BuffAction.UNABLE_TO_MOVE:
                battleSystem.Dizziness(target);
                break;
            case BuffAction.CANNOT_USE_SKILLS:
                battleSystem.DisableSkill(target);
                break;
            case BuffAction.CANNOT_USE_ATTACK:
                battleSystem.DisableAttack(target);
                break;
            case BuffAction.INVINCIBLE:
                LogTool.Log("不明确");
                break;

      
            case BuffAction.MANA_COST_INCREASED_BY_X_PCT: 
            case BuffAction.INCREASE_THE_TOTAL_DAMAGE_CAUSED_BY_X_PCT: 
            case BuffAction.INCREASE_THE_MAGIC_DAMAGE_CAUSED_BY_X_PCT: 
            case BuffAction.INCREASE_THE_PHYSICAL_DAMAGE_CAUSED_BY_X_PCT: 
            case BuffAction.PHYSICAL_DAMAGE_TAKEN_INCREASED_BY_X_PCT:
            case BuffAction.THE_TOTAL_DAMAGE_INCREASE_BY_X_PCT:
            case BuffAction.THE_MAGIC_DAMAGE_INCREASE_BY_X_PCT:
                actionValue.valueType = ValueType.PERCENT;
                buffCalInfo.pct *= actionValue; 
                LogTool.Log(">>>>>>>>>>>" + actionValue);
                break;
            
            case BuffAction.INCREASE_THE_TOTAL_DAMAGE_CAUSED_X:
            case BuffAction.INCREASE_THE_PHYSICAL_DAMAGE_CAUSED_X:
            case BuffAction.INCREASE_THE_MAGIC_DAMAGE_CAUSED_X:
            case BuffAction.THE_TOTAL_DAMAGE_TAKEN_INCREASE_X:
            case BuffAction.THE_PHYSICAL_DAMAGE_TAKEN_INCREASE_X:
            case BuffAction.THE_MAGIC_DAMAG_TAKENE_INCREASE_X:
                actionValue.valueType = ValueType.INT;
                buffCalInfo.offset += actionValue;
                break;
            case BuffAction.TAKE_X_PHYSICAL_DAMAGE:
                battleSystem.PhysicalDamage(null,target, actionValue);
                break;
            case BuffAction.TAKE_X_MAGIC_DAMAGE:
                battleSystem.MagicDamage(null,target, actionValue);
                break;
            case BuffAction.ADJUST_X_POINTS_OF_HEALTH:
                battleSystem.ChangeHP(target, actionValue);
                break;
            case BuffAction.ADJUST_X_POINTS_OF_MANA:
                battleSystem.ChangeMP(target, actionValue);
                break;
            case BuffAction.ADJUST_X_OF_MANA_PCT:
                actionValue.valueType = ValueType.PERCENT;
                battleSystem.ChangeMPPCT(target, actionValue);
                break;
            case BuffAction.ADJUST_X_OF_HEALTH_PCT:
                actionValue.valueType = ValueType.PERCENT; 
                battleSystem.ChangeHPPCT(target, actionValue);
                break;
            case BuffAction.CRITICAL_STRIKE_DAMAGE_INCREASED_BY_X_PCT:
                battleSystem.ChangeCriticalDamage(target,buff,actionValue );
                break;
            case BuffAction.CRITICAL_STRIKE_RATE_INCREASED_BY_X_PCT:
                battleSystem.ChangeCrit(target,buff, actionValue );
                break; 
            case BuffAction.DECREASE_X_DEFENSE:
                battleSystem.ChangeDefense(target, buff, actionValue );
                break;
            case BuffAction.INCREASES_DEFENCE_BY_X:
                battleSystem.ChangeDefense(target, buff, actionValue );
                break;
            case BuffAction.INCREASE_X_PERCENT_ATTACK:
                battleSystem.ChangeAttackPCT(target, buff, actionValue );
                break;
            case BuffAction.INCREASES_ATTACK_BY_X:
                battleSystem.ChangeAttack(target, buff, actionValue);
                break;
            case BuffAction.CAN_WITHSTAND_X_DAMAGE:
                LogTool.Log("不明确");
                break;
            case BuffAction.RANDOMLY_LOSE_X_SKILLS:
                LogTool.Log("不明确");
                break;
            default:
                break;
        } 
    }
}
