using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilder : ICharacterBuilder
{ 

    public override void AddAI()
    {
        //TODO 玩家AI
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
        StateSystem stateSystem = new StateSystem(character);
           
        stateSystem.baseCfg = this.cfg;
        stateSystem.Init();
        character.RegisterStateSystem(stateSystem);
    }

    public void CompletionAttribute(StateSystem oldStateSystem)
    { 
        StateSystem stateSystem = new StateSystem(character);
        stateSystem.hp = oldStateSystem.hp;
        stateSystem.mp = oldStateSystem.mp;
        stateSystem.baseCfg = this.cfg;
        character.GetEquipmentSystem().ReLoad(stateSystem);
        stateSystem.Refresh();
        character.RegisterStateSystem(stateSystem);
    }

    public override void LoadEquipment()
    {
        EquipmentSystem equipmentSystem = new EquipmentSystem();
        equipmentSystem.equipment.Add(1011);
        character.RegisterEquipmentSystem(equipmentSystem);
    }

    public override void LoadModel()
    {
        
        character.gameObject = ResFactory.instance.LoadPlayer(cfg.model);
    }

    public override void LoadProps()
    {
        PropSystem propSystem = new PropSystem();
        propSystem.props.Add(1001,2);
        propSystem.props.Add(1002,2);
        character.RegisterPropSystem(propSystem);
    }

    public override void LoadSkills()
    { 
        SkillSystem skillSystem = new SkillSystem();
        character.RegisterSkillSystem(skillSystem);

        List<int> skills = cfg.skills;
        if (skills != null) { 
            int cnt = Mathf.Min(3,skills.Count);
            for (int i = 0; i < cnt; ++i) { 
                character.AddSkill(ResFactory.instance.GetSkillById(skills[i]));
            } 
        }
    }
}
