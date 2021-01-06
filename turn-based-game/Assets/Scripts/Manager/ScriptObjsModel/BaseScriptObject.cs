using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseScriptObject : ScriptableObject{ 
    public virtual void GetScriptObject() { }
     
    public object GetResByName(string name)
    {
        //Debug.Log(name);
        object obj = this.GetType().GetField(name).GetValue(this);
        //Debug.Log( ( (GameObject)obj ).name);
        return obj;
    }
}
