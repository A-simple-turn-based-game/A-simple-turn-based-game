using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : RecoverableObject
{
    private Image m_Icon = null;
    private Text m_Num;
    private PointerListener m_PointerListener;
    public Action<int> callBack;
    [HideInInspector]
    public int idx;

    private void OnInit() {
        m_Icon = transform.Find("Icon").GetComponent<Image>();
        m_Num = transform.Find("Num").GetComponent<Text>();
        m_PointerListener = gameObject.AddComponent<PointerListener>();
    }

    public void SetGridInfo(int _idx, Sprite sprite,int num) {
        if (m_Icon == null) OnInit();
        idx = _idx;
        m_Icon.sprite = sprite;
        m_Num.text = "" + num;
        m_PointerListener.onClick = (_) => { callBack(idx); };
    }
    public void SetGridInfo(int _idx,Sprite sprite)
    {
        if (m_Icon == null) OnInit();
        idx = _idx;
        m_Icon.sprite = sprite;
        m_Num.text = "";
        m_PointerListener.onClick = (_) => { callBack(idx); };
    }
}
