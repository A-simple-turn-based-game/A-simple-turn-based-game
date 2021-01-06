using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyPanel : BasePanel
{
    private Button m_StartBtn;
    private Button m_ContinueBtn;
    private Button m_Quit;
     
    private void OnInit()
    { 
        m_StartBtn = transform.Find("RightPanel/StartBtn").GetComponent<Button>();
        m_ContinueBtn = transform.Find("RightPanel/ContinueBtn").GetComponent<Button>();
        m_Quit = transform.Find("UpPanel/Quit").GetComponent<Button>();
    }


    private void Start(){
    
        LobbyNetSys.instance.OnInit(this);
        m_StartBtn.onClick.AddListener(StartBtn); 
        m_Quit.onClick.AddListener(Quit);
        m_ContinueBtn.onClick.AddListener(ContinueBtn);
    }

    public void RspContinueGame(int mapIdx,Player player) {
        GameRoot.instance.LoadSceneAsync((int)Config.SCENETYPE.Game, () => {
            Debug.Log("LobbyPanel");
            GamePanel gamePanel = uiManager.PushPanel(UIPanelType.GamePanel) as GamePanel;
            gamePanel.ContinueGame(mapIdx,player);
        });
    }

    private void Quit() {
        GameRoot.instance.LoadSceneAsync((int)Config.SCENETYPE.Start, () => {
            //Debug.Log("LobbyPanel");

            if (Global.isOnlineLogin == true) {
                SendLoginQuitMsg sendLoginQuit = new SendLoginQuitMsg() { };
                NetService.SendMsg(SendMsgType.SEND_LOGIN_QUIT,sendLoginQuit);
            }

            uiManager.PopPanel();
            uiManager.PushPanel(UIPanelType.StartPanel);
            Global.isOnlineLogin = false;
        });
    }
    private void ContinueBtn() {
        SendRequestSaveInfoMsg sendRequestSaveInfoMsg = new SendRequestSaveInfoMsg() {
        };
        NetService.SendMsg(SendMsgType.SEND_REQUEST_SAVE_INFO,sendRequestSaveInfoMsg);
    }
    private void StartBtn() { 
        GameRoot.instance.LoadSceneAsync((int)Config.SCENETYPE.Game,()=> {
            Debug.Log("LobbyPanel");
            GamePanel gamePanel = uiManager.PushPanel(UIPanelType.GamePanel) as GamePanel;
            gamePanel.StartGame();
        }); 
    }

    public override void OnEnter()
    {
        base.OnEnter();

        GameRoot.instance.ResetCamera();
        gameObject.SetActive(true);
        OnInit();

        if (Global.isOnlineLogin == false) m_ContinueBtn.interactable = false;
        else m_ContinueBtn.interactable = true;
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
        LogTool.Log("LobbyResume");
        if (Global.isOnlineLogin == false) m_ContinueBtn.interactable = false;
        else m_ContinueBtn.interactable = true;
        GameRoot.instance.PlayBgSound(ResFactory.instance.LoadAudioClip(Config.STAET_BGM));
        GameRoot.instance.ResetCamera();
        gameObject.SetActive(true);
    }
}
