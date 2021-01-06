using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池物体必须继承
/// </summary>
public class RecoverableObject : MonoBehaviour
{

    public string poolID;

    public virtual void OnGenerate() { }

    public virtual void OnRecycle() { }


    public void OnDestroy()
    {
        ResFactory.instance.RemoveObjectFromPool(this);
    }
}
