using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoGroup : MonoBehaviour
{
    private GamePanel m_GamePanel; 

    private Button m_MonsterTabBtn;
    private Button m_PlayerTabBtn;
    private Button m_SkillTabBtn;
    private Button m_EquipmentTabBtn;
    private Button m_PropTabBtn;
    private Button m_CloseBtn;
     
    private Text m_FirstTitleText;
    private Text m_SecondTitleText;

    private SkillsTab m_SkillsTab;
    private MonsterTab m_MonsterTab;
    private EquipmentTab m_EquipmentTab;
    private PropTab m_PropTab;
    private PlayerTab m_PlayerTab;

    private float m_TabPosX;
    private float m_OffsetPosX = 30;

    private GameObject m_LastTabBtn = null;
    private GameObject m_LastTab = null;
    public void OnInit(GamePanel gamePanel)
    {
        this.m_GamePanel = gamePanel;

        m_MonsterTabBtn = transform.Find("MonsterTab").GetComponent<Button>();
        m_PlayerTabBtn = transform.Find("PlayerTab").GetComponent<Button>();
        m_SkillTabBtn = transform.Find("SkillTab").GetComponent<Button>();
        m_EquipmentTabBtn = transform.Find("EquipmentTab").GetComponent<Button>();
        m_PropTabBtn = transform.Find("PropTab").GetComponent<Button>();

        m_CloseBtn = transform.Find("Content/Close").GetComponent<Button>();

        m_FirstTitleText = transform.Find("Content/FirstTitle/Text").GetComponent<Text>();
        m_SecondTitleText = transform.Find("Content/SecondTitle/Text").GetComponent<Text>();

        m_SkillsTab = transform.Find("Content/SkillsTab").GetComponent<SkillsTab>();
        m_SkillsTab.gameObject.SetActive(false);
        m_SkillsTab.OnInit();
        m_MonsterTab = transform.Find("Content/MonsterTab").GetComponent<MonsterTab>();
        m_MonsterTab.gameObject.SetActive(false);
        m_MonsterTab.OnInit();
        m_EquipmentTab = transform.Find("Content/EquipmentTab").GetComponent<EquipmentTab>();
        m_EquipmentTab.gameObject.SetActive(false);
        m_EquipmentTab.OnInit();
        m_PlayerTab = transform.Find("Content/PlayerTab").GetComponent<PlayerTab>();
        m_PlayerTab.gameObject.SetActive(false);
        m_PlayerTab.OnInit();
        m_PropTab = transform.Find("Content/PropTab").GetComponent<PropTab>();
        m_PropTab.gameObject.SetActive(false);
        m_PropTab.OnInit();
         
        m_TabPosX = m_SkillsTab.transform.localPosition.x;
         
        m_CloseBtn.onClick.AddListener(Close);
        m_SkillTabBtn.onClick.AddListener(SelectSkillsTab);
        m_PropTabBtn.onClick.AddListener(SelectPropTab);
        m_PlayerTabBtn.onClick.AddListener(SelectPlayerTab);
        m_MonsterTabBtn.onClick.AddListener(SelectMonsterTab);
        m_EquipmentTabBtn.onClick.AddListener(SelectEquipmentTab);
    }

    private void Close() { 
        if(m_LastTabBtn != null)
            m_LastTabBtn.transform.localPosition -= new Vector3(m_OffsetPosX, 0, 0);
        if(m_LastTab != null)
            m_LastTab.gameObject.SetActive(false);
        m_LastTab = null;
        m_LastTabBtn = null;
        this.gameObject.SetActive(false);
    }   

    private void TabBtnSelected(Button btn) {
        if (btn == m_LastTabBtn) return;
        if (m_LastTabBtn != null) { 
            Vector3 pos = m_LastTabBtn.transform.localPosition;
            m_LastTabBtn.transform.localPosition -= new Vector3(m_OffsetPosX, 0,0);
        }
        btn.transform.localPosition += new Vector3(m_OffsetPosX,0,0);
        
        m_LastTab?.gameObject.SetActive(false);
        m_LastTabBtn = btn.gameObject;
    }


    public void SelectSkillsTab() {
        TabBtnSelected(m_SkillTabBtn);
        m_LastTab = m_SkillsTab.gameObject;
        
        m_FirstTitleText.text = "技";
        m_SecondTitleText.text = "能";
        m_SkillsTab.gameObject.SetActive(true);

        m_SkillsTab.RegisterSkills(m_GamePanel.GetPlayer().GetAllSkills());
    }

    public void SelectMonsterTab() {
        TabBtnSelected(m_MonsterTabBtn);
        m_LastTab = m_MonsterTab.gameObject;
        
        m_FirstTitleText.text = "怪";
        m_SecondTitleText.text = "物";
        m_MonsterTab.gameObject.SetActive(true);


        m_MonsterTab.UpdateMonstersInfo(m_GamePanel.GetMonsters());
    }

    public void SelectPlayerTab() {
        TabBtnSelected(m_PlayerTabBtn);
        m_LastTab = m_PlayerTab.gameObject;

        m_FirstTitleText.text = "玩";
        m_SecondTitleText.text = "家";
        m_PlayerTab.gameObject.SetActive(true);
        m_PlayerTab.UpdatePlayerInfo(m_GamePanel.GetPlayer());
    }

    public void SelectPropTab() {
        TabBtnSelected(m_PropTabBtn);
        m_LastTab = m_PropTab.gameObject;

        m_FirstTitleText.text = "物";
        m_SecondTitleText.text = "品";
        m_PropTab.gameObject.SetActive(true);
        m_PropTab.UpdatePropsInfo(m_GamePanel.GetPlayer().GetPropSystem().props);
    }

    public void SelectEquipmentTab() {
        TabBtnSelected(m_EquipmentTabBtn);
        m_LastTab = m_EquipmentTab.gameObject;

        m_FirstTitleText.text = "装";
        m_SecondTitleText.text = "备";
        m_EquipmentTab.gameObject.SetActive(true);
        m_EquipmentTab.UpdateEquipmentsInfo(m_GamePanel.GetPlayer().GetEquipmentSystem().equipment);
    }
}
