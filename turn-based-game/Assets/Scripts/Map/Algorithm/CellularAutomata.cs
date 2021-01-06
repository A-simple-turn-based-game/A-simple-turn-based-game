 
using System.Collections;
using System.Collections.Generic; 

public class CellularAutomata : BaseAlgorithm
{

    private int m_InitWallRandomPercent;
     
    
    private int m_WallDeathRate;
    private int m_WallLiveRate;
    private int m_LifeAndDeadTime = 10;
    private int m_DeathTime = 1;


    public CellularAutomata(int row,int col,int initWallRandomPercent = 50,int wallDeathRate = 5, int wallLiveRate = 2):base( row , col ) {

        this.m_InitWallRandomPercent = initWallRandomPercent % 100;
        this.m_WallDeathRate = wallDeathRate % 9;
        this.m_WallLiveRate = wallLiveRate % 16;
    }

    public override int[,] GetMap() {
        int[,] board = new int[m_Row, m_Col];
        InitRandom(ref board);
        //先对墙体进行生长和死亡
        for (int i = 0; i < m_LifeAndDeadTime; ++i)
        {
            GameOfLife(ref board, 2);
        }
        // 再对墙体进行死亡,扩充道路
        for (int i = 0; i < m_DeathTime; ++i)
        {
            GameOfLife(ref board, 1);
        }
        return board;
    }


    /// <summary>
    /// 初始化地图，有 m_InitWallRandomPercent /100 的可能性初始化为墙体
    /// </summary>
    /// <param name="board">网格</param>
    void InitRandom(ref int[,] board)
    {
        for (int i = 0; i < m_Row; i++)
        {
            for (int j = 0; j < m_Col; j++)
            {
                if (i == 0 || i == m_Row - 1 || j == 0 || j == m_Col - 1) {
                    board[i, j] = 1;continue;
                }

                int val = QTool.GetRandomInt(0, 99 ,i * 10 + j);
                //LogTool.Log(""+val);
                if (val <= m_InitWallRandomPercent) {
                    board[i, j] = 1; // 墙
                }
                else { 
                    board[i, j] = 0; // 路
                }
            }
        } 
    }


    /// <summary>
    /// 细胞自动机
    /// </summary>
    /// <param name="board">网格</param>
    /// <param name="mode">1 - 进行生长和死亡  2 - 只进行死亡 </param>
    void GameOfLife(ref int[,] board,int mode = 1)
    {  
        for (int i = 1; i < m_Row-1; i++)
        {
            for (int j = 1; j < m_Col-1; j++)
            { 
                int count = 0;
                // 计算周围第一圈
                for (int k = 0; k < 8; k++)
                {
                    int h = i + m_OneDir[k,0];
                    int w = j + m_OneDir[k,1];
                    if (h >= 0 && w >= 0 && h < m_Row && w < m_Col && (board[h,w] & 1) == 1)
                    {
                        count++;
                    }
                }
                if (count >= m_WallDeathRate)
                {
                    board[i,j] |= 0x2;
                    continue;
                }
                if (mode == 1) continue;


                // 计算周围第二圈 
                for (int k = 0; k < 8; k++)
                {
                    int h = i + m_TwoDir[k, 0];
                    int w = j + m_TwoDir[k, 1];
                    if (h >= 0 && w >= 0 && h < m_Row && w < m_Col && (board[h,w] & 1) == 1)
                    {
                        count++;
                    }
                }
                if (count <= m_WallLiveRate) {
                    board[i,j] |= 0x2;
                }
            }
        } 
        for (int i = 1; i < m_Row-1; i++)
        {
            for (int j = 1; j < m_Col-1; j++)
            {
                board[i,j] >>= 1;
            }
        }
    }
}
