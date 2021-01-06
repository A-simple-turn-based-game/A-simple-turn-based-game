using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory
{  

    private static CharacterFactory _instance = null;

    public static CharacterFactory instance {
        get {
            if (_instance == null) _instance = new CharacterFactory();
            return _instance;
        }
    }

    private CharacterFactory() { 
    }
    // 目前只有一种角色 Player 暂时没有 PlayerRoles playerRoles
    public Player GeneratePlayer(int id) {

        PlayerBuilder playerBuilder = new PlayerBuilder();

        playerBuilder.character = new Player();

        playerBuilder.cfg = ResFactory.instance.GetPlayerCfgById(id);
        
        playerBuilder.LoadModel();
        playerBuilder.LoadAttribute();
        playerBuilder.LoadEquipment();
        playerBuilder.LoadProps();
        playerBuilder.LoadSkills();
        playerBuilder.LoadController();
        playerBuilder.AddAI();

        return playerBuilder.character as Player; 
    }


    public Player CompletionPlayer(Player player)
    {

        PlayerBuilder playerBuilder = new PlayerBuilder();

        playerBuilder.character = player;

        StateSystem oldStateSystem = player.GetStateSystem();
        playerBuilder.cfg = oldStateSystem.baseCfg;

        playerBuilder.CompletionAttribute(oldStateSystem);
          
        playerBuilder.LoadModel();   
        playerBuilder.LoadController(); 

        return playerBuilder.character as Player;
    }
    public Monster GenerateMonster(int id) {
        MonsterBuilder monsterBuilder = new MonsterBuilder();
         
        monsterBuilder.character = new Monster();

        monsterBuilder.cfg = ResFactory.instance.GetMonsterCfgById(id);

        monsterBuilder.LoadModel();
        
        monsterBuilder.LoadAttribute();  
        
        monsterBuilder.LoadSkills();
        
        monsterBuilder.LoadController();

        monsterBuilder.AddAI();

        return monsterBuilder.character as Monster; 
    }
}
