using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBtn : MonoBehaviour
{
    private ZButton m_Button;
    private Image m_Icon;
    private Text m_Info;
    private Text m_CD;
    private GameObject m_CDBG;
    private GameObject m_InfoBG;
    private GameObject m_Mask;

    public int idx;
    //public int cnt;
    public Action<int> OnClick;
    public Action<int> OnLongPress;
    public Action OnLongPressUp = null;

    public void OnInit(int idx) {
        this.idx = idx;
        m_Mask = transform.Find("Mask").gameObject;
        m_Mask.SetActive(false);
        m_InfoBG = m_Mask.transform.Find("InfoBG").gameObject;
        m_InfoBG.SetActive(false);
        m_CDBG = m_Mask.transform.Find("CDBG").gameObject;
        m_CDBG.SetActive(false);

        m_Icon = m_Mask.transform.Find("Icon").GetComponent<Image>();
        m_Info = m_InfoBG.transform.Find("Text").GetComponent<Text>();
        m_CD = m_CDBG.transform.Find("Text").GetComponent<Text>();
        
        m_Button = transform.GetComponent<ZButton>();
        m_Button.onClick = () => { OnClick?.Invoke(idx); };
        m_Button.onLongPress = () => { OnLongPress?.Invoke(idx); };
        m_Button.onLongPressUp = ()=> { OnLongPressUp?.Invoke(); };
        m_Button.interactable = false;
    }

    public void SetBtnInfo(Sprite sprite,int cnt) {
        m_Mask.SetActive(true);
        m_InfoBG.SetActive(true);
        m_CDBG.SetActive(false);
        m_Button.interactable = true;

        m_Icon.sprite = sprite;
        m_Info.text = cnt.ToString();
    }
    public void SetBtnInfo(Sprite sprite, string info)
    {
        m_Mask.SetActive(true);
        m_InfoBG.SetActive(true);
        m_CDBG.SetActive(false);
        m_Button.interactable = true;

        m_Icon.sprite = sprite;
        m_Info.text = info;
    }
    public void SetBtnInfo(Sprite sprite)
    { 
        m_Mask.SetActive(true);
        m_InfoBG.SetActive(false);
        m_CDBG.SetActive(false);
        m_Button.interactable = true;
        
        m_Icon.sprite = sprite;
    }

    public void UpdateCount(int cnt)
    {  
        m_Info.text = "" + cnt; 
    }

    public void UpdateCD(int cd)
    {
        if (cd == 0)
        {
            m_CDBG.SetActive(false);
            m_Button.interactable = true;
        }
        else
        {
            m_CDBG.gameObject.SetActive(true);
            m_Button.interactable = false;
            m_CD.text = "" + cd;
        }
    }

    public void Clear() {
        m_Mask.gameObject.SetActive(false);
        m_InfoBG.SetActive(false);
        m_CDBG.SetActive(false);
        m_Button.interactable = false;
        
    }

}
