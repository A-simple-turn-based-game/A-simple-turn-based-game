using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    private Image m_Icon = null; 
    private PointerListener m_PointerListener;
    public Action<int> callBack;
    [HideInInspector]
    public int idx;

    private void OnInit()
    {
        m_Icon = transform.Find("Mask/Icon").GetComponent<Image>(); 
        m_PointerListener = gameObject.AddComponent<PointerListener>();
    }

    public void SetBtnInfo(int _idx,Sprite sprite)
    {
        if (m_Icon == null) OnInit();
        idx = _idx;
        m_Icon.sprite = sprite; 
        m_PointerListener.onClick = (_) => { callBack(idx); };
    }
    public void ClearGridInfo(Sprite sprite)
    {
        if (m_Icon == null) OnInit();
        m_Icon.sprite = sprite; 
        m_PointerListener.onClick = null;
    }
}
