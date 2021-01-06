using Newtonsoft.Json;

/// <summary>
/// 人物状态
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public class StateSystem  
{

    private ICharacter m_Character;

    public Value maxHp = new Value(ValueType.INT);
    public Value maxMp = new Value(ValueType.INT);
    [JsonProperty]
    public Value hp = new Value(ValueType.INT);//HP，血量，战斗双方中一方血量为0时战斗结束。
    [JsonProperty]
    public Value mp = new Value(ValueType.INT);//MP，蓝量，可用于使用技能。
    public Value atk = new Value(ValueType.INT);// ATK，攻击，普通攻击时造成的伤害。
    public Value def = new Value(ValueType.INT);//DEF，防御，受到伤害时产生减伤效果。
    public Value crit = new Value(ValueType.PERCENT); //Crit 暴击率
    public Value criticalDamage = new Value(ValueType.PERCENT); // 暴击伤害

    // 基础属性
    [JsonProperty]
    public CharacterCfg baseCfg;

    // 百分比属性加成 
    public Value hp_max_pct = new Value(100,valueType:ValueType.PERCENT);
    public Value mp_max_pct = new Value(100, valueType: ValueType.PERCENT);
    public Value atk_pct = new Value(100, valueType: ValueType.PERCENT);
    public Value def_pct = new Value(100, valueType: ValueType.PERCENT);
    public Value crit_pct = new Value(100, valueType: ValueType.PERCENT);
    public Value criticalDamage_pct = new Value(100, valueType: ValueType.PERCENT);

    // 数值属性加成
    public Value hp_max_offset = new Value(0);
    public Value mp_max_offset = new Value(0);
    public Value atk_offset = new Value(0);
    public Value def_offset = new Value(0);
    public Value crit_offset = new Value(0, valueType: ValueType.PERCENT);
    public Value criticalDamage_offset = new Value(0, valueType: ValueType.PERCENT);


    public StateSystem(ICharacter character) { this.m_Character = character; }

    // 恢复原本属性
    public void Init() {
        // TODO 考虑血量是否继承
        Refresh();
        hp = maxHp;
        mp = maxMp;
    }
     
    public void Clear() {
        hp_max_pct = new Value(100, valueType: ValueType.PERCENT);
        mp_max_pct = new Value(100, valueType: ValueType.PERCENT);
        atk_pct = new Value(100, valueType: ValueType.PERCENT);
        def_pct = new Value(100, valueType: ValueType.PERCENT);
        crit_pct = new Value(100, valueType: ValueType.PERCENT);
        criticalDamage_pct = new Value(100, valueType: ValueType.PERCENT);

        // 数值属性加成
        hp_max_offset = new Value(0);
        mp_max_offset = new Value(0);
        atk_offset = new Value(0);
        def_offset = new Value(0);
        crit_offset = new Value(0, valueType: ValueType.PERCENT);
        criticalDamage_offset = new Value(0, valueType: ValueType.PERCENT);
         
    }

    public void Refresh()
    {
        Value tmp = (baseCfg.hp + hp_max_offset) * hp_max_pct;
        maxHp.value = tmp.realVal;
        tmp = (baseCfg.mp + mp_max_offset) * mp_max_pct;
        maxMp.value = tmp.realVal;
        tmp = (baseCfg.atk + atk_offset) * atk_pct;
        atk.value = tmp.realVal;
        tmp = (baseCfg.def + def_offset) * def_pct;
        def.value = tmp.realVal;
        crit = (baseCfg.crit + crit_offset) * crit_pct;
        criticalDamage = (baseCfg.criticalDamage + criticalDamage_offset) * criticalDamage_pct;

        m_Character.OnStateChanged?.Invoke(this);
    }


    public Value GetDef() {
        Value tmp = (baseCfg.def + def_offset) * def_pct;
        def.value = tmp.realVal;
        return def;
    }

}
