using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearTrackMonsterAI : ICharacterAI
{


    private int CheckRow(Ceil[,] ceils,Ceil mine,Ceil target) {
        int col = mine.col;
        int flag = mine.row < target.row ? 1 : -1;
        int minRow = Mathf.Min(mine.row,target.row);
        int maxRow = Mathf.Max(mine.row,target.row);
        for (int i = minRow+1; i < maxRow; i++)
        {
            if (ceils[i, col].CanBePutOnNow == false) return 0;
        }
        return flag;
    }
    private int CheckCol(Ceil[,] ceils, Ceil mine, Ceil target)
    {
        int row = mine.row;
        int flag = mine.col < target.col ? 1 : -1;
        int minCol = Mathf.Min(mine.col, target.col);
        int maxCol = Mathf.Max(mine.col, target.col);
        for (int i = minCol+1; i < maxCol; i++)
        {
            if (ceils[row, i].CanBePutOnNow == false) return 0;
        }
        return flag; 
    }
    public override void MapActionAI(MapSystem mapSystem)
    {
        Ceil mine = mapSystem.characterCeilDict[character];
        Ceil target = mapSystem.characterCeilDict[mapSystem.player];
        Ceil[,] ceils = mapSystem.ceils;
 
        if (mine.row == target.row) {
            // 再检查纵向
            int flag = CheckCol(mapSystem.ceils, mine, target);
            if (flag != 0) {
                Ceil next = ceils[mine.row , mine.col + flag]; 
                controllerSystem.ResetAndAddMoveCeilBuffer(next);
                controllerSystem.GetHUDCanvas().FindPlayer() ;
                return;
            }
        } else if (mine.col == target.col) { 
            // 先检查横向
            int flag = CheckRow(mapSystem.ceils,mine,target);
            if (flag != 0) {
                Ceil next = ceils[mine.row + flag, mine.col]; 
                controllerSystem.ResetAndAddMoveCeilBuffer(next);
                controllerSystem.GetHUDCanvas().FindPlayer();
                return;
            }
        }
        controllerSystem.GetHUDCanvas().LossPlayer();
        character.isEndMapRound = true;  
    }
}
