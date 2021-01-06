using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDCanvas : MonoBehaviour
{
    private Transform m_ValuePos;
    private Image m_Tips;
    private Text m_Name;
    private void Awake()
    {
        m_ValuePos = transform.Find("ValuePos");
        m_Tips = transform.Find("Head/Tips").GetComponent<Image>();
        m_Tips.gameObject.SetActive(false);
        m_Name = transform.Find("Head/Name").GetComponent<Text>();
    } 
    private void Update()
    {
        Rotation();
    }
    public void RegisterName(string name) {
        this.m_Name.text = name;
    }
    public void FindPlayer() {
        this.m_Name.color = new Color(0.8f,0,0);
        m_Tips.gameObject.SetActive(true);
    }
    public void LossPlayer()
    {
        this.m_Name.color = new Color(1f, 1,1);
        m_Tips.gameObject.SetActive(false);
    }

    public Transform GetValuePos() { return m_ValuePos; }

    public void ShowValue(Value value)
    {
        ShowValue sv = ResFactory.instance.LoadUIPrefabs("Value").GetComponent<ShowValue>();
        sv.transform.SetParent(transform);
        sv.transform.localPosition = m_ValuePos.localPosition;
        sv.SetValue(value);
    }

    void Rotation()
    {
        this.transform.LookAt(Camera.main.transform); 
    }
}