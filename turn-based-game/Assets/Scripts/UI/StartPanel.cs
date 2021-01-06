using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartPanel : BasePanel
{
    private Button m_BtnStart;
    private Button m_BtnLoginStart;
     
    [HideInInspector]
    public LoginGroup loginGroup;
    [HideInInspector]
    public RegisterGroup registerGroup;

    private void Awake() {

        // 初始化
        StartNetSys.instance.OnInit(this); 

        m_BtnStart = transform.Find("Btn_Start").GetComponent<Button>();
        m_BtnLoginStart = transform.Find("Btn_LoginStart").GetComponent<Button>();
        loginGroup = transform.Find("LoginPanel").GetComponent<LoginGroup>();
        registerGroup = transform.Find("RegisterPanel").GetComponent<RegisterGroup>();
        loginGroup.OnInit();
        registerGroup.OnInit();
        loginGroup.gameObject.SetActive(false);
        registerGroup.gameObject.SetActive(false);
    }


    private void Start()
    {

        m_BtnStart.onClick.AddListener(StartGame);

        m_BtnLoginStart.onClick.AddListener(()=> {
            loginGroup.gameObject.SetActive(true);
        });


        loginGroup.RegisterCloseEvent(() => { loginGroup.gameObject.SetActive(false); });
        loginGroup.RegisterRegisterEvent(() =>
        {
            loginGroup.gameObject.SetActive(false);
            registerGroup.gameObject.SetActive(true);
        });
        //EventCenter.Broadcast<string>(EventType.TIPS, "Start");
        loginGroup.RegisterLoginEvent(()=> {
            //        
            //EventCenter.Broadcast<string>(EventType.TIPS, "RegisterLoginEvent");
            SendLoginMsg sendLoginMsg = new SendLoginMsg
            {
                username = loginGroup.GetAccount(),
                password = loginGroup.GetPassword()
            };
            NetService.SendMsg(SendMsgType.SEND_LOGIN,sendLoginMsg);
        }); 

        registerGroup.RegisterCloseEvent(() =>
        {
            registerGroup.gameObject.SetActive(false);
            loginGroup.gameObject.SetActive(true);
        });

        registerGroup.RegisterRegisterEvent(() =>
        {
            SendRegisterMsg sendRegisterMsg = new SendRegisterMsg
            {
                username = registerGroup.GetAccount(),
                password = registerGroup.GetPassword()
            };
            NetService.SendMsg(SendMsgType.SEND_REGESTER,sendRegisterMsg);
        });
    }


    public void StartGame() {
        GameRoot.instance.LoadSceneAsync((int)Config.SCENETYPE.Lobby,()=> {
            Debug.Log("StartGame");
            uiManager.PushPanel(UIPanelType.LobbyPanel);
        }); 
    }



    public override void OnEnter()
    {
        base.OnEnter();
        GameRoot.instance.ResetCamera();
        GameRoot.instance.PlayBgSound(ResFactory.instance.LoadAudioClip(Config.STAET_BGM));
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
        gameObject.SetActive(false);
    }

    public override void OnResume()
    {
        base.OnResume();
        gameObject.SetActive(true);
    }
}
