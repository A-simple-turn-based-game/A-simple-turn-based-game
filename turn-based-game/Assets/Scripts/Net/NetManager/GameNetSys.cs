using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNetSys
{
    private static GameNetSys _instance;
    public static GameNetSys instance
    {
        get
        {
            if (_instance == null) _instance = new GameNetSys();
            return _instance;
        }
    }

    private GamePanel m_GamePanel;

    public void OnInit(GamePanel gamePanel) {
        m_GamePanel = gamePanel;
    }

    public void RspSaveInfoMsg(RspSaveInfoMsg msg)
    { 
        int code = msg.code;
        EventCenter.Broadcast<string>(EventType.TIPS, msg.msg); 
    }


}
