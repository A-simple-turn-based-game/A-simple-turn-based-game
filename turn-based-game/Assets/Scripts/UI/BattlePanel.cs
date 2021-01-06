 
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : BasePanel
{ 
    // 结束战斗回调
    public Action<ICharacter> endFightCallBack;
    // 使用技能回调
    public Func<ICharacter,ICharacter, ISkill,bool> useSkillCallBack;
    // 使用物品回调
    public Func<ICharacter, ICharacter, IProp,bool> usePropCallBack;
    // 使用装备回调
    public Func<ICharacter, ICharacter, IEquipment, bool> useEquipmentCallBack;
    // 攻击回调
    public Action<ICharacter, ICharacter> attackCallBack;

     
    // 角色相关 
    private Player m_Player;
    private Monster m_Monster;
   
    private StateSystem m_PlayerSys;
    private StateSystem m_MonsterSys;

    private ControllerSystem m_PMCtr;
    private ControllerSystem m_MMCtr;

    private InstructionPanel m_InstructPanel;

    private StateBox m_PlayerState;
    private StateBox m_MonsterState;

    private FingerListener m_FingerListener;

    private GameObject m_CameraFocus;

    private GameObject m_EffectTransfrom;

    private Transform m_ValueTrans;
    #region UI组件
    // Pos 


    private Vector3 m_PosSelectGroupMove; 

    private Vector3 m_PosBtnSkill;
    private Vector3 m_PosBtnEquipment;
    private Vector3 m_PosBtnAttack;
    private Vector3 m_PosBtnProp;
     
    // UI
    private AtrributeGroup m_PlayerAtrributeGroup;
    private AtrributeGroup m_MonsterAtrributeGroup;
    private Slider m_PlayerHp;
    private Text m_PlayerHpText;
    private Slider m_PlayerMp;
    private Text m_PlayerMpText;
    private Slider m_MonsterHp;
    private Text m_MonsterHpText;

    private Text m_MonsterName;

    private SkillNameEffect m_SkillNameEffect;

    private RectTransform m_WinPanel;
    private Text m_Exp;
    private Text m_Gold;

    private RectTransform m_LosePanel;
    private RectTransform m_RoundTips;
    private Text m_RoundTipsText;
    private RectTransform m_SelectGroup;
    private Button m_BtnAttack;
    private Button m_BtnSkill;
    private Button m_BtnProp;
    private Button m_BtnEquipment;
     
 
    private SkillGroup m_SkillGroup;  

    private PropGroup m_PropGroup; 

    private EquipmentGroup m_EquipmentGroup; 


    private Button m_Quit;

    #endregion

    private void Awake()
    {
        this.gameObject.SetActive(false);

        m_FingerListener = gameObject.AddComponent<FingerListener>();

        m_FingerListener.onFingerSwipe = GameRoot.instance.RoateLens;
        m_FingerListener.onFingerZoom = GameRoot.instance.Zoom;
         
        #region UI 绑定
        // 提示性UI 
        m_RoundTips = transform.Find("RoundTips").GetComponent<RectTransform>();
        m_RoundTipsText = m_RoundTips.Find("Text").GetComponent<Text>();
        m_WinPanel = transform.Find("WinPanel").GetComponent<RectTransform>();
        m_Exp = m_WinPanel.Find("Exp").GetComponent<Text>();
        m_Gold = m_WinPanel.Find("Gold").GetComponent<Text>();
        m_LosePanel = transform.Find("LosePanel").GetComponent<RectTransform>();
        m_InstructPanel = transform.Find("Instruction").GetComponent<InstructionPanel>();
        m_InstructPanel.OnInit(); 
        
        m_SkillNameEffect = transform.Find("SkillNameEffect").GetComponent<SkillNameEffect>();
        m_SkillNameEffect.OnInit();

        m_PlayerState = transform.Find("PlayerInfo/StateBox").GetComponent<StateBox>();
        m_MonsterState = transform.Find("MonsterInfo/StateBox").GetComponent<StateBox>();

        m_Quit = transform.Find("Btn_Quit").GetComponent<Button>();
        m_Quit.onClick.AddListener(Btn_Quit);

        // 角色状态栏
        m_PlayerHp = transform.Find("PlayerInfo/HpInfo/HpBox").GetComponent<Slider>();
        m_PlayerHpText = transform.Find("PlayerInfo/HpInfo/HpText").GetComponent<Text>();
        m_PlayerMp = transform.Find("PlayerInfo/MpInfo/MpBox").GetComponent<Slider>();
        m_PlayerMpText = transform.Find("PlayerInfo/MpInfo/MpText").GetComponent<Text>();
        m_MonsterHp = transform.Find("MonsterInfo/HpInfo/HpBox").GetComponent<Slider>();
        m_MonsterHpText = transform.Find("MonsterInfo/HpInfo/HpText").GetComponent<Text>();
        m_MonsterName = transform.Find("MonsterInfo/Name").GetComponent<Text>();
        m_PlayerAtrributeGroup = transform.Find("PlayerInfo/Atrribute").GetComponent<AtrributeGroup>();
        m_MonsterAtrributeGroup = transform.Find("MonsterAtrribute").GetComponent<AtrributeGroup>();


        // 注册 btn 事件
        m_SelectGroup = transform.Find("SelectGroup").GetComponent<RectTransform>();
        m_BtnAttack = m_SelectGroup.Find("Btn_Attack").GetComponent<Button>();
        m_BtnAttack.onClick.AddListener(Btn_Attack);
        m_BtnAttack.interactable = false;

        m_BtnProp = m_SelectGroup.Find("Btn_Prop").GetComponent<Button>();
        m_BtnProp.onClick.AddListener(Btn_Prop);
        m_BtnProp.interactable = false;

        m_BtnSkill = m_SelectGroup.Find("Btn_Skill").GetComponent<Button>();
        m_BtnSkill.onClick.AddListener(Btn_Skill);
        m_BtnSkill.interactable = false;

        m_BtnEquipment = m_SelectGroup.Find("Btn_Equipment").GetComponent<Button>();
        m_BtnEquipment.onClick.AddListener(Btn_Equipment);
        m_BtnEquipment.interactable = false;


        // 初始化SkillGroup技能组  
        m_SkillGroup = transform.Find("SkillGroup").GetComponent<SkillGroup>();
        m_SkillGroup.OnInit();
        m_SkillGroup.OnLongPress = m_InstructPanel.ShowSkillInfo;
        m_SkillGroup.OnLongPressUp = () => m_InstructPanel.Close();
        m_SkillGroup.OnClick = Btn_UseSkill;
        m_SkillGroup.OnBack = AppearSelectGroup;

        // 初始化PropGroup技能组 
        m_PropGroup = transform.Find("PropGroup").GetComponent<PropGroup>();
        m_PropGroup.OnInit();
        m_PropGroup.OnLongPress = m_InstructPanel.ShowPropInfo;
        m_PropGroup.OnLongPressUp = () => m_InstructPanel.Close();
        m_PropGroup.OnClick = Btn_UseProp;
        m_PropGroup.OnBack = AppearSelectGroup;

        // 初始化EquipmentGroup技能组 
        m_EquipmentGroup = transform.Find("EquipmentGroup").GetComponent<EquipmentGroup>();
        m_EquipmentGroup.OnInit();
        m_EquipmentGroup.OnLongPress = m_InstructPanel.ShowEquipmentInfo;
        m_EquipmentGroup.OnLongPressUp = () => m_InstructPanel.Close();
        m_EquipmentGroup.OnClick = Btn_UseEquipment;
        m_EquipmentGroup.OnBack = AppearSelectGroup;
         
        m_PosSelectGroupMove = m_SelectGroup.Find("BtnMovePos").GetComponent<RectTransform>().localPosition;
         
        m_PosBtnSkill = m_BtnSkill.transform.localPosition;
        m_PosBtnEquipment = m_BtnEquipment.transform.localPosition;
        m_PosBtnAttack = m_BtnAttack.transform.localPosition;
        m_PosBtnProp = m_BtnProp.transform.localPosition;

        m_ValueTrans = transform.Find("ValueContent");
        #endregion
    }


    #region BattlePanel 生命函数  
    public override void OnEnter()
    {
        base.OnEnter();
        OnInit();
        gameObject.SetActive(true); 
    } 
    public override void OnExit()
    {
        base.OnExit();
        gameObject.SetActive(false);
    }

    public override void OnPuase()
    {
        base.OnPuase();
    }

    public override void OnResume()
    {
        base.OnResume();
    }

    #endregion

    #region 点击事件


    private void Btn_Quit() {
        endFightCallBack?.Invoke(m_Player);
    }

 
    private void Btn_UseSkill(ISkill skill) {
        bool isSuccess = (bool)(useSkillCallBack?.Invoke(m_Player, m_Monster, skill));
        if (isSuccess == false) return;
        PlayerTurnEnd();
        
        m_SkillGroup.DisAppear(null); 
    }
    private void Btn_UseEquipment(IEquipment equipment)
    {
        bool isSuccess = (bool)(useEquipmentCallBack?.Invoke(m_Player, m_Monster, equipment));
        if (isSuccess == false) return;
        PlayerTurnEnd();

        m_EquipmentGroup.DisAppear(null);
    }
    private void Btn_UseProp(IProp prop)
    {
        bool isSuccess = (bool)(usePropCallBack?.Invoke(m_Player, m_Monster, prop));
        if (isSuccess == false) return;
        PlayerTurnEnd();

        m_PropGroup.DisAppear(null);
    }
    private void Btn_Attack() {
        PlayerTurnEnd();
        attackCallBack?.Invoke(m_Player,m_Monster);
        DisAppearSelectGroup(null);
    }
  
    private void Btn_Skill() {
        DisAppearSelectGroup(() =>
        {
            m_SkillGroup.Appear(); 
        });
    }
    private void Btn_Prop() {

        DisAppearSelectGroup(() =>{
            m_PropGroup.Appear();
        });
    }

    private void Btn_Equipment() {
        DisAppearSelectGroup(()=> {
            m_EquipmentGroup.Appear();
        });
        
    }

    private void OnInit() {

        m_RoundTips.gameObject.SetActive(false);
        m_WinPanel.gameObject.SetActive(false);
        m_LosePanel.gameObject.SetActive(false);

        m_SelectGroup.gameObject.SetActive(true);
        m_SkillGroup.gameObject.SetActive(true);
        m_PropGroup.gameObject.SetActive(true);
        m_EquipmentGroup.gameObject.SetActive(true);

        Vector2 es = new Vector2(0,0);
        Vector2 ts = new Vector2(1, 1);

        QTool.SetLocalPosAndLocalScale(m_BtnAttack.gameObject.transform,m_PosSelectGroupMove,es);
        QTool.SetLocalPosAndLocalScale(m_BtnProp.gameObject.transform, m_PosSelectGroupMove, es);
        QTool.SetLocalPosAndLocalScale(m_BtnSkill.gameObject.transform, m_PosSelectGroupMove, es);
        QTool.SetLocalPosAndLocalScale(m_BtnEquipment.gameObject.transform, m_PosSelectGroupMove, es);

        m_SkillGroup.OnReset();
        m_PropGroup.OnReset();
        m_EquipmentGroup.OnReset();  
    } 
 
    private void AppearSelectGroup() {

        QTool.DOLocalPosAndScale(m_BtnAttack.transform,m_PosBtnAttack,new Vector2(1f,1f));
        QTool.DOLocalPosAndScale(m_BtnProp.transform, m_PosBtnProp, new Vector2(1f, 1f));
        QTool.DOLocalPosAndScale(m_BtnSkill.transform, m_PosBtnSkill, new Vector2(1f, 1f));
        QTool.DOLocalPosAndScale(m_BtnEquipment.transform, m_PosBtnEquipment, new Vector2(1f, 1f)); 
    }

    private void DisAppearSelectGroup(Action callBack) {

        QTool.DOLocalPosAndScale(m_BtnAttack.transform, m_PosSelectGroupMove, new Vector2(0f, 0f));
        QTool.DOLocalPosAndScale(m_BtnProp.transform, m_PosSelectGroupMove, new Vector2(0f, 0f));
        QTool.DOLocalPosAndScale(m_BtnSkill.transform, m_PosSelectGroupMove, new Vector2(0f, 0f));
        QTool.DOLocalPosAndScale(m_BtnEquipment.transform, m_PosSelectGroupMove, new Vector2(0f, 0f),callBack:callBack); 
    }

    #endregion


    // 初始化角色
    public void InitCharacter(Player player,Monster monster) {

        m_Quit.interactable = false;

        m_EffectTransfrom = new GameObject("EffectTransfrom");

        // player 的 战斗预制体
        this.m_Player = player;
        StateSystem pss  = m_Player.GetStateSystem();
        this.m_PlayerSys = pss; 
        
        // 战斗控制器  
        m_PMCtr = player.GetControllerSystem();
        
        // monster 的  战斗预制体
        this.m_Monster = monster;
        StateSystem mss = m_Monster.GetStateSystem();
        this.m_MonsterSys = mss; 

        // 战斗控制器
        m_MMCtr = monster.GetControllerSystem();

        // .....

        // 玩家数据准备
        m_PlayerHp.maxValue = pss.maxHp.value;
        m_PlayerHp.value = pss.hp.value;
        m_PlayerMp.maxValue = pss.maxMp.value;
        m_PlayerMp.value = pss.mp.value;

        m_PlayerHpText.text = pss.hp + "/" + pss.maxHp;
        m_PlayerMpText.text = pss.mp + "/" + pss.maxMp;

        // 怪物数据准备
        m_MonsterHp.maxValue = mss.maxHp.value;
        m_MonsterHp.value = mss.hp.value;

        m_MonsterHpText.text = mss.hp + "/" + mss.maxHp;
        m_MonsterName.text = monster.name;

        // 加载技能数据 
        m_SkillGroup.UpdateSkills(m_Player.GetAllSkills());
        m_EquipmentGroup.UpdateEquipment(m_Player.GetEquipmentSystem().GetUseableEquipment());
        m_PropGroup.UpdateProp(m_Player.GetPropSystem().props);

        m_PlayerAtrributeGroup.UpdateAtrribute(m_Player);
        m_MonsterAtrributeGroup.UpdateAtrribute(m_Monster);


        // 初始化相机位置
         
        Vector3 playerPos = m_PMCtr.transform.position;
        Vector3 monsterPos = m_MMCtr.transform.position;


        if(m_CameraFocus == null) m_CameraFocus = new GameObject("CameraFoucs");
        m_CameraFocus.transform.position = new Vector3((playerPos.x + monsterPos.x)/2,Config.CAMERA_BATTLE_HIGHT,(playerPos.z + monsterPos.z)/2 );
        GameRoot.instance.SetCameraFollow(m_CameraFocus.transform);
        
        m_Quit.interactable = true;

        // 播放音乐
        if(mss.baseCfg.monsterType == MonsterType.Normal)
            GameRoot.instance.PlayBgSound(ResFactory.instance.LoadAudioClip(Global.cfg.battlebgm));
        else if (mss.baseCfg.monsterType == MonsterType.Boss)
            GameRoot.instance.PlayBgSound(ResFactory.instance.LoadAudioClip(Global.cfg.bossbgm));
    }

     

    #region UI显示控制

    public void ShowSkillNameEffect(ISkill skill) { m_SkillNameEffect.ShowSkillName(skill.name); }
    public void UpdateCharacterAtrribute() {
        m_PlayerAtrributeGroup.UpdateAtrribute(m_Player);
        m_MonsterAtrributeGroup.UpdateAtrribute(m_Monster); 
    }

    public void UpdateSkillCD(ISkill skill,int cd) {
        m_SkillGroup.UpdateSkillCD(skill,cd);
    }
    public void UpdateEquipmentCD(int id, int cd)
    {
        m_EquipmentGroup.UpdateEquipmentCD(id, cd);
    }
    public void UpdatePropCnt(int id, int cnt)
    {
        m_PropGroup.UpdatePropCnt(id, cnt);
    }
    public void UpdatePlayerAtrribute() {
        m_PlayerAtrributeGroup.UpdateAtrribute(m_Player);
    }

    public void LoadEffect(ICharacter user,ICharacter target,string effectName,ReleaseType releaseType)
    {
        GameObject effect = ResFactory.instance.LoadBattleEffect(effectName);
        effect.transform.SetParent(m_EffectTransfrom.transform);
        EffectsController ec = effect.GetComponent<EffectsController>();
        ec.releaseType = releaseType;
        ec.OnInit(user,target); 
    }


    public void AddBuff(ICharacter character, IBuff state ,int round  ) {

        if (character == m_Player)
        {
            m_PlayerState.AddBuff(state,round);
        }
        else
        {
            m_MonsterState.AddBuff(state,round);
        }
    }

    public void UpdateBuff(ICharacter character, IBuff state, int round)
    {

        if (character == m_Player)
        {
            m_PlayerState.UpdateBuff(state.id, round);
        }
        else
        {
            m_MonsterState.UpdateBuff(state.id, round);
        }
    }

    public void RemoveState(ICharacter character, IBuff state)
    {

        if (character == m_Player)
        {
            m_PlayerState.RemoveState(state);
        }
        else
        {
            m_MonsterState.RemoveState(state);
        }
    }

    public void ShowRoundTips(int roundNum)
    {
        LogTool.Log("roundTips");
        m_RoundTipsText.text = "Round " + roundNum;
        m_RoundTips.localPosition = new Vector3(Screen.width/2 + m_RoundTips.sizeDelta.x/2  + 200 ,0f ,m_RoundTips.position.z);
        m_RoundTips.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.Append(m_RoundTips.DOLocalMoveX(0f, 0.8f).SetEase(Ease.OutQuint));
        sequence.AppendInterval(0.2f);
        sequence.Append(m_RoundTips.DOLocalMoveX(-Screen.width / 2 - m_RoundTips.sizeDelta.x / 2 - 200, 0.8f).SetEase(Ease.InQuint));
       
    }


    public void ShowDamge(ICharacter target, Value damage) {

        m_FingerListener.enabled = false;

        Vector3 vec = Camera.main.WorldToScreenPoint(target.GetValuePos().position);
        vec.z = 0;
        vec.x -= Screen.width / 2;
        vec.y -= Screen.height / 2;

        ShowValue sv = ResFactory.instance.LoadUIPrefabs("Value").GetComponent<ShowValue>();
        sv.transform.SetParent(m_ValueTrans, false);
        sv.transform.localPosition = vec;

        sv.SetValue(damage, () =>{ m_FingerListener.enabled = true; });

        if (target == m_Player)
        {
            EventCenter.Broadcast<string>(EventType.BATTLEINFO,"对玩家：" + damage.meaningType + "  " + damage);
        }
        else { 
            EventCenter.Broadcast<string>(EventType.BATTLEINFO, "对怪兽：" + damage.meaningType + "  " + damage);
        } 
    }

    public void UpdateMp() {

        int remain = m_PlayerSys.mp.value;
        m_PlayerMpText.text = m_PlayerSys.mp + "/" + m_PlayerSys.maxMp;
        m_PlayerMp.DOValue(remain, 0.8f);
    }

    public void UpdateHp(ICharacter target,Action callBack)
    {
        //LogTool.Log(user.gameObject.name + " atk " + target.gameObject.name + " : " + damage);
        int remain = 0;
        if (target == m_Player)
        { 
            remain = Mathf.Max(0, m_PlayerSys.hp.value ); 
            m_PlayerHpText.text = remain + "/" + m_PlayerSys.maxHp; 
            if (remain == 0)
            {
                // TODO 死亡特效
                m_PlayerHp.DOValue(remain, 1f).onComplete = ()=> callBack?.Invoke();
                return;
            }
            else {
                m_PlayerHp.DOValue(remain, 0.8f);
            }  
        }
        else { 

            remain = Mathf.Max(0, m_MonsterSys.hp.value); 
            m_MonsterHpText.text = remain + "/" + m_MonsterHp.maxValue;
            if (remain == 0)
            {
                // TODO 死亡特效
                m_MonsterHp.DOValue(remain, 1f).onComplete = () => callBack?.Invoke();
                return;
            }
            else { 
                m_MonsterHp.DOValue(remain, 0.8f);
            
            } 
        } 
    }
    public void ShowWinPanel(Action callBack) {

        GameRoot.instance.PlayBgSound(ResFactory.instance.LoadAudioClip(Config.WIN_BGM));

        Color color = m_WinPanel.GetComponent<Image>().color;
        color.a = 0;
        m_WinPanel.GetComponent<Image>().color = color;
        m_WinPanel.gameObject.SetActive(true);

        m_Exp.text = "+" + m_Monster.exps[0];
        m_Gold.text = "+" + m_Monster.gold;
        m_WinPanel.GetComponent<Image>().DOFade(0.6f, 1.5f).onComplete =()=> {

            m_WinPanel.GetComponent<PointerListener>().onClick = (_) =>
            {
                m_WinPanel.GetComponent<PointerListener>().onClick = null;
                callBack?.Invoke();
            }; 
        };
    }

    public void ShowLosePanel(Action callBack) {

        GameRoot.instance.PlayBgSound(ResFactory.instance.LoadAudioClip(Config.DEATH_BGM));
        
        Color color = m_WinPanel.GetComponent<Image>().color;
        color.a = 0;
        m_WinPanel.GetComponent<Image>().color = color;
        m_LosePanel.gameObject.SetActive(true);
        m_LosePanel.GetComponent<Image>().DOFade(0.6f, 1.5f).onComplete = () => {

            m_LosePanel.GetComponent<PointerListener>().onClick = (_) =>
            {
                m_LosePanel.GetComponent<PointerListener>().onClick = null;
                callBack?.Invoke();
            };
        };
    }
    #endregion

    #region 回合关键阶段的更新
    public void PlayerTurnStart(int roundNum)
    {
        // TODO 更新技能cd

        // 解放UI
        AppearSelectGroup();

        m_BtnAttack.interactable = true;
        m_BtnProp.interactable = true;
        m_BtnEquipment.interactable = true;
        m_BtnSkill.interactable = true;

        m_SkillGroup.ChangeInteractable(true);
        m_PropGroup.ChangeInteractable(true);
        m_EquipmentGroup.ChangeInteractable(true);
    }

    public void PlayerTurnEnd()
    { 
        m_BtnAttack.interactable = false;
        m_BtnProp.interactable = false;
        m_BtnEquipment.interactable = false;
        m_BtnSkill.interactable = false;

        m_SkillGroup.ChangeInteractable(false);
        m_PropGroup.ChangeInteractable(false);
        m_EquipmentGroup.ChangeInteractable(false);
    }

    public void UpdateState(int roundNum,Action callBack) {

        LogTool.Log("回合结束，更新状态");
        callBack?.Invoke();
    }

    public void BattleEnd() {

        // TODO 打扫战场 
        GameRoot.instance.SetCameraFollow(m_PMCtr.lookPos); 
        m_MonsterState.ClearAllState();
        m_PlayerState.ClearAllState();
        m_SkillGroup.ClearAllSkillInfo();
         
        //清理特效文件
        EffectsController[] objs = m_EffectTransfrom.GetComponentsInChildren<EffectsController>();
        foreach (EffectsController obj in objs)
        { 
            Destroy(obj.gameObject);
        }

        EventCenter.Broadcast(EventType.CLEAR_BATTLE_INFO);
        uiManager.PopPanel();
    }
    #endregion

}
