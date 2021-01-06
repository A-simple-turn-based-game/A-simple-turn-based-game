using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class SkillSystem  
{
    [JsonProperty]
    public List<ISkill> skills = new List<ISkill>();

    [JsonProperty]
    public Dictionary<int,List<SkillUpNode>> skillUp = new Dictionary<int, List<SkillUpNode>>();
     

   
}
