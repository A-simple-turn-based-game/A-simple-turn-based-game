using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubPool  
{
    private List<GameObject> m_ObjectList = new List<GameObject>();
    private GameObject m_Prefab;
    private string poolID;
    public SubPool(GameObject prefab,string poolID) {
        this.m_Prefab = prefab;
        this.poolID = poolID;
    }
     

    public GameObject Generate() {
        GameObject target = null; 
        foreach (GameObject obj in m_ObjectList) {
            if (!obj.activeInHierarchy) {
                target = obj; break;
            }
        }
        if (target == null) {
            target = GameObject.Instantiate(m_Prefab);
            m_ObjectList.Add(target);
        }
        if (target.GetComponent<RecoverableObject>() == null) {
            LogTool.LogError("生成失败，物体" + m_Prefab.name + "需要挂载 RecoverableObject 脚本");
        }
        target.GetComponent<RecoverableObject>().poolID = poolID;
        target.SetActive(true);
        target.GetComponent<RecoverableObject>().OnGenerate();
        return target;
    }
    public void RecycleAll()
    {
        foreach (GameObject obj in m_ObjectList) {
            if (!obj.activeInHierarchy) continue;
            if (obj.GetComponent<RecoverableObject>() == null)
            {
                LogTool.LogError("回收失败，物体" + m_Prefab.name + "需要挂载 RecoverableObject 脚本");
            }   
            obj.GetComponent<RecoverableObject>().OnRecycle();
            obj.SetActive(false);
        }
    }

    internal void Remove(RecoverableObject recoverableObject)
    {
        m_ObjectList.Remove(recoverableObject.gameObject);
    }

    public void ClearAll() {
        foreach (GameObject obj in m_ObjectList)
        {
            GameObject.Destroy(obj);
        }
        m_ObjectList.Clear();
    }

}
