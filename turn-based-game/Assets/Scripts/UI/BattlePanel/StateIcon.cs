using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateIcon : RecoverableObject
{
    private ZButton m_BtnIcon;
    private Image m_ICon;
    private RectTransform m_Description;
    private Text m_Content;
    private Text m_Count;
    private Text m_Name;
    private Text m_Round;
    private float m_OffsetY = 50f;
    private int m_Cnt;
 

    private void Awake()
    {
        m_BtnIcon = transform.GetComponent<ZButton>();
        m_ICon = transform.GetComponent<Image>();
        m_Description = transform.Find("Description").GetComponent<RectTransform>();
        m_Content = transform.Find("Description/Content").GetComponent<Text>();
        m_Count = transform.Find("Description/Count").GetComponent<Text>();
        m_Name = transform.Find("Description/Name").GetComponent<Text>();
        m_Round = transform.Find("Description/Round").GetComponent<Text>();
        m_Description.gameObject.SetActive(false); 
        //m_BtnIcon.longPressTime = 0.2f;
         
        m_BtnIcon.onLongPress = () =>
        {
            Vector3 vec = Camera.main.WorldToScreenPoint(transform.position);
      
            if (vec.y < Screen.height/2)
            {
                m_Description.localPosition =  new Vector3(0, m_OffsetY + m_Description.rect.height / 2);
            }
            else
            {
                m_Description.localPosition =  new Vector3(0, -m_OffsetY - m_Description.rect.height / 2);
            }
            m_Description.gameObject.SetActive(true);

        };

        m_BtnIcon.onLongPressUp = () => m_Description.gameObject.SetActive(false);
    }

    public void UpdateCnt()
    { 
        m_Count.text = "X" + ++m_Cnt;
    }

    public void UpdateRound(int round) {
        if (round == -1)
        {
            m_Round.text = "回合：N";
        }
        else
        {
            m_Round.text = "回合：" + round;
        }
    }

    public void OnInit(IBuff state, int round) {

        m_ICon.sprite = ResFactory.instance.LoadStateIcon(state.icon);
        m_Content.text = state.description;
        m_Count.text = "X 1";
        m_Cnt = 1;
        if (round  < 0)
        {
            m_Round.text = "回合：N";
        }
        else { 
            m_Round.text = "回合：" + round ;
        } 
        m_Name.text = state.name;
        gameObject.SetActive(true);
    }

    public override void OnGenerate()
    {
        base.OnGenerate();
        gameObject.SetActive(true);
    }

    public override void OnRecycle()
    {
        base.OnRecycle();
        //LogTool.Log("....");
        gameObject.SetActive(false);
    }
}
