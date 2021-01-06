using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICharacterBuilder  
{ 
    public ICharacter character;

    public CharacterCfg cfg;

    // 加载人物模型
    public abstract void LoadModel();
    // 加载人物属性
    public abstract void LoadAttribute();
    // 加载人物技能
    public abstract void LoadSkills();
    // 加载人物道具
    public abstract void LoadProps();
    // 加载人物装备
    public abstract void LoadEquipment();
    // 加载人物控制器
    public abstract void LoadController();
    // 添加人物AI
    public abstract void AddAI();

}
