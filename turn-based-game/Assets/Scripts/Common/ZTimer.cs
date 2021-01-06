using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

//public static class ZTimerSvc {

//    static ZTimer timer = new ZTimer(); 
//    public static void AddTask(double seconds, Action callBack)
//    {
//        timer.AddTask(seconds,callBack);
//    }
//    public static void ClearALLTask() {
//        timer.ClearALLTask();
//    }
//}


/// <summary>
/// 简易定时器 单线程使用
/// </summary>
//class ZTimer
//{
//    private class TimeTask {
//        public Action callBack;
//        public double delayTime;
//        public TimeTask(double delayTime, Action callBack) {
//            this.delayTime = delayTime;
//            this.callBack = callBack;
//        }
//    };

//    private Timer m_Timer;
//    private float m_Interval = 100;
//    private DateTime m_StartDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
//    private List<TimeTask> m_TaskList = new List<TimeTask>();
//    private static readonly object mutex = new object();
    
//    public ZTimer() {
//        m_Timer = new Timer(m_Interval);
//        m_Timer.AutoReset = true;
//        m_Timer.Elapsed += (object sender, ElapsedEventArgs e) =>
//        {
//            Update();
//        };
//        m_Timer.Start();
       
//    }

//    ~ZTimer() { 
//        m_Timer.Close();
//    }
//    public void AddTask(double seconds,Action callBack) {
//        if(seconds < 0.001){
//            callBack?.Invoke();
//            return;
//        }
//        lock (mutex) { 
//            double delay = seconds * 1000 + GetUTCMilliseconds();
//            m_TaskList.Add(new TimeTask(delay,callBack));
//        }
//    } 
//    private double GetUTCMilliseconds() {
//        TimeSpan ts = DateTime.UtcNow - m_StartDateTime;
//        return ts.TotalMilliseconds;
//    }

//    private void Update()
//    {
//        lock (mutex) { 
//            double nowTime = GetUTCMilliseconds();
//            for (int index = 0; index < m_TaskList.Count; index++)
//            {
//                TimeTask task = m_TaskList[index];
//                //LogTool.Log(nowTime + "    " + task.delayTime);
//                if (nowTime.CompareTo(task.delayTime) < 0){
//                    continue;
//                }
//                else
//                {
//                    m_TaskList.RemoveAt(index);
//                    index--; 
//                    task.callBack?.Invoke(); 
//                }

//            }
//        }
//    }

//    public void ClearALLTask()
//    {
//        lock (mutex) {
//            m_TaskList.Clear();
//        }
//    }
//} 
