using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleScrollRect : MonoBehaviour
{
    public Action<int, int> UpdateContent;

    private List<ItemBtn> m_Items;
     
    private List<int> m_IdxMap = new List<int>(); // 索引映射数组 

    private Transform m_CheckPos;
    private int m_ObjCnt = 30;

    public int equalNum = 10; // 几等分
    public float speed = 0.5f;
    public float radius = 265;
    public float minAbsPower = 5f; 
    public int upCheckIdx = 7;
    public int downCheckIdx = 0;
     
    private float m_EndPower;
    private float m_MaxRemainPower = 8f; //不可轻易调戏
    private float m_MinRemainPower = 1f;
    private float m_Frictional = 0.1f;

    private int m_ActiveNum; // 显示的数量
    private int m_EndDir = 1; // 结束时候的方向
    private float m_Angle;
    private bool m_IsDrag = false;

    
    private Vector2 m_LastPointPos;
    private Vector2 m_CurrPointPos;
       
    private PointerListener m_PL;

    public void OnInit(Transform checkPos, List<ItemBtn> itemBtns,List<int> idxMap)
    {
        this.m_CheckPos = checkPos;
        this.m_Items = itemBtns;
        this.m_IdxMap = idxMap;
        for (int i = 0; i < m_Items.Count; ++i)
        {
            idxMap.Add(i); 
        }
    }
    public void Refresh(int itemCnt) {
        m_ObjCnt = Mathf.Max( itemCnt,transform.childCount);
        
        for (int i = 0; i < m_Items.Count; ++i)
        { 
            m_IdxMap[i] = i; 
        } 
        m_ActiveNum = transform.childCount;   
        m_Angle = 360 / equalNum;
        for (int i = 0; i < m_ActiveNum; ++i)
        { 
            m_Items[i].transform.localPosition = Quaternion.Euler(0, 0, -m_Angle + m_Angle * -i) * Vector2.down * radius;
        }
          
        // 计算世界空间下的半径长度
        //m_RadiusInWorld = Vector2.Distance(items[0].position,transform.position);

        // cS
        //m_CheckPosX = transform.position.x + Mathf.Sin(m_Angle * Mathf.Deg2Rad) * m_RadiusInWorld;
    }
    

    private void Start()
    {
        transform.Rotate(Vector3.back  );

        m_PL = gameObject.AddComponent<PointerListener>();
        m_PL.onBeginDrag = (eventData)=>{
            m_IsDrag = false;
            m_LastPointPos = eventData.position;
        };

        m_PL.onDrag = (eventData) => {

            m_CurrPointPos = eventData.position;

            Vector2 dir = m_CurrPointPos - m_LastPointPos;
            if (Mathf.Abs(dir.y) > minAbsPower) {
                int singal = dir.y < 0 ? -1 : 1;
                float dealy = Mathf.Min(5, dir.y * singal) * singal;
                UpdateRotatation(dealy);
            }
           // LogTool.Log("    " + dir.x + "  " + dir.y);
            m_LastPointPos = m_CurrPointPos;
        };

        m_PL.onEndDrag = (eventData) => {

            m_CurrPointPos = eventData.position;
            float yval = m_CurrPointPos.y - m_LastPointPos.y;
            if (Mathf.Abs(yval) > m_MinRemainPower) { 
                m_IsDrag = true;
                m_EndDir = yval < 0 ? -1 : 1;
                m_EndPower = Mathf.Min(m_MaxRemainPower, yval * m_EndDir) * m_EndDir;
            }
        };
    }

    private void Update()
    {
        //LogTool.Log(transform.position + " posx :" + m_CheckPosX);
        //LogTool.Log(">>" + items[0].position + " posx :" + m_CheckPosX);
    }
    private void FixedUpdate()
    {
        if (m_IsDrag && Mathf.Abs(m_EndPower) > 0) { 
            
            m_EndPower -= m_EndDir * m_Frictional;
            if (m_EndPower * m_EndDir < 0) m_IsDrag = false;
            UpdateRotatation(m_EndPower  );
        }
        UpdatePos();

    }
     
    private void UpdateRotatation(float power) {
        power *= speed; 
         
        transform.Rotate(Vector3.back * power);
        for (int i = 0; i < m_ActiveNum; i++)
        { 
            m_Items[i].transform.Rotate(Vector3.forward * power); 
        }  
    }
     
    private void UpdatePos() { 

        for (int i = 0; i < m_ActiveNum; i++)
        { 
            Vector2 currPos = m_Items[i].transform.position;
            if (Mathf.Abs(currPos.x - m_CheckPos.position.x) < 0.2f)
            {
                //LogTool.Log(" >> " + items[i].GetComponentInChildren<Text>().text);
                if (currPos.y > m_CheckPos.position.y)
                {
                    int lastIdx = (i - 1 + m_ActiveNum) % m_ActiveNum;

                    if (m_IdxMap[i] != (1 + m_IdxMap[lastIdx]) % m_ObjCnt)
                    {
                        m_IdxMap[i] = (1 + m_IdxMap[lastIdx]) % m_ObjCnt;
                        UpdateContent?.Invoke(i, m_IdxMap[i]);
                    }
                }
                else
                {
                    int lastIdx = (i + 1 + m_ActiveNum) % m_ActiveNum;

                    if (m_IdxMap[i] != (m_IdxMap[lastIdx] - 1 + m_ObjCnt) % m_ObjCnt)
                    {
                        m_IdxMap[i] = (m_IdxMap[lastIdx] - 1 + m_ObjCnt) % m_ObjCnt;
                        UpdateContent?.Invoke(i, m_IdxMap[i]);
                    }
                }
            }

        }

    } 
}



//LogTool.Log(angleSum);
// 更新列表
//int signal = angleSum < 0 ? -1 : 1;
//while (angleSum*signal > m_Angle) {

//    if (signal > 0) {
//        int next = (downCheckIdx - 1 + m_ActiveNum ) % m_ActiveNum;
//        //LogTool.Log(next + " - " + downCheckIdx);
//        if ((idxMap[downCheckIdx] - 1 + m_ObjCnt) % m_ObjCnt != idxMap[next]) {
//            UpdateContent(next, (idxMap[downCheckIdx] - 1 + m_ObjCnt) % m_ObjCnt);
//        }
//        downCheckIdx = next;
//    }
//    else {
//        int next = (upCheckIdx + 1) % m_ActiveNum;
//        //LogTool.Log(">>" + next + " - " + upCheckIdx );
//        if ((idxMap[upCheckIdx] + 1) % m_ObjCnt != idxMap[next] ) {
//            UpdateContent(next, (idxMap[upCheckIdx] + 1) % m_ObjCnt);
//        }
//        upCheckIdx = next;
//    } 
//    angleSum -= m_Angle * signal;
//}