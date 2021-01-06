using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCeilController 
{
   
    private int row;
    private int col;
    private int[,] dir = new int[4, 2] { { 0,1},{ -1,0},{ 0,-1},{ 1,0} };
     
    private Ceil[,] m_Ceils;
    private List<Ceil> m_SelectedCeils = new List<Ceil>();  // 选中的单元格

    private Ceil m_LastSelected = null;
    private MapSystem m_MapSystem;
     
    public MapCeilController(MapSystem mapSystem) {
        this.m_MapSystem = mapSystem;
    }
    public MapSystem GetMapSystem() { return m_MapSystem; }
    public void RegisterCeils(Ceil[,] ceils) {
        this.m_Ceils = ceils;
        this.row = ceils.GetLength(0);
        this.col = ceils.GetLength(1);

        foreach (Ceil ceil in ceils) {
            ceil.RegisterController(this);
        }
    }
    //public void ClearSelectedCeil()
    //{ 
    //    foreach (Ceil ceil in m_SelectedCeils)
    //    {
    //        ceil.CancelSelectedMove();
    //    }
    //    m_SelectedCeils.Clear();
    //}

    public void Clear() { m_LastSelected = null; }
    public void OnSelected(Ceil ceil)
    {
        //if (ceil.isSelected)
        //{
        //    ICharacter tmp = ceil.Character;
        //    ceil.SelectedEvent?.Invoke(ceil);

        //    // 更新地图资源信息
        //    if (ceil.Character != null)
        //    {
        //        m_MapSystem.characterCeilDict[ceil.Character] = ceil; 
        //    }
        //    ClearSelectedCeil();
        //}
        //else
        //{
        //   ClearSelectedCeil();

        m_LastSelected?.CancelSelected();
        ceil?.Selected();
        m_LastSelected = ceil;
        m_MapSystem.player.OnSelected(this,ceil);

            // TODO 有待调整
            //if (ceil.Character != null)
            //{
            //    ceil.Character.OnSelected(this, ceil);
            //}
            //else if (ceil.Building != null)
            //{
            //    ceil.Building.OnSelected(this,ceil);
            //}
            //else
            //{ 
            //    ceil.Block.BlockSelected();
            //}
        //}
    }

     
    // 寻找附近可以被放置的点
    public Ceil FindTheAvailableCeilsNearby(Ceil[,] ceils, Ceil target)
    {

        if (target.CanBePutOnNow) return target;
        int row = ceils.GetLength(0);
        int col = ceils.GetLength(1);
         
        Queue<Ceil> cq = new Queue<Ceil>();
        HashSet<Ceil> visited = new HashSet<Ceil>();
        cq.Enqueue(target);

        while (cq.Count != 0)
        {
            int n = cq.Count;
            for (int i = 0; i < n; i++)
            {
                Ceil tmp = cq.Dequeue(); 
                for (int k = 0; k < 4; k++)
                {
                    int newR = tmp.row + dir[k, 0];
                    int newC = tmp.col + dir[k, 1];
                    if (newR >= 0 && newR < row && newC >= 0 && newC < col && m_Ceils[newR, newC].Block != null && m_Ceils[newR, newC].Block.blockType == BlockType.FLOOR  )
                    {  
                        if (m_Ceils[newR, newC].CanBePutOnNow)
                        {
                            return m_Ceils[newR, newC];
                        }
                        else
                        {
                            cq.Enqueue(m_Ceils[newR,newC]);
                        }
                    }
                }
            }

        } 
        return null;
    }

    #region 不同的选取表现形式
    // 显示移动范围
    public void ShowMoveSelectRange( Ceil ceil,int power ,Action<Ceil> SelectedCallBack)
    {
        int i = ceil.row, j = ceil.col; 
        Queue<int> rq = new Queue<int>(); 
        Queue<int> cq = new Queue<int>();
        rq.Enqueue(i);cq.Enqueue(j);
        m_Ceils[i, j].SelectedMove(null,null);
        m_SelectedCeils.Add(m_Ceils[i,j]);
        while (rq.Count != 0 && cq.Count != 0 && power != 0) {
            int cnt = rq.Count; 
            for (int n = 0; n < cnt; ++n ) {
                int currR = rq.Dequeue();
                int currC = cq.Dequeue();
                for (int k = 0; k < 4; ++k) {
                    int newR = currR + dir[k , 0];
                    int newC = currC + dir[k , 1];
                    if (newR >= 0 && newR < row && newC >= 0 && newC < col 
                        && m_Ceils[newR, newC].Block != null && m_Ceils[newR, newC].Block.blockType == BlockType.FLOOR && m_Ceils[newR,newC].isSelected == false) {
                        
                        m_Ceils[newR, newC].SelectedMove(m_Ceils[currR, currC], SelectedCallBack);
                        m_SelectedCeils.Add(m_Ceils[newR, newC]);

                        if (m_Ceils[newR, newC].Character == null && m_Ceils[newR, newC].Building == null)
                        {
                            rq.Enqueue(newR);cq.Enqueue(newC);
                        } 
                    }
                }

            }
            --power;
        } 
    }
    #endregion
}
