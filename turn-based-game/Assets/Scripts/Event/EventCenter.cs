using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter
{
    private static Dictionary<EventType, Delegate> m_EventTable = new Dictionary<EventType, Delegate>();

    //无参数
    public static void AddListener(EventType eventType, CallBack callBack)
    {
        if (!m_EventTable.ContainsKey(eventType))
        {
            m_EventTable.Add(eventType, null);
        }

        Delegate dele = m_EventTable[eventType];
        if (dele != null && dele.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("尝试添加不同类型的委托"));
        }

        m_EventTable[eventType] = (CallBack)m_EventTable[eventType] + callBack;

    }
    public static void AddListener<T>(EventType eventType, CallBack<T> callBack)
    {
        if (!m_EventTable.ContainsKey(eventType))
        {
            m_EventTable.Add(eventType, null);
        }

        Delegate dele = m_EventTable[eventType];
        if (dele != null && dele.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("尝试添加不同类型的委托"));
        }

        m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] + callBack;

    }
    public static void AddListener<T,K>(EventType eventType, CallBack<T,K> callBack)
    {
        if (!m_EventTable.ContainsKey(eventType))
        {
            m_EventTable.Add(eventType, null);
        }

        Delegate dele = m_EventTable[eventType];
        if (dele != null && dele.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("尝试添加不同类型的委托"));
        }

        m_EventTable[eventType] = (CallBack<T,K>)m_EventTable[eventType] + callBack;

    }
    public static void AddListener<T, K ,V>(EventType eventType, CallBack<T, K ,V> callBack)
    {
        if (!m_EventTable.ContainsKey(eventType))
        {
            m_EventTable.Add(eventType, null);
        }

        Delegate dele = m_EventTable[eventType];
        if (dele != null && dele.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("尝试添加不同类型的委托"));
        }

        m_EventTable[eventType] = (CallBack<T, K , V>)m_EventTable[eventType] + callBack;

    }
    public static void RemoveListener(EventType eventType, CallBack callBack)
    {
        if (m_EventTable.ContainsKey(eventType))
        {
            Delegate dele = m_EventTable[eventType];
            if (dele == null)
            {
                throw new Exception(string.Format("移除监听错误"));
            }
            else if (dele.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("移除监听错误"));

            }
        }
        else
        {
            throw new Exception(string.Format("移除监听错误"));
        }
        m_EventTable[eventType] = (CallBack)m_EventTable[eventType] - callBack;
        if (m_EventTable[eventType] == null)
        {
            m_EventTable.Remove(eventType);
        }
    }
    //one
    public static void RemoveListener<T>(EventType eventType, CallBack<T> callBack)
    {
        if (m_EventTable.ContainsKey(eventType))
        {
            Delegate dele = m_EventTable[eventType];
            if (dele == null)
            {
                throw new Exception(string.Format("移除监听错误"));
            }
            else if (dele.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("移除监听错误"));

            }
        }
        else
        {
            throw new Exception(string.Format("移除监听错误"));
        }
        m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] - callBack;

    }
    public static void RemoveListener<T,K>(EventType eventType, CallBack<T,K> callBack)
    {
        if (m_EventTable.ContainsKey(eventType))
        {
            Delegate dele = m_EventTable[eventType];
            if (dele == null)
            {
                throw new Exception(string.Format("移除监听错误"));
            }
            else if (dele.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("移除监听错误"));

            }
        }
        else
        {
            throw new Exception(string.Format("移除监听错误"));
        }
        m_EventTable[eventType] = (CallBack<T,K>)m_EventTable[eventType] - callBack;

    }
    public static void RemoveListener<T, K ,V>(EventType eventType, CallBack<T, K, V> callBack)
    {
        if (m_EventTable.ContainsKey(eventType))
        {
            Delegate dele = m_EventTable[eventType];
            if (dele == null)
            {
                throw new Exception(string.Format("移除监听错误"));
            }
            else if (dele.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("移除监听错误"));

            }
        }
        else
        {
            throw new Exception(string.Format("移除监听错误"));
        }
        m_EventTable[eventType] = (CallBack<T, K ,V>)m_EventTable[eventType] - callBack;

    }
    public static void Broadcast(EventType eventType)
    {
        Delegate dele;
        if (m_EventTable.TryGetValue(eventType, out dele))
        {

            CallBack callBack = dele as CallBack;
            if (callBack != null)
            {
                callBack();
            }
            else
            {
                throw new Exception(string.Format("广播失败"));
            }
        }
    }
    public static void Broadcast<T>(EventType eventType, T arg)
    {
        Delegate dele;
        if (m_EventTable.TryGetValue(eventType, out dele))
        {

            CallBack<T> callBack = dele as CallBack<T>;
            if (callBack != null)
            {
                callBack(arg);
            }
            else
            {
                throw new Exception(string.Format("广播失败" + eventType.ToString()));
            }
        }
        else
        {
            Debug.Log(eventType + "无法找到");
        }
    }
    public static void Broadcast<T,K>(EventType eventType, T arg,K arg1)
    {
        Delegate dele;
        if (m_EventTable.TryGetValue(eventType, out dele))
        {

            CallBack<T,K> callBack = dele as CallBack<T,K>;
            if (callBack != null)
            {
                callBack(arg,arg1);
            }
            else
            {
                throw new Exception(string.Format("广播失败"));
            }
        }
    }
    public static void Broadcast<T, K ,V>(EventType eventType, T arg, K arg1,V arg2)
    {
        Delegate dele;
        if (m_EventTable.TryGetValue(eventType, out dele))
        {

            CallBack<T, K, V> callBack = dele as CallBack<T, K ,V>;
            if (callBack != null)
            {
                callBack(arg,arg1, arg2);
            }
            else
            {
                throw new Exception(string.Format("广播失败"));
            }
        }
    }
}

