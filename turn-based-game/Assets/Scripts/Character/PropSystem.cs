using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSystem   
{
    // 物品ID 数量
    public Dictionary<int, int> props = new Dictionary<int, int>();

    public void AddProp(int propId) {
        if (!props.ContainsKey(propId)) props.Add(propId, 1);
        else props[propId] += 1;
    }
    public int UseProp(int propId) {
        int res = 0;
        props[propId] -= 1;
        res = props[propId];
        if (props[propId] == 0) { props.Remove(propId); }
        return res;
    }
}
