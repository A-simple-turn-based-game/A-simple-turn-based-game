using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventOptBtn : MonoBehaviour
{
    public Action<int> onClickEvent;

    private Text m_ShortDescription;
    private Text m_Description;
    private Button m_Btn;
    private int m_Id;

    // Start is called before the first frame update
    public void OnInit(int id,string shortDescription,string longDescription)
    {
        m_ShortDescription = transform.Find("ShortDescription").GetComponent<Text>();
        m_Description = transform.Find("Description").GetComponent<Text>();
        m_Btn = transform.GetComponent<Button>();


        m_ShortDescription.text = shortDescription;
        m_Description.text = longDescription;
        m_Id = id;

        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.identity;

        m_Btn.onClick.AddListener(()=> onClickEvent?.Invoke(m_Id));
    } 
 
}
