using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class IReciveMsg 
{
    [JsonProperty]
    public int code;
    [JsonProperty]
    public string msg;
}


public enum ReciveMsgType { 

    RSP_LOGIN = 102,
    RSP_REGISTER = 104,
    RSP_SAVE_INFO = 108,
    RSP_REQUEST_SAVE_INFO = 110,
    RSP_LOGIN_QUIT = 112,
};

public class RspLoginMsg : IReciveMsg{ }
public class RspRegisterMsg : IReciveMsg{ }
public class RspSaveInfoMsg : IReciveMsg { }

[JsonObject(MemberSerialization.OptIn)]
public class RspRequestSaveInfoMsg : IReciveMsg
{
    [JsonProperty]
    private string LastMapIdx {
        set { _lastMapIdx = int.Parse(value); } 
    }
    [JsonProperty]
    private string LastPlayerInfo
    {
        set { 
            _lastPlayerInfo = JsonConvert.DeserializeObject<Player>(value);  
        }
    }
    public int _lastMapIdx;
    public Player _lastPlayerInfo;
}
public class RspLoginQuitMsg : IReciveMsg { }