using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZButton : Selectable, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private bool m_IsDown = false;
    private bool m_IsLongPress = false;
    private float m_LastTime;
    private float m_LongPressTime = 0.4f;
     
    public Action onClick = null;
    public Action onLongPress = null;
    public Action onLongPressUp = null;
    private void Update()
    { 
        if (m_IsDown )
        {
            //Debug.Log(Time.time - m_LastTime + "  - " + m_LongPressTime);
            if (Time.time - m_LastTime > m_LongPressTime)
            { 
                LogTool.Log("LongPress");
                onLongPress?.Invoke();
                m_IsDown = false;
                m_IsLongPress = true;
            }
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_IsLongPress || interactable == false) return; 
        onClick?.Invoke();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        
        m_LastTime = Time.time;
        m_IsLongPress = false;
        m_IsDown = true;
        base.OnPointerDown(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
         
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        m_IsDown = false;
         
        if (m_IsLongPress) onLongPressUp?.Invoke();
        base.OnPointerExit(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        m_IsDown = false;
         
        if (m_IsLongPress) onLongPressUp?.Invoke();
        base.OnPointerUp(eventData);
    }
}
