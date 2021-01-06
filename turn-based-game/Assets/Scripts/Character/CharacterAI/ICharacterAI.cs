using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIType { 

    StationaryMonster,
    GlobalTraceMonster,
    LinearTrackMonster,
}

public abstract class ICharacterAI 
{
    public int actionType;
    
    public ICharacter character;
    
    // 技能管理 
    public SkillSystem skillSystem;

    // 控制管理
    public ControllerSystem controllerSystem;

    // 状态管理
    public StateSystem stateSystem;

    // 道具管理
    public PropSystem propSystem;

    // 装备管理
    public EquipmentSystem equipmentSystem;

    public void BattleAI(BattleSystem battleSystem)
    {
 
        foreach (ISkill skill in skillSystem.skills)
        {
            if (skill.passive == true) continue;
            if (battleSystem.GetSklllRecorder()[character][skill] == 0)
            {
                bool canUse = battleSystem.UseSkill(character, battleSystem.player, skill);
                if (canUse) return;
            }
        }

        battleSystem.Attack(battleSystem.monster, battleSystem.player);
    }

    public abstract void MapActionAI(MapSystem mapSystem);
}
