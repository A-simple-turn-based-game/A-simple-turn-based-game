using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager
{
    protected GameRoot gameRoot;
    public BaseManager(GameRoot gameRoot)
    {
        this.gameRoot = gameRoot;
    }
    public virtual void OnInit() { }
    public virtual void OnUpdate() { }
    public virtual void OnDestroy() { }
}
