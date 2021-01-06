using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;


public enum ValueType { 
    INT,        // 数值
    PERCENT,    // 百分比
};
public enum ValueMeaningType { 
    // 无特殊含义
    NONE,
    // 物理伤害
    PHYSICAL,
    // 魔法伤害
    MAGIC,
    // 暴击伤害
    CRITICAL_DAMAGE,  
    // 加血
    HEALING,
    // 加蓝
    MANA,
};
[JsonObject(MemberSerialization.OptIn)]
public struct Value
{
    [JsonProperty]
    public int value;
    [JsonProperty]
    public ValueType valueType;
    [JsonProperty]
    public ValueMeaningType meaningType;

    public int realVal {
        get {
            if (valueType == ValueType.PERCENT) { return (int)value / 100; }
            return value;
        }
    }
    public int percentVal {
        get {
            if (valueType == ValueType.INT) { return value * 100; }
            return value;
        }
    }
    public float floatVal {
        get {
            if (valueType == ValueType.PERCENT) { return (float)value / 100; }
            return value;
        }
    }
    public Value(int val,ValueMeaningType meaningType = ValueMeaningType.NONE,ValueType valueType = ValueType.INT)
    {
        this.value = val;
        this.meaningType = meaningType;
        this.valueType = valueType;
    }
 
    public Value(ValueType valueType )
    {
        this.value = 0;
        this.meaningType = ValueMeaningType.NONE;
        this.valueType = valueType;
    }

    public void ConvertToIntValue() {
        this.value = realVal;
        this.valueType = ValueType.INT;
    }

    #region 重载
    public static bool operator ==(Value a, Value b)
    {
        if (a.valueType != b.valueType) return a.realVal == b.realVal;
        return a.value == b.value;
    }
    public static bool operator !=(Value a, Value b)
    {
        if (a.valueType != b.valueType) return a.realVal == b.realVal;
        return a.value != b.value;
    } 
    public static bool operator >(Value a,Value b) {
        if (a.valueType != b.valueType) return a.realVal > b.realVal;
        return a.value > b.value;
    }
    public static bool operator <(Value a, Value b){
        if (a.valueType != b.valueType) return a.realVal < b.realVal;
        return a.value < b.value;
    }
    public static bool operator >=(Value a, Value b)
    {
        if (a.valueType != b.valueType) return a.realVal >= b.realVal;
        return a.value >= b.value;
    }
    public static bool operator <=(Value a, Value b)
    {
        if (a.valueType != b.valueType) return a.realVal <= b.realVal;
        return a.value <= b.value;
    }
    public static Value operator +(Value a, Value b)
    {
        if (a.valueType != b.valueType)
        {
            return new Value(a.percentVal + b.percentVal,a.meaningType,valueType:ValueType.PERCENT);
        }
        return new Value(a.value + b.value,a.meaningType,valueType:a.valueType );
    } 
    public static Value operator -(Value a, Value b)
    {
        if (a.valueType != b.valueType)
        {
            return new Value(a.percentVal - b.percentVal, a.meaningType, valueType: ValueType.PERCENT);
        }
        return new Value(a.value - b.value, a.meaningType, valueType: a.valueType);
    }
    public static Value operator *(Value a, Value b)
    {
        if (a.valueType == b.valueType && a.valueType == ValueType.INT)
        {
            return new Value(a.value * b.value, a.meaningType, valueType: ValueType.INT);
        } 
        return new Value(a.percentVal * b.percentVal / 100, a.meaningType, valueType: ValueType.PERCENT);
    }
    public static Value operator /(Value a, Value b)
    { 
        return new Value(a.percentVal * 100 / b.percentVal , a.meaningType, valueType: ValueType.PERCENT);
    }
    public override bool Equals(object obj)
    {
        return obj is Value value &&
               this.value == value.value &&
               valueType == value.valueType &&
               meaningType == value.meaningType;
    }

    public override int GetHashCode()
    {
        int hashCode = 1810992816;
        hashCode = hashCode * -1521134295 + value.GetHashCode();
        hashCode = hashCode * -1521134295 + valueType.GetHashCode();
        hashCode = hashCode * -1521134295 + meaningType.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        if (valueType == ValueType.INT)
        {
            return "" + value;
        }
        else { 
        
            return "" + value + "%" ;
        }
    }
    #endregion
}
