using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTraceMonsterAI : ICharacterAI
{
    Ceil m_LastPlayerCeil = null;
    AStar m_AStar = new AStar();

    public override void MapActionAI(MapSystem mapSystem)
    {
        Ceil ceil = mapSystem.characterCeilDict[mapSystem.player];
        Ceil curr = mapSystem.characterCeilDict[character];
        if (ceil == m_LastPlayerCeil)
        {
            // 继续上次寻路结果
            controllerSystem.StartMoving();
        }
        else {
            // 重新规划寻路
            Stack<Ceil> path = m_AStar.GetPath(mapSystem.ceils,curr,ceil);
            if (path == null || path.Count == 0) {
                character.isEndMapRound = true;
            }
            else
            {
                controllerSystem.RegisterMoveCeilBuffer(path);
            }
        }
        controllerSystem.GetHUDCanvas().FindPlayer();
        m_LastPlayerCeil = ceil;
    }
}
