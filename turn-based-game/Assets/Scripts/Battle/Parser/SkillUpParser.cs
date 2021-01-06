using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 
基础物理伤害+x
基础物理伤害+x%
基础魔法伤害+x
基础魔法伤害+x%
MP消耗+x
MP消耗+x%
冷却时间+x
冷却时间+x%
 
Base physical damage increased by X

Basic physical damage increased by X %

Base magic damage increased by X

Base magic damage increased by X %

MP consumption increases x

MP consumption increases by X %

Cooldown increased by x

Cooldown time increased by X %

 */
public class SkillUpNode {
    public int id;
    public int skillId;
    public string name;
    public string description;

    public List<PropertyChangeType> propertyChanges = new List<PropertyChangeType>();

    public List<List<Value>> changeValue = new List<List<Value>>();

    public List<SkillCfgNode> cfgUpNode = new List<SkillCfgNode>();

    public SkillUpNode(){}
};
 
public enum PropertyChangeType {
    //编号x的SkillAction基础数值调整y
    BASE_X_SKILLACTION_INCREASED_BY_Y = 1001,
    //编号x的SkillAction基础数值调整y%
    BASE_X_SKILLACTION_INCREASED_BY_Y_PCT = 1002,
    //编号x的SkillJUDGE基础数值调整y
    BASE_X_SKILLJUDGE_BY_Y = 1003,
    //编号x的SkillJUDGE基础数值调整y%
    BASE_X_SKILLJUDGE_BY_Y_PCT = 1004,  
    //冷却时间+x
    CD_INCREASED_BY_X = 1005,
    //冷却时间+x%
    CD_TIME_INCREASED_BY_X_PCT = 1006,
    //添加编号为X的SkillAction 到主技能
    ADD_BASE_X_SKILLACTION_Y_VALUE = 1007,
    //添加编号为X的SkillJudge 到主技能
    ADD_BASE_X_SKILLJUDGE_Y_VALUE = 1008,
    //添加编号为X的SkillCycleJudge 到主技能
    ADD_BASE_X_SKILLCYCLEJUDGE_Y_VALUE = 1009,
    //添加编号为X的CycleType 到主技能
    ADD_BASE_X_CYCLETYPE = 1010,
}

