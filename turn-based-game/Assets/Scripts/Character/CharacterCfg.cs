using System.Collections;
using System.Collections.Generic;


public enum MonsterType { 
    Normal,
    Boss
};

public class CharacterCfg 
{
    public int id;
    public string icon;
    public int gold;
    public MonsterType monsterType;
    public AIType aiType;
    public string name;
    public string description;
    public string model;
    public List<int> exp; 
    public List<int> skills;
    public List<List<Value>> deadEventEffects;
    public Dictionary<CharacterState, float> stateDelay;
    public float atkDamage_time;
    public Value hp;//HP，血量，战斗双方中一方血量为0时战斗结束。
    public Value mp;//MP，蓝量，可用于使用技能。
    public Value atk;// ATK，攻击，普通攻击时造成的伤害。
    public Value def;//DEF，防御，受到伤害时产生减伤效果。
    public Value crit; //Crit 暴击率  
    public Value criticalDamage; // 暴击伤害
}
