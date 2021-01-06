using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerListener : MonoBehaviour , IPointerClickHandler, IPointerDownHandler,IPointerUpHandler,IDragHandler,IBeginDragHandler,IEndDragHandler,IPointerExitHandler
{

    public Action<object> onClick;
    public Action<PointerEventData> onClickDown;
    public Action<PointerEventData> onClickUp;
    public Action<PointerEventData> onDrag;
    public Action<PointerEventData> onBeginDrag;
    public Action<PointerEventData> onEndDrag;
    public Action<PointerEventData> onPointerExit;
    public object obj;

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    { 
        onDrag?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag?.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(obj);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onClickDown?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onClickUp?.Invoke(eventData);
    }

 
}
