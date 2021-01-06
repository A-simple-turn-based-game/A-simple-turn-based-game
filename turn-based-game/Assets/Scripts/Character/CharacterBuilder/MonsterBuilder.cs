using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBuilder : ICharacterBuilder
{
    public override void AddAI()
    {
        AIType type = cfg.aiType;
 
        switch (type)
        {
            case AIType.StationaryMonster:
                character.SetAI(new StationaryMonsterAI());
                break;
            case AIType.GlobalTraceMonster:
                character.SetAI(new GlobalTraceMonsterAI());
                break;
            case AIType.LinearTrackMonster:
                character.SetAI(new LinearTrackMonsterAI());
                break;
            default:
                break;
        }
    }
    public override void LoadController()
    {
        ControllerSystem controllerSystem = character.gameObject.AddComponent<ControllerSystem>();
        controllerSystem.RegisterCharacter(character);
        controllerSystem.stateDelay = cfg.stateDelay;
        controllerSystem.atkDamageTime = cfg.atkDamage_time;
        character.RegisterControllerSystem(controllerSystem);
    }
    public override void LoadAttribute()
    {
        character.name = cfg.name; 
        character.description = cfg.description;
        character.gold = cfg.gold;
        character.exps = cfg.exp;
        character.deadEventEffect = cfg.deadEventEffects;
        StateSystem stateSystem = new StateSystem(character);

        //CharacterCfg characterCfg = new CharacterCfg();

        //characterCfg.hp.value = 100; 
        //characterCfg.atk.value = 15;
        //characterCfg.def.value = 5;
        //characterCfg.crit = new Value(0, valueType: ValueType.PERCENT);
        //characterCfg.criticalDamage = new Value(100, valueType: ValueType.PERCENT); ;

        // TODO 默认怪物蓝无限
        cfg.mp = new Value(999999);
        stateSystem.baseCfg = cfg;

        stateSystem.Init();
        character.RegisterStateSystem(stateSystem);

    }

    public override void LoadEquipment()
    {
        EquipmentSystem equipmentSystem = new EquipmentSystem();
        character.RegisterEquipmentSystem(equipmentSystem);
    }

    public override void LoadModel()
    {
        character.gameObject = ResFactory.instance.LoadMonster(cfg.model);
    }

    public override void LoadProps()
    {
        PropSystem propSystem = new PropSystem();
        character.RegisterPropSystem(propSystem);
    }

    public override void LoadSkills()
    { 

        SkillSystem skillSystem = new SkillSystem();
        character.RegisterSkillSystem(skillSystem);

        List<int> skills = cfg.skills;
        if (skills != null) { 
            int cnt = skills.Count;
            for (int i = 0; i < cnt; ++i)
            {
                character.AddSkill(ResFactory.instance.GetSkillById(skills[i]));
            }
        }
    }
}
