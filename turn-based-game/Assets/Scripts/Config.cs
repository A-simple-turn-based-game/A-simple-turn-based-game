 
public class Config{ 
    public static readonly string IP = "127.0.0.1";
    public static readonly int PORT = 20001;
    public static readonly bool NET = true;


    public enum SCENETYPE {
        Entry,
        Start,
        Lobby,
        Game
    };

    public static readonly string AUDIO_PATH = "Audio/";

    public static readonly string MOSTER_PATH = "Prefabs/Monster/";
    public static readonly string MOSTER_BATTLE_MODEL_PATH = "Prefabs/Monster/Battle/";
    public static readonly string PLAYER_PATH = "Prefabs/Player/";
    public static readonly string PLAYER_BATTLE_MODEL_PATH = "Prefabs/Player/Battle/";
    public static readonly string FLOORBLOCK_PATH = "Prefabs/Tile/Floor/";
    public static readonly string WALLBLOCK_PATH  = "Prefabs/Tile/Wall/";
    public static readonly string BUILDING_PATH = "Prefabs/Tile/Building/";
    public static readonly string UI_PREFABS_PATH = "Prefabs/UI/";
    public static readonly string BATTLE_EFFECT_PATH = "Prefabs/Effect/Battle/";

    public static readonly string CHARACTER_ICON_PATH = "Images/Icon/Character/";
    public static readonly string SKILL_ICON_PATH = "Images/Icon/Skill/";
    public static readonly string STATE_ICON_PATH = "Images/Icon/State/";
    public static readonly string EVENT_ICON_PATH = "Images/Icon/Event/";
    public static readonly string ITEM_ICON_PATH = "Images/Icon/Item/";


    public static readonly string SKILLCFG_PATH = "Config/SkillCfg";
    public static readonly string BUFFCFG_PATH = "Config/BuffCfg";
    public static readonly string PLAYERCFG_PATH = "Config/PlayerCfg";
    public static readonly string MONSTERCFG_PATH = "Config/MonsterCfg";
    public static readonly string SKILLUPCFG_PATH = "Config/SkillUpCfg";
    public static readonly string EVENTCFG_PATH = "Config/EventCfg";
    public static readonly string LOADMAPCFG_PATH = "Config/MapCfg";
    public static readonly string PROPCFG_PATH = "Config/PropCfg";
    public static readonly string EQUIPMENTCFG_PATH = "Config/EquipmentCfg";

    public static readonly bool isLog = true;
    public static readonly bool isLogError = true;
    public static readonly bool isLogWarning = true;

    public static readonly bool isDebugMode = false;

    public static readonly float CAMERA_BATTLE_HIGHT = 1.6f;

    public static readonly float CAMERA_MIN_DISTANCE = 2;
    public static readonly float CAMERA_MAX_DISTANCE = 10;
    public static readonly float CAMERA_MAX_UP_ROTATION = 80;
    public static readonly float CAMERA_MIN_DOWN_ROTATION = -8;
    public static readonly float CAMERA_ZOOM_IN_SPEED = 8;
    public static readonly float CAMERA_LR_ROTATION_SPEED = 0.05f;
    public static readonly float CAMERA_UD_ROTATION_SPEED = 0.05f;
    public static readonly float CAMERA_ZOOM_IN_SPEED_Andriod = 0.01f;
    public static readonly float CAMERA_LR_ROTATION_SPEED_Andriod = 0.1f;
    public static readonly float CAMERA_UD_ROTATION_SPEED_Andriod = 0.1f;

    public static readonly float BLOCK_OFFSETX = 3.1f;
    public static readonly float BLCOK_OFFSETZ = -3.1f;
    public static readonly float CHARACTER_OFFSETY = 0.05f;

    public static readonly float ATK_DISTANCE = 1f;


    #region 音效

    public static readonly string STAET_BGM = "start";
    public static readonly string DEATH_BGM = "playerDeath";
    public static readonly string WIN_BGM = "playerWin";

    #endregion


    #region 时间配置
    /*
     
     public enum BattleProgress { 
    STARTFIGHT, // 战斗开始
    ROUNDTIPS,  // 回合提示
    HALFSTART,  // 半回合开始
    HALFMID,    // 半回合中
    HALFEND,    // 半回合结束
    ENDFIGHT,    // 战斗结束  
    NONE,
};
     */

    // 对应战斗阶段时间 
    public static readonly float[] BATTLE_WAITING_TIME = {0f,1.8f,0f,0.5f,0.5f,0f,2f };


    public static readonly float ATK_FLINCH_TIME = 1f;

    
    #endregion

}
