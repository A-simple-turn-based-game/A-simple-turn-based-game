using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : BaseManager
{  
     
    public CharacterManager(GameRoot gameRoot) : base(gameRoot)
    {
    }

    public override void OnInit()
    { 
        base.OnInit();
    }

    /// <summary>
    /// 设置出生点
    /// </summary>
    /// <param name="SpawnPoint"></param>
    public void SetSpawnPoint(Transform SpawnPoint)
    { 
    }
    /// <summary>
    /// 生成角色
    /// </summary>
    public void GeneratePlayer(Transform parent = null)
    { 
    }
     
    public void PlayerDead()
    {  
    } 
    public void DisableControl()
    {
 
    }
    public void EnableControl()
    {
 
    }

    public void Reset()
    { 
 
    }

}
