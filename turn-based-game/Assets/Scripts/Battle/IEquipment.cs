using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AdditionType
{
    HP_MAX_PCT,
    MP_MAX_PCT,
    ATK_PCT,
    DEF_PCT,
    CRIT_PCT,
    CRITCAL_DAMAGE_PCT,
    HP_MAX_OFFSET,
    MP_MAX_OFFSET,
    ATK_OFFSET,
    DEF_OFFSET,
    CRIT_OFFSET,
    CRITCAL_DAMAGE_OFFSET,
}
public enum RarityType { 
    SSR,
    SR,
    R, 
};
[JsonObject(MemberSerialization.OptIn)]
public class IEquipment : TShopItem
{
    [JsonProperty]
    public int id;
    [JsonProperty]
    public int cost;
    [JsonProperty]
    public string name;
    [JsonProperty]
    public string description;
    [JsonProperty]
    public string icon;
    [JsonProperty]
    public RarityType rarity;

    // 携带的技能ID
    [JsonProperty]
    public int skillId = 0;

    private ISkill _skill = null;
    public ISkill Skill {
        get {
            if (_skill == null && skillId != 0) _skill = ResFactory.instance.GetSkillById(skillId);
            return _skill;
        }
    } 

    // 基础属性增益
    public Dictionary<AdditionType, Value> addition = new Dictionary<AdditionType, Value>();

     
    public int GetCost()
    {
        return cost;
    }

    public string GetDescription()
    {
        return description;
    }

    public string GetEffectDescription()
    {
        string des = "名字：" + name;
        des += "\n费用：" + cost;
        foreach (KeyValuePair<AdditionType,Value> item in addition)
        {
            string n = item.Key.ToString();
            string[] tmp = n.Split('_');
            switch (tmp[0])
            {
                case "HP":
                    des += "\n生命：" + item.Value;
                    break;
                case "MP":
                    des += "\n法力：" + item.Value;
                    break;
                case "ATK":
                    des += "\n攻击：" + item.Value;
                    break;
                case "DEF":
                    des += "\n防御：" + item.Value;
                    break;
                case "CRIT":
                    des += "\n暴击：" + item.Value;
                    break;
                case "CRITCAL":
                    des += "\n暴伤：" + item.Value;
                    break;
                default:
                    break;
            }
        }
        if (skillId != 0) { 
            des += "\n技能：" + Skill.name;
            des += "\n效果：" + Skill.description;
        }
        return des;
    }

    public string GetIcon()
    {
        return icon;
    }

    public string GetName()
    {
        return name;
    }
}