public class SkillUpParser
{ 
    public static void Parser(ISkill skill, SkillUpNode skillUpNode)
    {
        List<List<Value>> changeValue = skillUpNode.changeValue;
        for (int i = 0; i < skillUpNode.propertyChanges.Count; ++i)
        {
            Value val = changeValue[i][0];
            switch (skillUpNode.propertyChanges[i])
            {
                case PropertyChangeType.BASE_X_SKILLACTION_INCREASED_BY_Y:
                    {
                        SkillAction skillAction = (SkillAction)val.realVal;
                        Value val1 = changeValue[i][1];
                        foreach (SkillCfgNode cfgNode in skill.skillCfgNodes)
                        {
                            if (cfgNode.skillAction.ContainsKey(skillAction))
                            {
                                cfgNode.skillAction[skillAction] += val1;
                            }
                        }
                    }
                    break;
                case PropertyChangeType.BASE_X_SKILLACTION_INCREASED_BY_Y_PCT:
                    {
                        SkillAction skillAction_pct = (SkillAction)val.realVal;
                        Value val1 = changeValue[i][1];
                        val1.valueType = ValueType.PERCENT;
                        foreach (SkillCfgNode cfgNode in skill.skillCfgNodes)
                        {
                            if (cfgNode.skillAction.ContainsKey(skillAction_pct))
                            {
                                Value tmp = cfgNode.skillAction[skillAction_pct] * val1;
                                if (!skillAction_pct.ToString().EndsWith("PCT"))
                                {
                                    tmp.ConvertToIntValue();
                                }
                                cfgNode.skillAction[skillAction_pct] = tmp;
                            }
                        }
                    }
                    break;
                case PropertyChangeType.BASE_X_SKILLJUDGE_BY_Y:
                    {
                        SkillJudge skillJudge = (SkillJudge)val.realVal;
                        Value val1 = changeValue[i][1];
                        foreach (SkillCfgNode cfgNode in skill.skillCfgNodes)
                        {
                            if (cfgNode.skillJudge.ContainsKey(skillJudge))
                            {
                                cfgNode.skillJudge[skillJudge] += val1;
                            }
                        }
                    }
                    break;
                case PropertyChangeType.BASE_X_SKILLJUDGE_BY_Y_PCT:
                    {
                        SkillJudge skillJudge_pct = (SkillJudge)val.realVal;
                        Value val1 = changeValue[i][1];
                        val1.valueType = ValueType.PERCENT;
                        foreach (SkillCfgNode cfgNode in skill.skillCfgNodes)
                        {
                            if (cfgNode.skillJudge.ContainsKey(skillJudge_pct))
                            {
                                Value tmp = cfgNode.skillJudge[skillJudge_pct] * val1;
                                if (!skillJudge_pct.ToString().EndsWith("PCT"))
                                {
                                    tmp.ConvertToIntValue();
                                }
                                cfgNode.skillJudge[skillJudge_pct] = tmp;
                            }
                        }
                    }
                    break;
                case PropertyChangeType.CD_INCREASED_BY_X:
                    skill.cd += val.realVal;
                    skill.cd = Mathf.Max(skill.cd,0);
                    break;
                case PropertyChangeType.CD_TIME_INCREASED_BY_X_PCT:
                    val.valueType = ValueType.PERCENT;
                    skill.cd = skill.cd * val.realVal;
                    skill.cd = Mathf.Max(skill.cd, 0);
                    break;
                case PropertyChangeType.ADD_BASE_X_SKILLACTION_Y_VALUE:
                    {
                        SkillCfgNode node = skill.skillCfgNodes[0];
                        SkillAction skillAction = (SkillAction)changeValue[i][0].realVal;
                        node.skillAction.Add(skillAction, changeValue[i][1]);
                    }
                    break;
                case PropertyChangeType.ADD_BASE_X_SKILLJUDGE_Y_VALUE:
                    {
                        SkillCfgNode node = skill.skillCfgNodes[0];
                        SkillJudge skillJudge = (SkillJudge)changeValue[i][0].realVal;
                        node.skillJudge.Add(skillJudge, changeValue[i][1]);
                    }
                    break;
                case PropertyChangeType.ADD_BASE_X_SKILLCYCLEJUDGE_Y_VALUE:
                    {
                        SkillCfgNode node = skill.skillCfgNodes[0];
                        SkillCycleJudge skillCycleJudge = (SkillCycleJudge)changeValue[i][0].realVal;
                        node.skillCycleJudge.Add(skillCycleJudge, changeValue[i][1]);
                    }
                    break;
                case PropertyChangeType.ADD_BASE_X_CYCLETYPE:
                    {
                        SkillCfgNode node = skill.skillCfgNodes[0];
                        CycleType skillCycleType = (CycleType)changeValue[i][0].realVal;
                        node.cycleType = skillCycleType;
                    }
                    break;
                default:
                    break;
            }
        }

        // 添加新的技能效果
        int cnt = skillUpNode.cfgUpNode.Count;
        //int scnt = skill.skillCfgNodes.Count;
        for (int i = 0; i < cnt; i++)
        {
            skill.skillCfgNodes.Add(skillUpNode.cfgUpNode[i]); 
            //if (i < scnt)
            //{
            //    AddRangeDict<SkillJudge,Value>(skill.skillCfgNodes[i].skillJudge, skillUpNode.cfgUpNode[i].skillJudge);
            //    AddRangeDict<SkillCycleJudge, Value>(skill.skillCfgNodes[i].skillCycleJudge, skillUpNode.cfgUpNode[i].skillCycleJudge);
            //    skill.skillCfgNodes[i].cycleType = skillUpNode.cfgUpNode[i].cycleType;
            //    AddRangeDict<SkillAction, Value>(skill.skillCfgNodes[i].skillAction, skillUpNode.cfgUpNode[i].skillAction);
            //    AddRangeDict<int, int>(skill.skillCfgNodes[i].userBuff, skillUpNode.cfgUpNode[i].userBuff);
            //    AddRangeDict<int, int>(skill.skillCfgNodes[i].targetBuff, skillUpNode.cfgUpNode[i].targetBuff); 

            //}
            //else { 
            //} 
        }
        skill.description += "\n●" + skillUpNode.description;
    }

    private static void AddRangeDict<T,K>(Dictionary<T,K> source, Dictionary<T, K> target) {

        if (target == null || target.Count == 0 ) { return; }
        
        foreach (KeyValuePair<T,K> item in target)
        {
            source.Add(item.Key,item.Value);    
        }
    }
}