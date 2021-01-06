using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public class GameManager : BaseManager
{
    // 战斗系统
    public BattleSystem m_BattleSystem { get; } 
    public GameManager(GameRoot gameRoot) : base(gameRoot)
    {
        m_BattleSystem = new BattleSystem();
        LogTool.Log("GameManager");
    }
     

    public override void OnInit()
    {
        base.OnInit();
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        m_BattleSystem.OnUpdate();   
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
    }

}
