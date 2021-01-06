using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsTab : MonoBehaviour
{
    private Text m_Name;
    private Text m_Description;
    private SkillBtn[] m_SkillBtns; 
     
    public void OnInit()
    {
        m_Name = transform.Find("NameBG/Name").GetComponent<Text>();
        m_Description = transform.Find("Description/Scroll View/Viewport/Text").GetComponent<Text>(); 
        m_SkillBtns = transform.Find("Skills").GetComponentsInChildren<SkillBtn>(); 
    }
  
    public void RegisterSkills(List<ISkill> skills) {

        int cnt = m_SkillBtns.Length;
        int scnt = skills.Count;
        for (int i = 0; i < cnt; i++)
        {
            if (i < scnt)
            {
                Sprite sprite = ResFactory.instance.LoadSkillIcon(skills[i].icon); 
                m_SkillBtns[i].SetBtnInfo(i,sprite);
                m_SkillBtns[i].callBack = (idx) =>
                {
                    m_Name.text = skills[idx].name;
                    m_Description.text = skills[idx].description; 
                }; 
            }
            else {
                Sprite sprite = ResFactory.instance.LoadSkillIcon("icon_null");
                m_SkillBtns[i].ClearGridInfo(sprite);
            }
        }

        if (scnt != 0) {
            m_Name.text = skills[0].name;
            m_Description.text = skills[0].description;
        }
    }

}
