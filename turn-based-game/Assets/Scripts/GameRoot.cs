using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRoot : MonoBehaviour
{
    public static GameRoot instance;

    private AudioManager m_AudioManager = null;
    private CharacterManager m_CharacterManager = null;
    private UIManager m_UIManager = null;
    private GameManager m_GameManager = null;
    private CameraManager m_CameraManager = null;
    private ResFactory m_ResFactory = null;

    private AsyncOperation m_Operation;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;

        ZMonoTimer timer = gameObject.AddComponent<ZMonoTimer>();
        
        QTool.OnInit();
        ZTimerSvc.OnInit(timer);
        NetService.OnInit();
        ResFactory.OnInit();
        m_ResFactory = ResFactory.instance;

        m_AudioManager = new AudioManager(this);
        m_GameManager = new GameManager(this);
        m_UIManager = new UIManager(this);
        m_CharacterManager = new CharacterManager(this);
        m_CameraManager = new CameraManager(this);

        m_AudioManager.OnInit();
        m_UIManager.OnInit();
        m_CameraManager.OnInit();
        m_GameManager.OnInit();
        m_CharacterManager.OnInit(); 
    
    }


    private void Start()
    {

        bool isSuccess =  ResFactory.instance.InitCfg();
        if (isSuccess == false) {
            EventCenter.Broadcast<string>(EventType.CFGERROR, "导表错误！");
            return ;
        } 
        
        if(Config.NET)NetService.Connect();

        LoadSceneAsync((int)Config.SCENETYPE.Start,()=> { 
            m_UIManager.PushPanel(UIPanelType.StartPanel);
        }); 
        //DemoTest();
    } 
    private void Update()
    {
        m_AudioManager.OnUpdate();
        m_GameManager.OnUpdate();
        m_UIManager.OnUpdate();
        m_CharacterManager.OnUpdate();
        m_CameraManager.OnUpdate();
        if (Config.NET) NetService.OnUpdate();
        //MouseInput();
    }

    private void OnDestroy()
    {
        m_AudioManager.OnDestroy();
        m_GameManager.OnDestroy();
        m_UIManager.OnDestroy();
        m_CharacterManager.OnDestroy();
        m_ResFactory.OnDestroy();
    }

    public void LoadSceneAsync(int idx, Action callBack = null) {

        StartCoroutine(ILoadSceneAsync(idx,callBack));
    }

    IEnumerator ILoadSceneAsync(int idx,Action callBack)
    {
        m_Operation = SceneManager.LoadSceneAsync(idx);
        yield return m_Operation; 
        callBack?.Invoke();
    }

   
    public void ResetCamera() { m_CameraManager.ResetCameraTransfrom(); }
    public void SetCameraFollow(Transform target) {
        m_CameraManager.SetFollowTarget(target);
    } 
    public void Zoom(float offset) {
        m_CameraManager.ZoomIn(offset);
    }
    public void RoateLens(Vector2 vec) {
        m_CameraManager.RotateLens(vec);
    }
    public void OpenRaycasterLayer(string layer) {
        m_CameraManager.OpenRaycasterLayer(layer);
    }
    public void CloseRaycasterLayer(string layer) {
        m_CameraManager.CloseRaycasterLayer(layer);
    }

    public void PlaySound(AudioSource audioClip, AudioClip clip, bool loop = false) { m_AudioManager.PlaySound(audioClip,clip,loop); }
    public void PlaySound(AudioClip clip, bool loop = false) { m_AudioManager.PlaySound(clip, loop); }
    public void PlayBgSound(AudioClip clip, bool loop = true) { m_AudioManager.PlayBgSound(clip,loop); }

    public void Fight(ICharacter player,ICharacter monster) { 
        BattlePanel battlePanel  = m_UIManager.PushPanel(UIPanelType.BattlePanel) as BattlePanel;
        m_GameManager.m_BattleSystem.OnInit(player as Player,monster as Monster,battlePanel); 
    } 
}
