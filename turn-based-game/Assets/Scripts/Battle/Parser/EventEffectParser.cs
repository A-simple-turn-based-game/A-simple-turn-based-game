using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType {

    // 调整x点生命值
    ADJUST_THE_HEALTH_OF_POINT_X = 1001,
    // 调整x点法力值
    ADJUST_THE_MANA_OF_POINT_X = 1002,
    // 调整 x% 生命值
    ADJUST_THE_HEALTH_X_PCT = 1003,
    // 调整 x% 法力值
    ADJUST_THE_MANA_X_PCT = 1004,
    // 调整 x 金币
    ADJUST_X_GOLD_COIN = 1005,
    // 调整 x% 最大生命值
    ADJUST_THE_MAX_HEALTH_X_PCT = 1006,
    // 调整 x% 最大法力值
    ADJUST_THE_MAX_MANA_X_PCT = 1007,
    // 传送到地图编号x
    TRANSFER_TO_MAP_NUMBER_X = 1008,
    // 传送到下个地图集的随机一张
    TRANSFER_TO_NEXT_RANDOM_NUMBER_X = 1009,
    // 在触发地点附近生成事件X
    GENERATE_EVENTS_X_NEARBY = 1010,
    // 显示商店
    SHOW_SHOP = 1011,
};

public class EventEffectParser  
{ 
    public static void Parser(GamePanel gamePanel,ICharacter character,List<Value> effects) {

        if (effects == null || effects.Count == 0) return;
        int type = effects[0].realVal;
        StateSystem stateSystem = character.GetStateSystem();

        Value val;

        EffectType effectType = (EffectType)type;
        switch (effectType)
        {
            case EffectType.ADJUST_THE_HEALTH_OF_POINT_X:
                stateSystem.hp += effects[1];
                // TODO 目前触发仅仅只有玩家
                if (stateSystem.hp.realVal <= 0) EventCenter.Broadcast<ICharacter>(EventType.PLAYERDIE, character);
                stateSystem.hp.value = Mathf.Max(stateSystem.hp.value,stateSystem.maxHp.value);
                break;
            case EffectType.ADJUST_THE_MANA_OF_POINT_X:
                stateSystem.mp += effects[1];
                stateSystem.mp.value = Mathf.Clamp(stateSystem.mp.value, 0,stateSystem.maxMp.value);
                break;
            case EffectType.ADJUST_THE_HEALTH_X_PCT:
                val = effects[1];
                val.valueType = ValueType.PERCENT;
                stateSystem.hp *= val;
                stateSystem.hp.ConvertToIntValue();
                stateSystem.hp.value = Mathf.Clamp(stateSystem.hp.realVal, 1, stateSystem.maxHp.realVal);
                break;
            case EffectType.ADJUST_THE_MANA_X_PCT:
                val = effects[1];
                val.valueType = ValueType.PERCENT;
                stateSystem.mp *= val;
                stateSystem.mp.ConvertToIntValue();
                stateSystem.mp.value = Mathf.Clamp(stateSystem.mp.realVal, 0, stateSystem.maxMp.realVal);
                break;
            case EffectType.ADJUST_X_GOLD_COIN:
                Player player = (Player)character; 
                player.ChangeGold(effects[1].realVal); 
                break;
            case EffectType.ADJUST_THE_MAX_HEALTH_X_PCT:
                val = effects[1];
                val.valueType = ValueType.PERCENT;
                stateSystem.hp += stateSystem.maxHp * val;
                stateSystem.hp.ConvertToIntValue();
                stateSystem.hp.value = Mathf.Clamp(stateSystem.hp.realVal, 0, stateSystem.maxHp.realVal);
                break;
            case EffectType.ADJUST_THE_MAX_MANA_X_PCT:
                val = effects[1];
                val.valueType = ValueType.PERCENT;
                stateSystem.mp += stateSystem.maxMp * val;
                stateSystem.mp.ConvertToIntValue();
                stateSystem.mp.value = Mathf.Clamp(stateSystem.mp.realVal, 0, stateSystem.maxMp.realVal);
                break;
            case EffectType.TRANSFER_TO_MAP_NUMBER_X:
                val = effects[1];
                gamePanel.GetMapSystem().GoToMap(val.realVal);
                break;
            case EffectType.TRANSFER_TO_NEXT_RANDOM_NUMBER_X:
                gamePanel.GetMapSystem().GoToNextMapRandomly();
                break;
            case EffectType.GENERATE_EVENTS_X_NEARBY:
                val = effects[1];
                gamePanel.GetMapSystem().GenerateAnEventNearTheCharacter(val.realVal,character);
                break;
            case EffectType.SHOW_SHOP:
                gamePanel.ShowShop();
                break;
            default:
                break;
        }
        if(character is Player)
            gamePanel.PlayerStateUpdate(character.GetStateSystem());
    }
}
