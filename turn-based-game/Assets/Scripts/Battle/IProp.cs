using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[JsonObject(MemberSerialization.OptIn)]
public class IProp : TShopItem
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
    public string icon = null;
    // 携带的技能ID
    [JsonProperty]
    public int skillId = 0;
    [JsonProperty]
    public RarityType rarity;

    private ISkill _skill = null;
    public ISkill Skill
    {
        get
        {
            if (_skill == null && skillId != 0) _skill = ResFactory.instance.GetSkillById(skillId);
            return _skill;
        }
    }

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
        des += "费用：" + cost;
        des += "效果：" + ResFactory.instance.GetSkillById(skillId).description;
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
