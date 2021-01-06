using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{ 
    /// <summary>
    /// 储存不同游戏物体的对象池
    /// </summary>
    private Dictionary<string, SubPool> poolDict = new Dictionary<string, SubPool>();

    public GameObject Generate(GameObject prefab,string path) { 
        if (!poolDict.ContainsKey(path)) {
            SubPool subPool = new SubPool(prefab,path);
            poolDict.Add(path, subPool);
        } 
        return poolDict[path].Generate();
    }

    public void Recycle(GameObject obj)
    {
        if (obj.GetComponent<RecoverableObject>() == null)
        {
            LogTool.LogError("回收失败，物体" + obj.name + "需要挂载 RecoverableObject 脚本");
        }
        obj.GetComponent<RecoverableObject>().OnRecycle();
        obj.SetActive(false);
    }

    public void Clear() {

        foreach (KeyValuePair<string,SubPool> pool in poolDict)
        {
            pool.Value.ClearAll();
        }
    } 
    internal void Remove(RecoverableObject recoverableObject)
    {
        //Debug.Log(recoverableObject.gameObject.name);
        //Debug.Log(recoverableObject.transform.parent.name);
        //Debug.Log(recoverableObject.transform.parent.parent.name);
        SubPool subPool = poolDict[recoverableObject.poolID];
        subPool.Remove(recoverableObject);
    }
}
