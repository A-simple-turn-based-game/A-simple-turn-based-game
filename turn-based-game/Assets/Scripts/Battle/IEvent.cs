using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 type:
0 - 显示事件界面
1 - 直接触发效果
 */

public enum GameEventType {
    SelectEvents,
    TriggeringEvent
}
public class IEvent 
{
    public int id;
    public GameEventType type;
    public string icon;
    public string name;
    public string description;
    public string model;
    public List<string> shortOpts;
    public List<string> options;
    public List<string> results;
    public List<List<List<Value>>> effects;
}
