using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 不做斜向移动
public class AStar 
{
    private Stack<Ceil> m_Path;

    private HashSet<Ceil> m_OpenSet = new HashSet<Ceil>();

    private HashSet<Ceil> m_CloseSet = new HashSet<Ceil>();

    private int[,] m_Dir = new int[4, 2] { { 0, 1 }, { -1, 0 }, { 0, -1 }, { 1, 0 } };

    private int m_Cost = 1;

    private Ceil[,] m_Ceils;

    private int m_Row;
    private int m_Col;

     

    private void OnInit(Ceil[,] ceils, Ceil start, Ceil end) {
        this.m_Ceils = ceils;
        this.m_Row = ceils.GetLength(0);
        this.m_Col = ceils.GetLength(1);
        start.lastCeil = null;
        start.F = 0;
        start.H = 0;
        start.G = 0;
        end.lastCeil = null;

        m_Path = new Stack<Ceil>();
        m_OpenSet.Clear();
        m_CloseSet.Clear();
    
    } 
    public Stack<Ceil> GetPath(Ceil[,] ceils,Ceil start,Ceil end) {

        if (start == end || end.Block is WallBlock) return null;
        
        if(start.Character != null)
            Debug.Log(start.Character.name + " A 寻路 ： " );
        Debug.Log(start +  " to "  +end);
        OnInit(ceils,start,end); 
        if (FindPath(start, end) == false) return null;
 
        while (end != null && end != start) {
            m_Path.Push(end);
            end = end.lastCeil;
        } 
        return m_Path;
    }
    public Stack<Ceil> GetPath(Ceil[,] ceils, Ceil start, Ceil end,out List<Vector3> pathList)
    {
        pathList = new List<Vector3>();
        if (start == end || end.Block is WallBlock) return null;
        if (start.Character != null)
            Debug.Log(start.Character.name + " A 寻路 ： ");
        Debug.Log(start + " to " + end);
        OnInit(ceils, start, end);
        if (FindPath(start, end) == false) return null;

        while (end != null && end != start)
        {
            m_Path.Push(end);
            pathList.Add(end.Position);
            end = end.lastCeil;
        }
        return m_Path;
    }
    private Ceil GetMinFCeil() { 
        Ceil res = null;
        int minF = int.MaxValue;
        foreach (Ceil ceil in m_OpenSet)
        {
            if (ceil.F < minF) { 
                res = ceil;
                minF = ceil.F;
            }
        }
        return res;
    }
    //private List<Ceil> GetSurroundCeils(Ceil ceil) {
    //    List<Ceil> surround = new List<Ceil>();
    //    for (int i = 0; i < 4; i++)
    //    {
    //        int newR = ceil.row + m_Dir[i, 0];
    //        int newC = ceil.col + m_Dir[i, 1];
    //        if (newR >= 0 && newR < m_Row && newC >= 0 && newC < m_Col 
    //            && m_Ceils[newR,newC].CanBePutOn
    //            && !m_CloseSet.Contains(m_Ceils[newR,newC]))
    //        {
    //            surround.Add(m_Ceils[newR, newC]);
    //        }
    //    }
    //    return surround;
    //}
    private List<Ceil> GetSurroundCeils(Ceil ceil,Ceil target)
    {
        List<Ceil> surround = new List<Ceil>();
        for (int i = 0; i < 4; i++)
        {
            int newR = ceil.row + m_Dir[i, 0];
            int newC = ceil.col + m_Dir[i, 1];
            if (newR >= 0 && newR < m_Row && newC >= 0 && newC < m_Col
                && (m_Ceils[newR, newC].CanBePutOnNow || m_Ceils[newR, newC] == target)
                && !m_CloseSet.Contains(m_Ceils[newR, newC]))
            {
                surround.Add(m_Ceils[newR, newC]);
            }
        }
        return surround;
    }

    private bool FindPath(Ceil start, Ceil end) {
        
        m_OpenSet.Add(start);
        while (m_OpenSet.Count != 0) {
            Ceil curr = GetMinFCeil();
            m_OpenSet.Remove(curr);
            m_CloseSet.Add(curr);
            List<Ceil> sList = GetSurroundCeils(curr,end);
            foreach (Ceil next in sList)
            {
                if (m_OpenSet.Contains(next))
                {
                    int g = curr.G + m_Cost;
                    if (next.G > g) {
                        next.lastCeil = curr;
                        next.G = g;
                        next.F = next.G + next.H;
                    }
                }
                else {
                    next.lastCeil = curr;
                    next.G = curr.G + m_Cost;
                    next.H = Mathf.Abs(next.col - end.col) + Mathf.Abs(next.row - end.row);
                    next.F = next.H + next.G;
                    m_OpenSet.Add(next);
                }
                if (m_OpenSet.Contains(end)) return true;
            }
        }
        return false;
    }

}
