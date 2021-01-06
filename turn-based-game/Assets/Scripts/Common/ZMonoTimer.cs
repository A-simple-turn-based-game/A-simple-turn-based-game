using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class ZTimerSvc
{
    static ZMonoTimer m_ZTimer;
    public static void OnInit(ZMonoTimer timer) {
        m_ZTimer = timer;
    }
    public static void AddTask(float seconds, Action callBack)
    {
        m_ZTimer.AddTask(seconds, callBack);
    }
    public static void ClearAllTask()
    {
        m_ZTimer.ClearAllTask();
    }
}

public class ZMonoTimer : MonoBehaviour
{ 
    private List<Coroutine> taskList = new List<Coroutine>();
    private void Awake()
    { 
        DontDestroyOnLoad(this);
    }
    public void AddTask(float seconds, Action callBack)
    {
        if (seconds < 0.001)
        {
            callBack?.Invoke();
            return;
        }
        Coroutine c = StartCoroutine(DelayCall(seconds,callBack));
        taskList.Add(c);
    }
    public IEnumerator DelayCall(float delayTime,Action callBack) {

        yield return new WaitForSeconds(delayTime);
        callBack?.Invoke(); 
    }

    public void ClearAllTask()
    { 
        StopAllCoroutines();
    }
}
