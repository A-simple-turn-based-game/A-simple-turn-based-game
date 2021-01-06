using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BuffRoundMode {
    //Refresh the number of continuous rounds
    ROUND_REFRESH,
    //Sustained round stacking
    ROUND_STACKING,
};

public enum BuffEffectMode
{
    //buff效果不变 unchanged
    EFFECT_UNCHANGED, 
    //effect stacking
    EFFECT_STACKING,
    // 效果累乘
    EFFECT_MUL,
};

/// <summary>
/// 状态
/// </summary>
public class IBuff 
{
    public int id;

    public string name;

    public string description; // 描述
    
    public string icon;  
     
    public bool isDebuff; // 是否是debuff

    public bool canBeDispelled;// 是否可被驱散 

    public BuffEffectMode effectMode;

    public BuffRoundMode roundMode;

    public List<BuffCfgNode> buffCfgNode = new List<BuffCfgNode>();
      
}
