using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNetSys 
{
    private static LobbyNetSys _instance;
    public static LobbyNetSys instance
    {
        get
        {
            if (_instance == null) _instance = new LobbyNetSys();
            return _instance;
        }
    }
    private LobbyPanel m_LobbyPanel;
    public void OnInit(LobbyPanel lobbyPanel)
    {
        m_LobbyPanel = lobbyPanel;
    }

    public void RspRequestSaveInfoMsg(RspRequestSaveInfoMsg msg)
    {
        int code = msg.code;
        EventCenter.Broadcast<string>(EventType.TIPS, msg.msg);
        if (code == 0)
        {
        }
        else
        {
            m_LobbyPanel.RspContinueGame(msg._lastMapIdx,msg._lastPlayerInfo);
        }
    }

    internal void RspRequestLoginQuitMsg(RspLoginQuitMsg msg)
    {
        int code = msg.code;
        EventCenter.Broadcast<string>(EventType.TIPS, msg.msg);
        if (code == 0)
        {
        }
        else
        {
            LogTool.Log("退出成功");
        }
    }
}
