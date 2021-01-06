using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType { 

    PLAYER = 0, 
    OTHERS,
};
 
public class ISkill  
{
    public int id;

    public SkillType skillType;

    public string name;
    
    public string description;

    public string icon = null; 

    public bool passive; // 是否为被动技能
    
    public int cd;
     
    public List<SkillCfgNode> skillCfgNodes = new List<SkillCfgNode>();

    public ISkill() { }
      
}
