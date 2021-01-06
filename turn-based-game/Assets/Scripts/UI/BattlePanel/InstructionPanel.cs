using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InstructionPanel : MonoBehaviour
{

    private Image m_Icon;
    private Text m_Name;
    private Text m_Description;
    private Text m_Time;
    private Text m_Cost;
    private Vector3 m_Scale;

    private bool m_IsShowing = false;

    public void OnInit()
    {
        m_Icon = transform.Find("Icon/Image").GetComponent<Image>();
        m_Name = transform.Find("Name").GetComponent<Text>();
        m_Cost = transform.Find("Cost").GetComponent<Text>();
        m_Description = transform.Find("Description/Scroll View/Viewport/Text").GetComponent<Text>();
        m_Time = transform.Find("Time").GetComponent<Text>();
        m_Scale = transform.localScale;
        transform.localScale = new Vector3(0,0);
          
        gameObject.SetActive(false);
    }
    private void Start()
    {
    }
    public void ShowSkillInfo(ISkill skill) {
        Debug.Log("ddd");
        if (m_IsShowing) return;
        Debug.Log("sss");
        gameObject.SetActive(true);
        m_Icon.sprite = ResFactory.instance.LoadSkillIcon(skill.icon) as Sprite;
        m_IsShowing = true;
        m_Name.text = skill.name;
        m_Description.text = skill.description;

        if (m_Description.text.Contains(" "))
        {
            m_Description.text = m_Description.text.Replace(" ", "\u00A0");
        }
        //m_Cost.text = "MP : " + skill.mpCost;
        m_Time.text = "CD : " + skill.cd;
        transform.DOScale(m_Scale,0.2f);
    }
    public void ShowEquipmentInfo(IEquipment equipment)
    { 
        if (m_IsShowing) return;
        ISkill skill = equipment.Skill;
        gameObject.SetActive(true);
        m_Icon.sprite = ResFactory.instance.LoadItemIcon(equipment.icon) as Sprite;
        m_IsShowing = true;
        m_Name.text = equipment.name;
        m_Description.text = skill.description;

        if (m_Description.text.Contains(" "))
        {
            m_Description.text = m_Description.text.Replace(" ", "\u00A0");
        }
        //m_Cost.text = "MP : " + skill.mpCost;
        m_Time.text = "CD : " + skill.cd;
        transform.DOScale(m_Scale, 0.2f);
    }
    public void ShowPropInfo(IProp prop)
    { 
        if (m_IsShowing) return; 
        gameObject.SetActive(true);
        ISkill skill = prop.Skill;
        m_Icon.sprite = ResFactory.instance.LoadItemIcon(prop.icon) as Sprite;
        m_IsShowing = true;
        m_Name.text = prop.name;
        m_Description.text = prop.description;

        if (m_Description.text.Contains(" "))
        {
            m_Description.text = m_Description.text.Replace(" ", "\u00A0");
        }
        //m_Cost.text = "MP : " + skill.mpCost;
        m_Time.text = "";
        transform.DOScale(m_Scale, 0.2f);
    }
    public void Close() {
        
        if (m_IsShowing == false) return;

        transform.DOScale(new Vector3(0, 0), 0.2f).onComplete = () =>
        {
            gameObject.SetActive(false);
            m_IsShowing = false;
        };
    }
}
