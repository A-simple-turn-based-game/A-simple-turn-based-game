using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FingerListener : MonoBehaviour
{

    public bool isOnlyPointerObj = true;
    public Action<Vector2> onFingerSwipe = null;
    public Action<float> onFingerZoom = null;


    private Vector3 m_LastMousePos;  

    private Touch m_OldTouch1;
    private Touch m_OldTouch2;

    void Update()
    {
        OnFingerSwipe();
        OnFingerZoom(); 
    }


    void OnFingerSwipe() { 

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.Mouse0)) { 
            if (isOnlyPointerObj && QTool.IsOnUIElement() ) { return; } 
            m_LastMousePos = Input.mousePosition;
            return;
        } 
        if (Input.GetKey(KeyCode.Mouse0))
        { 
            if (isOnlyPointerObj && QTool.IsOnUIElement()  ) {
                m_LastMousePos = Input.mousePosition;
                return;
            }
            Vector2 vec = Input.mousePosition - m_LastMousePos;
            if (vec.magnitude < 0.1f) return;
            onFingerSwipe?.Invoke(vec);
            m_LastMousePos = Input.mousePosition;
        }

#elif UNITY_ANDROID
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved){
            if (isOnlyPointerObj && QTool.IsOnUIElement() ) { return; }
            Vector2 vec = Input.GetTouch(0).deltaPosition;
            onFingerSwipe?.Invoke(vec);
        } 

#endif

    }

    void OnFingerZoom() {

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            onFingerZoom?.Invoke( -Input.GetAxis("Mouse ScrollWheel"));
        }
#elif UNITY_ANDROID
        if (Input.touchCount == 2) {
            if (isOnlyPointerObj && QTool.IsOnUIElement()   ) { return; }

            Touch newTouch1 = Input.GetTouch(0); 
            Touch newTouch2 = Input.GetTouch(1);
            
            if (newTouch2.phase == TouchPhase.Began) {
                m_OldTouch1 = newTouch1;
                m_OldTouch2 = newTouch2;
                return;
            }
            float oldDistance = Vector2.Distance(m_OldTouch1.position,m_OldTouch2.position);
            float newDistance = Vector2.Distance(newTouch1.position,newTouch2.position);
            float offset = newDistance - oldDistance;
            onFingerZoom?.Invoke(-offset);
            m_OldTouch1 = newTouch1;
            m_OldTouch2 = newTouch2;
        }
#endif
    }

}
