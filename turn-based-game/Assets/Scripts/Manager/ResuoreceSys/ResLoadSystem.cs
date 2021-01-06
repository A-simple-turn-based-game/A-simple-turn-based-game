using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

public class ResLoadSystem
{
    /// <summary>
    /// 用于储存加载过的游戏物体
    /// </summary>
    private Dictionary<string, GameObject> m_GameObjectBuffer;

    private ObjectPool m_ObjectPool;


    public ResLoadSystem(ObjectPool objectPool) {
        m_GameObjectBuffer = new Dictionary<string, GameObject>();
        this.m_ObjectPool = objectPool;
    }

    private GameObject InstantiateGameObject(string path) {

        GameObject obj = null;
        if (m_GameObjectBuffer.ContainsKey(path))
        {
            obj = m_GameObjectBuffer[path];
        }
        else {
            object res = LoadObjectFromRes(path);
            if (res == null) return null;
            obj = res as GameObject;
        }
        // 如果标记为可回收物体，使用对象池加载
        if (obj.GetComponent<RecoverableObject>() != null)
        {
            return m_ObjectPool.Generate(obj, path);
        }
        else {
            return GameObject.Instantiate(obj);
        }
    }


    private System.Object LoadObjectFromRes(string path) { 
        System.Object obj = Resources.Load(path);
        if (obj == null) {
            LogTool.LogError("无法从路径：" + path + "加载资源");
        }
        return obj;
    }

  
    private string LoadObjectFromStreamingAssetsToString(string path)
    {
        string p = Application.streamingAssetsPath; 
        return File.ReadAllText(p + "\\" + path); 
    }
    private byte[] LoadObjectFromStreamingAssetsToBtye(string path)
    {
        string p = Application.streamingAssetsPath;
        return File.ReadAllBytes(p + "\\" + path);
    }
    private Sprite LoadSpriteFromRes(string path)
    {
        Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
        if (sprite == null)
        {
            LogTool.LogError("无法从路径：" + path + "加载资源");
        }
        return sprite;
    }
    private AudioClip LoadAudioFromRes(string path)
    {
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip == null)
        {
            LogTool.LogError("无法从路径：" + path + "加载资源");
        }
        return clip;
    }
    private Sprite LoadSpriteFromStreamingAssets(string path)
    { 
        byte[] bytes = LoadObjectFromStreamingAssetsToBtye(path);

        Texture2D texture = new Texture2D(300, 300);
        
        texture.LoadImage(bytes);//流数据转换成Texture2D
        //创建一个Sprite,以Texture2D对象为基础
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
         
        if (sprite == null)
        {
            LogTool.LogError("无法从路径：" + path + "加载资源");
        }
        return sprite;
    }


    #region 加载预制体
    public AudioClip LoadAudioClip(string name) {
        return LoadAudioFromRes(Config.AUDIO_PATH + name);
    }
    public GameObject LoadMoster(string name)
    {
        return InstantiateGameObject(Config.MOSTER_PATH + name);
    } 

    public GameObject LoadMosterBattleModel(string name)
    {
        return LoadObjectFromRes(Config.MOSTER_BATTLE_MODEL_PATH + name) as GameObject;
    }

    public GameObject LoadPlayer(string name)
    {
        return InstantiateGameObject(Config.PLAYER_PATH + name); ;
    }

    public GameObject LoadPlayerBattleModel(string name)
    {
        return LoadObjectFromRes(Config.PLAYER_BATTLE_MODEL_PATH + name) as GameObject;
    }

    public GameObject LoadFloorBlock(string name) {

        return InstantiateGameObject(Config.FLOORBLOCK_PATH + name);
    }

    internal GameObject LoadBuilding(string name)
    {
        return InstantiateGameObject(Config.BUILDING_PATH + name);
    }

    public GameObject LoadWallBlock(string name)
    {

        return InstantiateGameObject(Config.WALLBLOCK_PATH + name);
    }
    public GameObject LoadUIPrefabs(string name) {
        return InstantiateGameObject(Config.UI_PREFABS_PATH + name);
    }

    public GameObject LoadBattleEffect(string name) {
        return InstantiateGameObject(Config.BATTLE_EFFECT_PATH + name);
    }
    public Sprite LoadCharacterIcon(string name)
    {
        if (Config.isDebugMode)
        {
            return LoadSpriteFromStreamingAssets(Config.CHARACTER_ICON_PATH + name + ".png");
        }
        else
        {
            return LoadSpriteFromRes(Config.CHARACTER_ICON_PATH + name );
        }
    }

    public Sprite LoadSkillIcon(string name) {
        if (Config.isDebugMode)
        {
            return LoadSpriteFromStreamingAssets(Config.SKILL_ICON_PATH + name + ".png");
        }
        else
        {
            return LoadSpriteFromRes(Config.SKILL_ICON_PATH + name );
        }
        //return Resources.Load(Config.SKILL_ICON_PATH + name,typeof(Sprite)) as Sprite;
    }
    public Sprite LoadStateIcon(string name)
    {
        if (Config.isDebugMode)
        {
            return LoadSpriteFromStreamingAssets(Config.STATE_ICON_PATH + name + ".png");
        }
        else
        {
            return LoadSpriteFromRes(Config.STATE_ICON_PATH + name );
        }
        //return Resources.Load(Config.SKILL_ICON_PATH + name,typeof(Sprite)) as Sprite;
    }
    public Sprite LoadEventIcon(string name)
    {
        if (Config.isDebugMode)
        {
            return LoadSpriteFromStreamingAssets(Config.EVENT_ICON_PATH + name + ".png");
        }
        else { 
            return LoadSpriteFromRes(Config.EVENT_ICON_PATH + name );
        }
        //return Resources.Load(Config.SKILL_ICON_PATH + name,typeof(Sprite)) as Sprite;
    }
    public Sprite LoadItemIcon(string name)
    {
        if (Config.isDebugMode)
        {
            return LoadSpriteFromStreamingAssets(Config.ITEM_ICON_PATH + name + ".png");
        }
        else
        {
            return LoadSpriteFromRes(Config.ITEM_ICON_PATH + name );
        }
        //return Resources.Load(Config.SKILL_ICON_PATH + name,typeof(Sprite)) as Sprite;
    }
    #endregion


    #region 加载XML文件

    public XmlNodeList GetXmlNodeList(string path) {

        XmlDocument doc = new XmlDocument();
        string content = "";
        if (Config.isDebugMode) { 
            content = LoadObjectFromStreamingAssetsToString(path + ".xml");
        }
        else {  
            TextAsset xml = LoadObjectFromRes(path) as TextAsset;
            if (xml == null) {
                LogTool.LogError("xml文件" + path + " 路径无效");
            }
            content = xml.text; 
        }
          
        
        doc.LoadXml(content);
        
        
        return doc.SelectSingleNode("root").ChildNodes;
    }
    public Dictionary<int, ISkill> LoadSkillCfg() {
        Dictionary<int, ISkill> skillCfg = new Dictionary<int, ISkill>();
        XmlNodeList nodeList = GetXmlNodeList(Config.SKILLCFG_PATH);
        int len = nodeList.Count;
        for (int i = 0; i < len; ++i)
        {
            XmlElement ele = nodeList[i] as XmlElement;
            int id = Convert.ToInt32(ele.GetAttributeNode("id").InnerText);

            ISkill skill = new ISkill();
            skill.id = id;

            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "name":
                        skill.name = e.InnerText;
                        break;
                    case "description":
                        skill.description = e.InnerText;
                        break;
                    case "passive":
                        skill.passive = e.InnerText == "1";
                        break;
                    case "cd":
                        skill.cd = int.Parse(e.InnerText);
                        break;
                    case "icon":
                        skill.icon = e.InnerText;
                        break;
                    case "skillType":
                        skill.skillType = (SkillType)int.Parse(e.InnerText);
                        break;
                    case "maincfg":
                    case "additionalcfg": 
                        string content = e.InnerText;
                        if (string.IsNullOrWhiteSpace(content)) break;
                        content = content.Substring(1, content.Length - 2);
                        string[] info = Regex.Split(content, "},{", RegexOptions.IgnoreCase);
                        SkillCfgNode cfgNode = new SkillCfgNode();
                        skill.skillCfgNodes.Add(cfgNode);
                        foreach (string str in info)
                        {
                            string[] tmp = str.Split(',');
                            int n = int.Parse(tmp[0]);
                            Value val;
                            switch (n / 1000)
                            {
                                case 1:
                                    val = new Value();
                                    if (tmp.Length > 1) val.value = int.Parse(tmp[1]);
                                    cfgNode.skillJudge.Add((SkillJudge)n, val);
                                    break;
                                case 2:
                                    val = new Value();
                                    if (tmp.Length > 1) val.value = int.Parse(tmp[1]);
                                    cfgNode.skillCycleJudge.Add((SkillCycleJudge)n, val);
                                    break;
                                case 3:
                                    cfgNode.cycleType = (CycleType)n;
                                    break;
                                case 4:
                                    val = new Value();
                                    if (tmp.Length > 1) val.value = int.Parse(tmp[1]);
                                    cfgNode.skillAction.Add((SkillAction)n, val);
                                    break;
                                case 5:
                                    if (n == 5001) {
                                        cfgNode.targetBuff.Add(int.Parse(tmp[1]), int.Parse(tmp[2]));
                                    }
                                    else if (n == 5002) {
                                        cfgNode.userBuff.Add(int.Parse(tmp[1]), int.Parse(tmp[2]));
                                    }
                                    break;
                                case 6:
                                    cfgNode.effectName = tmp[1];
                                    cfgNode.releaseType = (ReleaseType)int.Parse(tmp[2]);
                                    cfgNode.effectTime = float.Parse(tmp[3]);
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            skillCfg.Add(id, skill);
        }
        return skillCfg;
    }

    public Dictionary<int, IBuff> LoadBuffCfg()
    {
        Dictionary<int, IBuff> buffCfg = new Dictionary<int, IBuff>();
        XmlNodeList nodeList = GetXmlNodeList(Config.BUFFCFG_PATH);
        int len = nodeList.Count;
        for (int i = 0; i < len; ++i)
        {
            XmlElement ele = nodeList[i] as XmlElement;
            int id = Convert.ToInt32(ele.GetAttributeNode("id").InnerText);

            IBuff buff = new IBuff();
            buff.id = id;
            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "name":
                        buff.name = e.InnerText;
                        break;
                    case "description":
                        buff.description = e.InnerText;
                        break; 
                    case "isDebuff":
                        buff.isDebuff = e.InnerText == "1";
                        break;
                    case "icon":
                        buff.icon = e.InnerText;
                        break; 
                    case "canBeDispelled":
                        buff.canBeDispelled = e.InnerText == "1";
                        break;
                    case "effectMode":
                        buff.effectMode = (BuffEffectMode)int.Parse(e.InnerText);
                        break;
                    case "roundMode":
                        buff.roundMode = (BuffRoundMode)int.Parse(e.InnerText);
                        break;
                    case "buffCfgNode1":
                    case "buffCfgNode2":
                    case "buffCfgNode3":
                        string content = e.InnerText;
                        if (string.IsNullOrWhiteSpace(content)) break;
                        content = content.Substring(1, content.Length - 2);
                        string[] info = Regex.Split(content, "},{", RegexOptions.IgnoreCase);
                        BuffCfgNode buffCfgNode = new BuffCfgNode(); 
                        foreach (string str in info)
                        {
                            string[] tmp = str.Split(',');
                            int n = int.Parse(tmp[0]);
                            Value val;
                            switch (n/1000)
                            {
                                case 1:
                                    val = new Value();
                                    if (tmp.Length > 1) val.value = int.Parse(tmp[1]);
                                    buffCfgNode.buffJudge.Add((BuffJudge)n, val);
                                    break;
                                case 2:
                                    val = new Value();
                                    if (tmp.Length > 1) val.value = int.Parse(tmp[1]);
                                    buffCfgNode.buffCycleJudge.Add((BuffCycleJudge)n, val);
                                    break;
                                case 3:
                                    val = new Value();
                                    if (tmp.Length > 1) val.value = int.Parse(tmp[1]);
                                    buffCfgNode.buffAction = (BuffAction)n;
                                    buffCfgNode.actionValue = val;
                                    break;
                                default:
                                    break;
                            } 
                        }
                        buff.buffCfgNode.Add(buffCfgNode);
                        break; 
                    default:
                        break;
                }
            }
            buffCfg.Add(id, buff);

        }
        return buffCfg;
    }

    public Dictionary<int, CharacterCfg> LoadPlayerCfg() {

        Dictionary<int, CharacterCfg> playerCfg = new Dictionary<int, CharacterCfg>();
        XmlNodeList nodeList = GetXmlNodeList(Config.PLAYERCFG_PATH);
        int len = nodeList.Count;
        for (int i = 0; i < len; ++i)
        {
            XmlElement ele = nodeList[i] as XmlElement;
            int id = Convert.ToInt32(ele.GetAttributeNode("id").InnerText);

            CharacterCfg characterCfg = new CharacterCfg();
            characterCfg.id = id;
            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "name":
                        characterCfg.name = e.InnerText;
                        break;
                    case "icon":
                        characterCfg.icon = e.InnerText;
                        break;
                    case "description":
                        characterCfg.description = e.InnerText;
                        break;
                    case "model":
                        characterCfg.model = e.InnerText;
                        break;
                    case "exp":
                        { 
                            List<int> exps = new List<int>();
                            string content = e.InnerText;
                            if (string.IsNullOrWhiteSpace(content)) break; 
                            string[] tmp = content.Split(',');
                            foreach (string t in tmp)
                            {
                                exps.Add(int.Parse(t));
                            }
                            characterCfg.exp = exps;
                        }
                        break;
                    case "gold":
                        characterCfg.gold = int.Parse(e.InnerText);
                        break;
                    case "hp":
                        characterCfg.hp = new Value(int.Parse(e.InnerText));
                        break;
                    case "mp":
                        characterCfg.mp = new Value(int.Parse(e.InnerText));
                        break;
                    case "atk":
                        characterCfg.atk = new Value(int.Parse(e.InnerText));
                        break;
                    case "def":
                        characterCfg.def = new Value(int.Parse(e.InnerText));
                        break;
                    case "crit":
                        characterCfg.crit = new Value(int.Parse(e.InnerText),valueType: ValueType.PERCENT);
                        break;
                    case "criticaldamage":
                        characterCfg.criticalDamage = new Value(int.Parse(e.InnerText), valueType: ValueType.PERCENT);
                        break;
                    case "skills":
                        {
                            List<int> skills = new List<int>();
                            string content = e.InnerText;
                            if (string.IsNullOrWhiteSpace(content)) break;
                            content = content.Substring(1, content.Length - 2); 
                            string[] tmp = content.Split(',');
                            foreach (string t in tmp)
                            {
                                skills.Add(int.Parse(t)); 
                            }
                            characterCfg.skills = skills;
                        } 
                        break;
                    case "AtkDamage_time":
                        {
                            characterCfg.atkDamage_time = float.Parse(e.InnerText);
                        }
                        break;
                    default:
                        { 
                            if (characterCfg.stateDelay == null) characterCfg.stateDelay = new Dictionary<CharacterState, float>();
                            string[] tmp = e.Name.Split('_');
                            CharacterState characterState = (CharacterState)Enum.Parse(typeof(CharacterState), tmp[0]);
                            characterCfg.stateDelay.Add(characterState,float.Parse(e.InnerText));
                        }
                        break;
                }
            }
            playerCfg.Add(id, characterCfg); 
        }
        return playerCfg;
    }
    public Dictionary<int, CharacterCfg> LoadMonsterCfg() {
        Dictionary<int, CharacterCfg> monsterCfg = new Dictionary<int, CharacterCfg>();
        XmlNodeList nodeList = GetXmlNodeList(Config.MONSTERCFG_PATH);
        int len = nodeList.Count;
        for (int i = 0; i < len; ++i)
        {
            XmlElement ele = nodeList[i] as XmlElement;
            int id = Convert.ToInt32(ele.GetAttributeNode("id").InnerText);

            CharacterCfg characterCfg = new CharacterCfg();
            characterCfg.id = id;
            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "name":
                        characterCfg.name = e.InnerText;
                        break;
                    case "icon":
                        characterCfg.icon = e.InnerText;
                        break;
                    case "description":
                        characterCfg.description = e.InnerText;
                        break;
                    case "monsterType":
                        characterCfg.monsterType = (MonsterType)int.Parse(e.InnerText);
                        break;
                    case "aiType":
                        characterCfg.aiType = (AIType)int.Parse(e.InnerText);
                        break;
                    case "model":
                        characterCfg.model = e.InnerText;
                        break;
                    case "exp":
                        {
                            List<int> exps = new List<int>();
                            string content = e.InnerText;
                            if (string.IsNullOrWhiteSpace(content)) break; 
                            string[] tmp = content.Split(',');
                            foreach (string t in tmp)
                            { 
                                exps.Add(int.Parse(t));
                            }
                            characterCfg.exp = exps;
                        }
                        break;
                    case "gold":
                        characterCfg.gold = int.Parse(e.InnerText);
                        break;
                    case "hp":
                        characterCfg.hp = new Value(int.Parse(e.InnerText));
                        break;
                    case "mp":
                        characterCfg.mp = new Value(int.Parse(e.InnerText));
                        break;
                    case "atk":
                        characterCfg.atk = new Value(int.Parse(e.InnerText));
                        break;
                    case "def":
                        characterCfg.def = new Value(int.Parse(e.InnerText));
                        break;
                    case "crit":
                        characterCfg.crit = new Value(int.Parse(e.InnerText),valueType: ValueType.PERCENT);
                        break;
                    case "criticaldamage":
                        characterCfg.criticalDamage = new Value(int.Parse(e.InnerText), valueType: ValueType.PERCENT);
                        break;
                    case "skills":
                        {
                            List<int> skills = new List<int>();
                            string content = e.InnerText;
                            if (string.IsNullOrWhiteSpace(content)) break;
                            content = content.Substring(1, content.Length - 2);
                            string[] tmp = content.Split(',');
                            foreach (string t in tmp)
                            {
                                skills.Add(int.Parse(t));
                            }
                            characterCfg.skills = skills;
                        }
                        break;
                    case "deadeventeffects":
                        {
                            List<List<Value>> effects = new List<List<Value>>();
                            string content = e.InnerText;
                            if (string.IsNullOrWhiteSpace(content)) break;
                            content = content.Substring(1, content.Length - 2);
                            string[] info = Regex.Split(content, "},{", RegexOptions.IgnoreCase);
                            foreach (string t in info)
                            {
                                string[] tmp = t.Split(',');
                                List<Value> eft = new List<Value>();
                                foreach (string p in tmp)
                                {
                                    eft.Add(new Value(int.Parse(p)));
                                }
                                effects.Add(eft);
                            }
                            characterCfg.deadEventEffects = effects;
                        }
                        break;
                    case "AtkDamage_time":
                        {
                            characterCfg.atkDamage_time = float.Parse(e.InnerText);
                        }
                        break;
                    default:
                        {
                            if (characterCfg.stateDelay == null) characterCfg.stateDelay = new Dictionary<CharacterState, float>();
                            string[] tmp = e.Name.Split('_');
                            CharacterState characterState = (CharacterState)Enum.Parse(typeof(CharacterState), tmp[0]);
                            characterCfg.stateDelay.Add(characterState, float.Parse(e.InnerText));
                        }
                        break;
                }
            }
            monsterCfg.Add(id, characterCfg);
        }
        return monsterCfg;
    }

    public Dictionary<int, Dictionary<int, SkillUpNode>> LoadSkillUpCfg() {

        Dictionary<int, Dictionary<int, SkillUpNode>> skillUpCfg = new Dictionary<int, Dictionary<int, SkillUpNode>>();
        XmlNodeList nodeList = GetXmlNodeList(Config.SKILLUPCFG_PATH);
        int len = nodeList.Count;
        for (int i = 0; i < len; ++i)
        {
            XmlElement ele = nodeList[i] as XmlElement;
            int id = Convert.ToInt32(ele.GetAttributeNode("id").InnerText);

            SkillUpNode node = new SkillUpNode();
            node.id = id; 
            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "name":
                        node.name = e.InnerText;
                        break;
                    case "skillid":
                        node.skillId = int.Parse( e.InnerText);
                        break;
                    case "description":
                        node.description = e.InnerText;
                        break;
                    case "propertychangedict":
                        { 
                            string content = e.InnerText;
                            if (string.IsNullOrWhiteSpace(content)) break;
                            content = content.Substring(1, content.Length - 2);
                            string[] info = Regex.Split(content, "},{", RegexOptions.IgnoreCase);
 
                            List<PropertyChangeType> propertyChange = new List<PropertyChangeType>();
                            List<List<Value>> changeValue = new List<List<Value>>();

                            foreach (string str in info)
                            {
                                string[] tmp = str.Split(',');
                                int n = int.Parse(tmp[0]);
                                PropertyChangeType propertyChangeType = (PropertyChangeType)n;
                                List<Value> values = new List<Value>();
                                int cnt = tmp.Length;
                                for (int m = 1; m < cnt; ++m) {
                                    values.Add(new Value(int.Parse(tmp[m])));
                                }
                                propertyChange.Add(propertyChangeType);
                                changeValue.Add(values); 
                            } 
                            node.propertyChanges = propertyChange;
                            node.changeValue = changeValue;
                        }
                        break; 
                    case "additionalcfg1":
                    case "additionalcfg2": 
                        { 
                            string content = e.InnerText;
                            if (string.IsNullOrWhiteSpace(content)) break;
                            content = content.Substring(1, content.Length - 2);
                            string[] info = Regex.Split(content, "},{", RegexOptions.IgnoreCase);
                            SkillCfgNode cfgNode = new SkillCfgNode();
                            node.cfgUpNode.Add(cfgNode);
                            foreach (string str in info)
                            {
                                string[] tmp = str.Split(',');
                                int n = int.Parse(tmp[0]);
                                Value val;
                                switch (n / 1000)
                                {
                                    case 1:
                                        val = new Value();
                                        if (tmp.Length > 1) val.value = int.Parse(tmp[1]);
                                        cfgNode.skillJudge.Add((SkillJudge)n, val);
                                        break;
                                    case 2:
                                        val = new Value();
                                        if (tmp.Length > 1) val.value = int.Parse(tmp[1]);
                                        cfgNode.skillCycleJudge.Add((SkillCycleJudge)n, val);
                                        break;
                                    case 3:
                                        cfgNode.cycleType = (CycleType)n;
                                        break;
                                    case 4:
                                        val = new Value();
                                        if (tmp.Length > 1) val.value = int.Parse(tmp[1]);
                                        cfgNode.skillAction.Add((SkillAction)n, val);
                                        break;
                                    case 5:
                                        if (n == 5001)
                                        {
                                            cfgNode.targetBuff.Add(int.Parse(tmp[1]), int.Parse(tmp[2]));
                                        }
                                        else if (n == 5002)
                                        {
                                            cfgNode.userBuff.Add(int.Parse(tmp[1]), int.Parse(tmp[2]));
                                        }
                                        break;
                                    case 6:
                                        cfgNode.effectName = tmp[1];
                                        cfgNode.effectTime = float.Parse(tmp[2]);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                } 
            }
            if (!skillUpCfg.ContainsKey(node.skillId)) { 
                skillUpCfg.Add(node.skillId, new Dictionary<int, SkillUpNode>());
            }
            skillUpCfg[node.skillId].Add(node.id,node);
        }
        return skillUpCfg; 
    }

    public Dictionary<int, IEvent> LoadEventCfg() {
        Dictionary<int, IEvent> eventCfg = new Dictionary<int, IEvent>();
        XmlNodeList nodeList = GetXmlNodeList(Config.EVENTCFG_PATH);
        int len = nodeList.Count;
        for (int i = 0; i < len; ++i)
        {
            XmlElement ele = nodeList[i] as XmlElement;
            int id = Convert.ToInt32(ele.GetAttributeNode("id").InnerText);

            IEvent ievent = new IEvent();
            ievent.id = id;
            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "name":
                        ievent.name = e.InnerText;
                        break;
                    case "description":
                        ievent.description = e.InnerText;
                        break;
                    case "icon":
                        ievent.icon = e.InnerText;
                        break;
                    case "type":
                        ievent.type = (GameEventType)int.Parse(e.InnerText);
                        break;
                    case "model":
                        ievent.model = e.InnerText;
                        break;
                    case "shortopts":
                        {
                            List<string> options = new List<string>();
                            string content = e.InnerText;
                            if (string.IsNullOrWhiteSpace(content)) break;
                            content = content.Substring(1, content.Length - 2);
                            string[] info = Regex.Split(content, "},{", RegexOptions.IgnoreCase);
                            foreach (string t in info)
                            {
                                options.Add(t);
                            }
                            ievent.shortOpts = options;
                        }
                        break;
                    case "options":
                        {
                            List<string > options = new List<string>();
                            string content = e.InnerText;
                            if (string.IsNullOrWhiteSpace(content)) break;
                            content = content.Substring(1, content.Length - 2);
                            string[] info = Regex.Split(content, "},{", RegexOptions.IgnoreCase);
                            foreach (string t in info)
                            {  
                                options.Add(t);
                            }
                            ievent.options = options;
                        }
                        break;
                    case "results":
                        {
                            List<string> results = new List<string>();
                            string content = e.InnerText;
                            if (string.IsNullOrWhiteSpace(content)) break;
                            content = content.Substring(1, content.Length - 2);
                            string[] info = Regex.Split(content, "},{", RegexOptions.IgnoreCase);
                            foreach (string t in info)
                            { 
                                results.Add(t);
                            }
                            ievent.results = results;
                        }
                        break;
                    case "effects":
                        {
                            List<List<List<Value>>> effects = new List<List<List<Value>>>();
                            string content = e.InnerText;
                            if (string.IsNullOrWhiteSpace(content)) break;
                            content = content.Substring(1, content.Length - 2);
                            string[] info = Regex.Split(content, "},{", RegexOptions.IgnoreCase);
                            foreach (string t in info)
                            {
                                List<List<Value>> effect = new List<List<Value>>();
                                effects.Add(effect);

                                if (string.IsNullOrWhiteSpace(t)) continue;

                                string[] tmp = t.Split('$');

                                foreach (string mm in tmp)
                                {
                                    List<Value> effectnode = new List<Value>();
                                    string[] ids = mm.Split(',');
                                    foreach (string n in ids)
                                    { 
                                        effectnode.Add(new Value(int.Parse(n)));
                                    }
                                    effect.Add(effectnode);
                                } 
                            }
                            ievent.effects = effects ;
                        }
                        break; 
                    default:
                        break;
                }
            }
            eventCfg.Add(id, ievent);
        }
        return eventCfg; 
    }
     
    public Dictionary<int,MapCfg> LoadMapCfg() {
        Dictionary<int, MapCfg> mapCfg = new Dictionary<int, MapCfg>();
        XmlNodeList nodeList = GetXmlNodeList(Config.LOADMAPCFG_PATH);
        int len = nodeList.Count;
        for (int i = 0; i < len; ++i)
        {
            XmlElement ele = nodeList[i] as XmlElement;
            int id = Convert.ToInt32(ele.GetAttributeNode("id").InnerText);

            MapCfg cfg = new MapCfg();
            cfg.id = id;
            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "name":
                        cfg.name = e.InnerText;
                        break;
                    case "row":
                        cfg.row = int.Parse( e.InnerText);
                        break;
                    case "col":
                        cfg.col = int.Parse( e.InnerText);
                        break;
                    case "description":
                        cfg.description = e.InnerText;
                        break;
                    case "model":
                        cfg.modelType = e.InnerText;
                        break;
                    case "monster":
                        {
                            Dictionary<int,int> monster = new Dictionary<int, int>();
                            string content = e.InnerText;
                            if (string.IsNullOrWhiteSpace(content)) break;
                            content = content.Substring(1, content.Length - 2);
                            string[] info = Regex.Split(content, "},{", RegexOptions.IgnoreCase);
                            foreach (string t in info)
                            {
                                string[] tmp = t.Split(',');
                                monster.Add(int.Parse(tmp[0]),int.Parse(tmp[1]));
                            }
                            cfg.monster = monster;
                        }
                        break;
                    case "event":
                        {
                            Dictionary<int,int> @event = new Dictionary<int, int>();
                            string content = e.InnerText;
                            if (string.IsNullOrWhiteSpace(content)) break;
                            content = content.Substring(1, content.Length - 2);
                            string[] info = Regex.Split(content, "},{", RegexOptions.IgnoreCase);
                            foreach (string t in info)
                            {
                                string[] tmp = t.Split(','); 
                                @event.Add(int.Parse(tmp[0]),int.Parse(tmp[1]));
                            }
                            cfg.@event = @event;
                        }
                        break;
                    case "nextmap":
                        {
                            string[] tmp = e.InnerText.Split(',');
                            foreach (string item in tmp)
                            {
                                cfg.nextMap.Add(int.Parse(item));
                            }
                        }
                        break;
                    case "bgm":
                         cfg.bgm = e.InnerText;
                        break;
                    case "bossbgm":
                        cfg.bossbgm = e.InnerText;
                        break;
                    case "normalbgm":
                        cfg.battlebgm = e.InnerText;
                        break;
                    default:
                        break;
                }
            }
            mapCfg.Add(id, cfg);
        }
        return mapCfg; 
    }
     
    public Dictionary<int, IEquipment> LoadEquipmentCfg()
    {
        Dictionary<int, IEquipment> equipmentCfg = new Dictionary<int, IEquipment>();
        XmlNodeList nodeList = GetXmlNodeList(Config.EQUIPMENTCFG_PATH);
        int len = nodeList.Count;
        for (int i = 0; i < len; ++i)
        {
            XmlElement ele = nodeList[i] as XmlElement;
            int id = Convert.ToInt32(ele.GetAttributeNode("id").InnerText);
            IEquipment equipment = new IEquipment();
            Dictionary<AdditionType, Value> addition = new Dictionary<AdditionType, Value>();
            equipment.addition = addition;
            equipment.id = id;

            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "name":
                        equipment.name = e.InnerText;
                        break;
                    case "description":
                        equipment.description = e.InnerText;
                        break;
                    case "cost":
                        equipment.cost = int.Parse(e.InnerText);
                        break;
                    case "skillId":
                        equipment.skillId = int.Parse(e.InnerText);
                        break;
                    case "icon":
                        equipment.icon = e.InnerText;
                        break;
                    case "rarity":
                        equipment.rarity = (RarityType)Enum.Parse(typeof(RarityType),e.InnerText);
                        break;
                    case "hp":
                        { 
                            if (e.InnerText.EndsWith("%")) { 
                                int val = int.Parse(e.InnerText.Substring(0,e.InnerText.Length-1));
                                addition.Add(AdditionType.HP_MAX_PCT,new Value(val,valueType: ValueType.PERCENT));
                            }
                            else {
                                int val = int.Parse(e.InnerText);
                                addition.Add(AdditionType.HP_MAX_OFFSET, new Value(val));
                            }
                        }
                        break;
                    case "mp":
                        {
                            if (e.InnerText.EndsWith("%")) {
                                int val = int.Parse(e.InnerText.Substring(0, e.InnerText.Length - 1));
                                addition.Add(AdditionType.MP_MAX_PCT, new Value(val, valueType: ValueType.PERCENT)); }
                            else { 
                                int val = int.Parse(e.InnerText);
                                addition.Add(AdditionType.MP_MAX_OFFSET, new Value(val)); }
                        }
                        break;
                    case "atk":
                        {
                            if (e.InnerText.EndsWith("%")) {
                                int val = int.Parse(e.InnerText.Substring(0, e.InnerText.Length - 1));
                                addition.Add(AdditionType.ATK_PCT, new Value(val, valueType: ValueType.PERCENT)); }
                            else { 
                                int val = int.Parse(e.InnerText);
                                addition.Add(AdditionType.ATK_OFFSET, new Value(val)); }
                        }
                        break;
                    case "def":
                        {
                            if (e.InnerText.EndsWith("%")) {
                                int val = int.Parse(e.InnerText.Substring(0, e.InnerText.Length - 1));
                                addition.Add(AdditionType.DEF_PCT, new Value(val, valueType: ValueType.PERCENT)); }
                            else { 
                                int val = int.Parse(e.InnerText);
                                addition.Add(AdditionType.DEF_OFFSET, new Value(val)); }
                        }
                        break;
                    case "crit":
                        {
                            if (e.InnerText.EndsWith("%")) {
                                int val = int.Parse(e.InnerText.Substring(0, e.InnerText.Length - 1));
                                addition.Add(AdditionType.CRIT_PCT, new Value(val, valueType: ValueType.PERCENT)); }
                            else {
                                int val = int.Parse(e.InnerText);
                                addition.Add(AdditionType.CRIT_OFFSET, new Value(val, valueType: ValueType.PERCENT)); }
                        }
                        break;
                    case "criticalDamage":
                        {
                            if (e.InnerText.EndsWith("%")) {
                                int val = int.Parse(e.InnerText.Substring(0, e.InnerText.Length - 1));
                                addition.Add(AdditionType.CRITCAL_DAMAGE_PCT, new Value(val, valueType: ValueType.PERCENT)); }
                            else { 
                                int val = int.Parse(e.InnerText);
                                addition.Add(AdditionType.CRITCAL_DAMAGE_OFFSET, new Value(val, valueType: ValueType.PERCENT)); }
                        }
                        break; 
                }
            }
            equipmentCfg.Add(id, equipment);
        }
        return equipmentCfg;
    }

    public Dictionary<int, IProp> LoadPropCfg()
    {
        Dictionary<int, IProp> propCfg = new Dictionary<int, IProp>();
        XmlNodeList nodeList = GetXmlNodeList(Config.PROPCFG_PATH);
        int len = nodeList.Count;
        for (int i = 0; i < len; ++i)
        {
            XmlElement ele = nodeList[i] as XmlElement;
            int id = Convert.ToInt32(ele.GetAttributeNode("id").InnerText);
            IProp prop = new IProp(); 
            prop.id = id;

            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "name":
                        prop.name = e.InnerText;
                        break;
                    case "description":
                        prop.description = e.InnerText;
                        break;
                    case "cost":
                        prop.cost = int.Parse(e.InnerText);
                        break;
                    case "skillId":
                        prop.skillId = int.Parse(e.InnerText);
                        break;
                    case "icon":
                        prop.icon = e.InnerText;
                        break;
                    case "rarity":
                        prop.rarity = (RarityType)Enum.Parse(typeof(RarityType), e.InnerText);
                        break;
                    default: 
                        break;
                }
            }
            propCfg.Add(id, prop);
        }
        return propCfg;
    }

    #endregion
}
