using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtrributeGroup : MonoBehaviour
{
    private Text m_TextAtk;
    private Text m_TextDef;
    private Text m_TextCrit;
    private Text m_TextCritDamage;

    private void Awake()
    {
        m_TextAtk = transform.Find("Text_ATK").GetComponent<Text>();
        m_TextDef = transform.Find("Text_Def").GetComponent<Text>();
        m_TextCrit = transform.Find("Text_Crit").GetComponent<Text>();
        m_TextCritDamage = transform.Find("Text_CritDamage").GetComponent<Text>();
    }

    public void UpdateAtrribute(ICharacter character) {
        StateSystem stateSystem = character.GetStateSystem();
        m_TextAtk.text = ""+stateSystem.atk;
        m_TextDef.text = ""+stateSystem.def;
        m_TextCrit.text = "" + stateSystem.crit;
        m_TextCritDamage.text = "" + stateSystem.criticalDamage;
    }
}
