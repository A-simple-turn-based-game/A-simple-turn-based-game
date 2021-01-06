 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LogTool
{
    public static void Log(object msg) {
       if(Config.isLog) Debug.Log(msg);
    }

    public static void LogError(object msg) {
        if(Config.isLogError) Debug.LogError(msg);
    }
    public static void LogWarning(object msg) {
        if (Config.isLogWarning) Debug.LogWarning(msg);
    }

    
}
