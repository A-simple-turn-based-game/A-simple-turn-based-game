using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCfg 
{
    public int id;
    public int row;
    public int col;
    public string name;
    public string modelType;
    public string description;

    public string bgm;
    public string bossbgm;
    public string battlebgm;

    public Dictionary<int,int> monster = new Dictionary<int, int>();
    public Dictionary<int, int> @event = new Dictionary<int, int>();
    // 下一个地图集
    public List<int> nextMap = new List<int>();
}
