using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator  
{

    private float m_OffsetX;
    private float m_OffsetZ;
        
    private BaseAlgorithm m_algorithm;
    private ResFactory m_ResFactory;

    private MapSystem m_MapSystem;

    public MapGenerator(MapSystem mapSystem)
    {
        m_ResFactory = ResFactory.instance;
        this.m_MapSystem = mapSystem;
        this.m_OffsetX = Config.BLOCK_OFFSETX;
        this.m_OffsetZ = Config.BLCOK_OFFSETZ;

    }
    private int[,] dir = new int[4, 2] { { 0, 1 }, { -1, 0 }, { 0, -1 }, { 1, 0 } };
    private int[,] sdir = new int[8, 2] { { 0, 1 }, { -1, 0 }, { 0, -1 }, { 1, 0 }, { 1, 1 }, { -1, 1 }, { -1, -1 }, { 1, -1 } };
 
    // 标记
    private void Dyeing(int[,] map,int [,] visit,int i,int j,int code) {

        int row = map.GetLength(0);
        int col = map.GetLength(1);
        Queue<int> rq = new Queue<int>();
        Queue<int> cq = new Queue<int>();
        rq.Enqueue(i); cq.Enqueue(j);
        visit[i, j] = code;
        while (rq.Count != 0 && cq.Count != 0)
        {
            int cnt = rq.Count;
            for (int n = 0; n < cnt; ++n)
            {
                int currR = rq.Dequeue();
                int currC = cq.Dequeue();
                for (int k = 0; k < 4; ++k)
                {
                    int newR = currR + dir[k, 0];
                    int newC = currC + dir[k, 1];
                    if (newR >= 0 && newR < row && newC >= 0 && newC < col && map[newR,newC] == 0 && visit[newR,newC] != code)
                    {
                        visit[newR, newC] = code; 
                        rq.Enqueue(newR); cq.Enqueue(newC); 
                    }
                }

            } 
        }
    }
    
    // 连通
    private void Connect(int [,]map,int i,int j,int m,int n) {
        int row = map.GetLength(0);
        int col = map.GetLength(1);
        if (i > m) {
            int tmp = i;
            i = m;
            m = tmp;
        }
        // 确认横向连通
        for (int k = i;k <= m;++k) {
            map[k, j] = 0;
        }
        if (j > n)
        {
            int tmp = j;
            j = n;
            n = tmp;
        }
        // 确认纵向连通
        for (int k = j; k <= n; k++)
        {
            map[m, k] = 0;
        }
    }

    private bool CheckMap(int[,] map,int row,int col) {
        int n = 0;
  
        int[,] visit = new int[row,col];
        // 1. 确保覆盖率
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (map[i, j] == 0) ++n;
                visit[i, j] = -1;
            }
        }
        float rate = (float) n / (row * col);
        if (rate < 0.4f) { return false; }

        // 2.  确认是否连通  

        int code = 0;
        Dictionary<int, List<int>> part = new Dictionary<int, List<int>>();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (map[i, j] == 0 && visit[i,j] == -1) {
                    Dyeing(map,visit,i,j,code);
                    part.Add(code,new List<int>(){ i,j});
                    ++code;
                }
            }
        }
        if (code == 1) return true;

        // 3. 如果有没有连通区域，连通！
         
        int r = part[0][0];int c = part[0][1];
        for (int i = 1; i < code; i++)
        {
            int r1 = part[i][0]; int c1 = part[i][1];
            Connect(map,r,c,r1,c1);
        }

        return true;
    }
     
    private int[,] GetMap(int row,int col,AlgoType atype) {
        //TODO 地图加载策略有待完善
        int[,] map;
        do {
            switch (atype)
            {
                case AlgoType.CELLULAR_AUTOMATA:
                    m_algorithm = new CellularAutomata(row,col);
                    break;
                default:
                    break;
            }
            map = m_algorithm.GetMap();
        } while (CheckMap(map,row,col) == false);
         
        return map;
    }



    public Ceil[,] Generate(int row,int col,Vector3 position,Transform parent,AlgoType algoType) {

        Ceil[,] m_Ceils = new Ceil[row, col]; 
        int[,] m_Map = GetMap(row,col,algoType);
        for (int i = 0; i < row; ++i) { 
            for (int j = 0; j < col; ++j) {
                
                //TODO 墙体和地板的加载策略有待完善
                GameObject obj = null;
                if (m_Map[i, j] == 1)
                {
                    int type = QTool.GetRandomInt(0,1);
                    string objName = "";
                    if (type == 0) {
                        int t = QTool.GetRandomInt(1,5);
                        objName = "0" + t + "_stone";
                    }
                    else {
                        int t = QTool.GetRandomInt(1,4);
                        objName = "0" + t + "_tree";
                    }
                    obj = m_ResFactory.LoadWallBlock(objName);   
                }
                else {
                    //int t = QTool.GetRandomInt(1,10);
                    //if (t <= 9) {
                        obj = m_ResFactory.LoadFloorBlock("01_grass");
                    //} else {
                    //    t = QTool.GetRandomInt(2,4);
                    //    string name = "0" + t + "_grass";
                    //    obj = m_ResFactory.LoadFloorBlock(name);
                    //}
                } 
                obj.transform.position =  new Vector3( i * m_OffsetX, 0 ,j * this.m_OffsetZ);
                obj.transform.parent = parent;

                m_Ceils[i, j] = new Ceil(i,j);
                m_Ceils[i, j].ICharacterUpdateEvent += m_MapSystem.CeilCharacterUpdate;
                m_Ceils[i, j].Block = obj.GetComponent<BaseBlock>(); 
            }
        }
         
        return m_Ceils;
    }

  
    public void LoadPlayer(Ceil[,] ceils, Player player) {
        int row = ceils.GetLength(0);
        int col = ceils.GetLength(1);

        int i, j;
        do
        {
            i = QTool.GetRandomInt(0, row - 1, 1);
            j = QTool.GetRandomInt(0, col - 1, 2);
        }
        while (!ceils[i, j].CanBePutOnNow);
          
        ceils[i, j].Character = player;
        player.gameObject.transform.position = ceils[i, j].Position + new Vector3(0, Config.CHARACTER_OFFSETY, 0);
     
    }

    public void LoadMonster(Ceil[,] ceils,Dictionary<int,int> monsterInfo,Transform parent) {

    
        int row = ceils.GetLength(0);
        int col = ceils.GetLength(1);

        int i, j; 

        foreach (KeyValuePair<int,int> kvp in monsterInfo)
        {
            for (int n = 0; n < kvp.Value; ++n) {   
                do
                {
                    i = QTool.GetRandomInt(0, row - 1, 30);
                    j = QTool.GetRandomInt(0, col - 1, 40);
                }
                while (!ceils[i, j].CanBePutOnNow);

                Monster monster = CharacterFactory.instance.GenerateMonster(kvp.Key);
                ceils[i, j].Character = monster;
                monster.gameObject.transform.SetParent(parent);
                monster.gameObject.transform.position = ceils[i, j].Position + new Vector3(0, Config.CHARACTER_OFFSETY, 0);
 
            } 
        }  
    }
    public IBuilding LoadEvent(int eventId, Ceil ceil, Transform parent) {

        IEvent @event = ResFactory.instance.GetEventCfgById(eventId);

        GameObject building = ResFactory.instance.LoadBuilding(@event.model);
        IBuilding baseBuilding = building.GetComponent<IBuilding>();
        baseBuilding.RegisterEvent(@event);

        ceil.Building = baseBuilding;
        building.gameObject.transform.SetParent(parent);
        building.gameObject.transform.position = ceil.Position + new Vector3(0, Config.CHARACTER_OFFSETY, 0);
        return baseBuilding;
    }
    public void LoadShop(Ceil[,] ceils , Transform parent)
    {
        int row = ceils.GetLength(0);
        int col = ceils.GetLength(1);

        int i, j;
        bool flag;
        do {
            do
            {
                i = QTool.GetRandomInt(0, row - 1, 30);
                j = QTool.GetRandomInt(0, col - 1, 40);
            }
            while (!ceils[i, j].CanBePutOnNow);
            flag = true;
            for (int k = 0; k < 8; ++k)
            {
                int newR = i + sdir[k, 0];
                int newC = j + sdir[k, 1];
                if (newR >= 0 && newR < row && newC >= 0 && newC < col && ceils[newR, newC].CanBePutOnNow == false)
                {
                    flag = false;
                    break;
                }
            } 
        } while (flag == false);
         
        IEvent @event = ResFactory.instance.GetEventCfgById(1006);

        GameObject building = ResFactory.instance.LoadBuilding(@event.model);
        IBuilding baseBuilding = building.GetComponent<IBuilding>();
        baseBuilding.RegisterEvent(@event);

        ceils[i, j].IBuildingUpdateEvent += m_MapSystem.CeilBuildingUpdate;
        ceils[i, j].Building = baseBuilding;
        building.gameObject.transform.SetParent(parent);
        building.gameObject.transform.position = ceils[i, j].Position + new Vector3(0, Config.CHARACTER_OFFSETY, 0);
  
    }
    public void LoadEvent(Ceil[,] ceils,Dictionary<int,int> eventInfo, Transform parent) {

     
        int row = ceils.GetLength(0);
        int col = ceils.GetLength(1);

        int i, j;
        foreach (KeyValuePair<int,int> kvp in eventInfo)
        {
            for (int n = 0; n < kvp.Value; ++n)
            {
                do
                {
                    i = QTool.GetRandomInt(0, row - 1, 30);
                    j = QTool.GetRandomInt(0, col - 1, 40);
                }
                while (!ceils[i, j].CanBePutOnNow);


                IEvent @event = ResFactory.instance.GetEventCfgById(kvp.Key);

                GameObject building = ResFactory.instance.LoadBuilding(@event.model);
                IBuilding baseBuilding = building.GetComponent<IBuilding>();
                baseBuilding.RegisterEvent(@event);

                ceils[i, j].IBuildingUpdateEvent += m_MapSystem.CeilBuildingUpdate;
                ceils[i, j].Building = baseBuilding;
                building.gameObject.transform.SetParent(parent);
                building.gameObject.transform.position = ceils[i, j].Position + new Vector3(0, Config.CHARACTER_OFFSETY, 0);
 
            }
        }
         
    }

}
