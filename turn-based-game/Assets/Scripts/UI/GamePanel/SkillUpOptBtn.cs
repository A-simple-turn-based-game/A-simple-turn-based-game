using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 
using UnityEngine.UI;

public class SkillUpOptBtn : MonoBehaviour
{
    private Button m_Btn;
    private Image m_Image;
    private Text m_TypeText;
    private Text m_Name;
    private Text m_Description;
    public void OnInit(string type,string name,string descritpion) {

        m_Btn = transform.GetComponent<Button>();
        m_TypeText = transform.Find("Type").GetComponent<Text>();
        m_Description = transform.Find("Description").GetComponent<Text>();
        m_Name = transform.Find("Icon/Name").GetComponent<Text>();
        m_Image = transform.Find("Icon/Mask/Icon").GetComponent<Image>();

        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.identity;

        m_TypeText.text = type;
        m_Name.text = name;
        m_Description.text = descritpion;
        if (m_Description.text.Contains(" "))
        {
            m_Description.text = m_Description.text.Replace(" ", "\u00A0");
        }
    
    }
    public void SetImage(Sprite img) {
        m_Image.sprite = img;
    }
    public void RegisterClickEvent(UnityAction action) { m_Btn.onClick.AddListener(action); }
}
