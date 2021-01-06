using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTab : MonoBehaviour
{
    private Image m_PlayerHead;
    private Text m_Name;
    private Text m_LV;
    private Text m_Gold;
    private Text m_Atk;
    private Text m_Def;
    private Text m_Crit;
    private Text m_CritDamage;
    private Text m_Description;
    public void OnInit()
    {
        m_PlayerHead = transform.Find("PlayerIcon/Icon").GetComponent<Image>();
        m_Name = transform.Find("PlayerInfo/Name/Text").GetComponent<Text>();
        m_LV = transform.Find("PlayerInfo/LV/Text").GetComponent<Text>();
        m_Gold = transform.Find("PlayerInfo/Gold/Text").GetComponent<Text>();
        m_Atk = transform.Find("PlayerInfo/Atk/Text").GetComponent<Text>();
        m_Def = transform.Find("PlayerInfo/Def/Text").GetComponent<Text>();
        m_Crit = transform.Find("PlayerInfo/Crit/Text").GetComponent<Text>();
        m_CritDamage = transform.Find("PlayerInfo/CritDamage/Text").GetComponent<Text>();
        m_Description = transform.Find("PlayerDescription/Text").GetComponent<Text>();
    }

    public void UpdatePlayerInfo(Player player) {
        player.RefreshState();
        m_Name.text = player.name;
        m_LV.text =""+ player.lv;
        m_Gold.text =""+ player.gold;
        StateSystem stateSystem = player.GetStateSystem();
        m_Atk.text = stateSystem.atk.ToString();
        m_Def.text = stateSystem.def.ToString();
        m_Crit.text = stateSystem.crit.ToString();
        m_CritDamage.text = stateSystem.criticalDamage.ToString();
        m_Description.text = player.description; 

    }

}
