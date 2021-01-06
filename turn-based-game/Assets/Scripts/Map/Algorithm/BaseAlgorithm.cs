using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AlgoType { 

    CELLULAR_AUTOMATA
};

public abstract class BaseAlgorithm 
{
    protected int m_Row;
    protected int m_Col;

    protected readonly int[,] m_OneDir = new int[8, 2] { { 0, 1 }, { 0, -1 }, { 1, 1 }, { 1, 0 }, { 1, -1 }, { -1, 0 }, { -1, -1 }, { -1, 1 } };
    protected readonly int[,] m_TwoDir = new int[12, 2] { { 0, 2 }, { 0, -2 }, { 1, 2 }, { 1, -2 }, { -1, 2 }, { -1, -2 }, { 2, 2 }, { 2, 0 }, { 2, -2 }, { -2, 0 }, { -2, -2 }, { -2, 2 } };

    public BaseAlgorithm(int row, int col)
    {
        this.m_Row = row;
        this.m_Col = col;
    }

    public abstract int[,] GetMap();
}
