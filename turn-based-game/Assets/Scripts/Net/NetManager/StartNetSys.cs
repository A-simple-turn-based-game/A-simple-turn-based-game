using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartNetSys  
{
    private static StartNetSys _instance;
    public static StartNetSys instance {
        get {
            if (_instance == null) _instance = new StartNetSys();
            return _instance;
        }
    }

    private StartPanel m_StartPanel;
    public void OnInit(StartPanel panel) { 
        m_StartPanel = panel;
    }
     

    public void RspLoginMsg(RspLoginMsg msg) { 
        
        int code = msg.code;
        EventCenter.Broadcast<string>(EventType.TIPS,msg.msg);
        if (code == 0)
        {
        }
        else {
            m_StartPanel.loginGroup.gameObject.SetActive(false);
            Global.isOnlineLogin = true;
            m_StartPanel.StartGame();
        } 

    }
    public void RspRegisterMsg(RspRegisterMsg rspRegisterMsg)
    {
        int code = rspRegisterMsg.code;
        EventCenter.Broadcast<string>(EventType.TIPS, rspRegisterMsg.msg);
        if (code == 0)
        {
        }
        else
        {
            m_StartPanel.registerGroup.gameObject.SetActive(false);
            m_StartPanel.loginGroup.gameObject.SetActive(true);
        }
    }

}
