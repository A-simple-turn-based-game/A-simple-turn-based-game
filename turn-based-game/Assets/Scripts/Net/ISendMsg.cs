using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SendMsgType { 
    SEND_LOGIN = 102,
    SEND_REGESTER = 104,
    SEND_SAVE_INFO = 108,
    SEND_REQUEST_SAVE_INFO = 110,
    SEND_LOGIN_QUIT = 112,
};

public class ISendMsg  
{ 
}

public class SendLoginMsg : ISendMsg { 
    public string username;
    public string password;
}

public class SendRegisterMsg : ISendMsg
{
    public string username;
    public string password;
}
/// <summary>
/// 发送存档信息
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public class SendSaveInfoMsg : ISendMsg {

    [JsonProperty]
    private string LastMapIdx
    { 
        get { return _lastMapIdx.ToString(); }
    }
    [JsonProperty]
    public string LastPlayerInfo
    { 
        get { return JsonConvert.SerializeObject(_lastPlayerInfo); }
    }
    public int _lastMapIdx;
    public Player _lastPlayerInfo;
}
/// <summary>
/// 发送请求存档信息
/// </summary>
public class SendRequestSaveInfoMsg : ISendMsg
{
 
}
public class SendLoginQuitMsg : ISendMsg
{

}