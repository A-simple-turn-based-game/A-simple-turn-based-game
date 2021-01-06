using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSystem  
{
    // 装备ID 数量
    public List<int> equipment = new List<int>();
    public void AddEquipment(StateSystem stateSystem,IEquipment equip)
    {
        int equipmentId = equip.id;
        equipment.Add(equipmentId); 

        foreach (KeyValuePair<AdditionType,Value> item in equip.addition)
        {
            Value val = item.Value;
            if (item.Key.ToString().EndsWith("PCT")) val.valueType = ValueType.PERCENT;
            
            switch (item.Key)
            {
                case AdditionType.HP_MAX_PCT:
                    stateSystem.hp_max_pct *= val;
                    break;
                case AdditionType.MP_MAX_PCT:
                    stateSystem.mp_max_pct *= val;
                    break;
                case AdditionType.ATK_PCT:
                    stateSystem.atk_pct *= val;
                    break;
                case AdditionType.DEF_PCT:
                    stateSystem.def_pct *= val;
                    break;
                case AdditionType.CRIT_PCT:
                    {
                        val.valueType = ValueType.PERCENT;
                        stateSystem.crit_pct *= val;
                    }
                    break;
                case AdditionType.CRITCAL_DAMAGE_PCT:
                    {
                        val.valueType = ValueType.PERCENT;
                        stateSystem.criticalDamage_pct *= val;
                    }
                    break;
                case AdditionType.HP_MAX_OFFSET:
                    stateSystem.hp += val;
                    stateSystem.hp_max_offset += val;
                    break;
                case AdditionType.MP_MAX_OFFSET:
                    stateSystem.mp += val;
                    stateSystem.mp_max_offset += val;
                    break;
                case AdditionType.ATK_OFFSET:
                    stateSystem.atk_offset += val;
                    break;
                case AdditionType.DEF_OFFSET:
                    stateSystem.def_offset += val;
                    break;
                case AdditionType.CRIT_OFFSET:
                    stateSystem.crit_offset += val;
                    break;
                case AdditionType.CRITCAL_DAMAGE_OFFSET:
                    stateSystem.criticalDamage_offset += val;
                    break;
                default:
                    break;
            }
        }

        stateSystem.Refresh();
    }
    // 重新计算装备数据
    public void ReLoad(StateSystem stateSystem) {

        foreach (int id in equipment)
        {
            IEquipment equip = ResFactory.instance.GetEquipmentCfgById(id);
            int equipmentId = equip.id; 

            foreach (KeyValuePair<AdditionType, Value> item in equip.addition)
            {
                Value val = item.Value;
                if (item.Key.ToString().EndsWith("PCT")) val.valueType = ValueType.PERCENT;

                switch (item.Key)
                {
                    case AdditionType.HP_MAX_PCT:
                        stateSystem.hp_max_pct *= val;
                        break;
                    case AdditionType.MP_MAX_PCT:
                        stateSystem.mp_max_pct *= val;
                        break;
                    case AdditionType.ATK_PCT:
                        stateSystem.atk_pct *= val;
                        break;
                    case AdditionType.DEF_PCT:
                        stateSystem.def_pct *= val;
                        break;
                    case AdditionType.CRIT_PCT:
                        {
                            val.valueType = ValueType.PERCENT;
                            stateSystem.crit_pct *= val;
                        }
                        break;
                    case AdditionType.CRITCAL_DAMAGE_PCT:
                        {
                            val.valueType = ValueType.PERCENT;
                            stateSystem.criticalDamage_pct *= val;
                        }
                        break;
                    case AdditionType.HP_MAX_OFFSET:
                        stateSystem.hp_max_offset += val;
                        break;
                    case AdditionType.MP_MAX_OFFSET:
                        stateSystem.mp_max_offset += val;
                        break;
                    case AdditionType.ATK_OFFSET:
                        stateSystem.atk_offset += val;
                        break;
                    case AdditionType.DEF_OFFSET:
                        stateSystem.def_offset += val;
                        break;
                    case AdditionType.CRIT_OFFSET:
                        stateSystem.crit_offset += val;
                        break;
                    case AdditionType.CRITCAL_DAMAGE_OFFSET:
                        stateSystem.criticalDamage_offset += val;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public List<int> GetUseableEquipment() {
        List<int> res = new List<int>();
        foreach (int id in equipment)
        {
            if (ResFactory.instance.GetEquipmentCfgById(id).Skill != null) { res.Add(id); }
        }
        return res;
    }

}
