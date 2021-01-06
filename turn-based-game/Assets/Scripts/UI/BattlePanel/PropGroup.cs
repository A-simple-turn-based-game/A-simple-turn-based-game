using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropGroup : MonoBehaviour
{
    private CircleScrollRect m_CircleScrollRect = null;
    private List<ItemBtn> m_Items = new List<ItemBtn>();
    private Button m_PropBackBtn;
    private RectTransform m_PropRotateBtn;
    private Transform m_CheckPos = null;
    private List<int> m_ItemId = new List<int>();
    // 索引映射数组 
    private List<int> m_IdxMap = new List<int>();
    // 实际物品个数
    private int m_ObjCnt;
    // 装备
    private Dictionary<int, int> m_Props;

    private bool m_CanBeClicked = true;

    public Action btnPropBackClicked = null;
    public Action<IProp> OnClick = null;
    public Action<IProp> OnLongPress = null;
    public Action OnLongPressUp = null;
    public Action OnBack;
    public void OnInit()
    {
        m_PropBackBtn = transform.Find("Back").GetComponent<Button>();
        m_PropRotateBtn = transform.Find("RotateBtn").GetComponent<RectTransform>();
        m_CircleScrollRect = transform.Find("RotateBtn").GetComponent<CircleScrollRect>();
        m_CheckPos = transform.Find("CheckPos").GetComponent<Transform>();
        for (int i = 0; i < m_CircleScrollRect.transform.childCount; ++i)
        {
            ItemBtn btn = m_CircleScrollRect.transform.GetChild(i).GetComponent<ItemBtn>();
            btn.OnInit(i);
            btn.OnClick = OnEquipmentBtnClick;
            btn.OnLongPress = OnPropBtnLongPress;
            btn.OnLongPressUp = ()=> OnLongPressUp?.Invoke();
            m_Items.Add(btn);
        }
        m_CircleScrollRect.OnInit(m_CheckPos, m_Items, m_IdxMap);
        m_CircleScrollRect.UpdateContent = UpdateContent;

        m_PropBackBtn.onClick.AddListener(() => DisAppear(OnBack));
    }
    private void OnEquipmentBtnClick(int itemIdx)
    {
        if (m_CanBeClicked == false) return;
        int idx = m_IdxMap[itemIdx];
        if (idx < m_ObjCnt)
        {
            int id = m_ItemId[idx];
            IProp prop = ResFactory.instance.GetPropCfgById(m_ItemId[idx]);
            OnClick?.Invoke(prop);
        }

    }
    private void OnPropBtnLongPress(int itemIdx)
    {
        int idx = m_IdxMap[itemIdx];
        if (idx < m_ObjCnt)
        {
            int id = m_ItemId[idx];
            IProp prop = ResFactory.instance.GetPropCfgById(m_ItemId[idx]);
            OnLongPress?.Invoke(prop);
        }
    }
    public void UpdateProp(Dictionary<int, int> props)
    { 
        m_ObjCnt = props.Count;
        m_Props = props;
        m_ItemId.Clear();
        foreach (KeyValuePair<int, int> item in props)
        {
            m_ItemId.Add(item.Key);
        }

        for (int i = 0; i < m_Items.Count; ++i)
        {
            if (i < m_ObjCnt)
            {
                IProp prop = ResFactory.instance.GetPropCfgById(m_ItemId[i]);
                Sprite icon = ResFactory.instance.LoadItemIcon(prop.icon);
                m_Items[i].SetBtnInfo(icon, m_Props[m_ItemId[i]]);
            }
            else
            {
                m_Items[i].Clear();
            }
        }
        m_CircleScrollRect.Refresh(m_ObjCnt);
    }
    


    private void UpdateContent(int itemIdx, int objIdx)
    {
        if (objIdx >= m_ObjCnt)
        {
            m_Items[itemIdx].Clear();
        }
        else
        {
            IEquipment equipment = ResFactory.instance.GetEquipmentCfgById(m_ItemId[objIdx]);
            Sprite icon = ResFactory.instance.LoadItemIcon(equipment.icon);
            m_Items[itemIdx].SetBtnInfo(icon, m_Props[m_ItemId[objIdx]]);
        }
    }

    public void ChangeInteractable(bool interactable)
    {
        m_CanBeClicked = interactable;
        m_PropBackBtn.interactable = interactable;
    }
    #region 动效
    public void OnReset()
    {
        m_PropRotateBtn.transform.localScale = Vector2.zero;
        m_PropBackBtn.transform.localScale = Vector2.zero;
    }

    public void Appear()
    {
        QTool.DOLocalPosAndScale(m_PropRotateBtn, m_PropRotateBtn.localPosition, new Vector2(1f, 1f));
        QTool.DOLocalPosAndScale(m_PropBackBtn.transform, m_PropBackBtn.transform.localPosition, new Vector2(1f, 1f));
    }

    public void DisAppear(Action callBack)
    {
        QTool.DOLocalPosAndScale(m_PropRotateBtn.transform, m_PropRotateBtn.localPosition, new Vector2(0f, 0f));
        QTool.DOLocalPosAndScale(m_PropBackBtn.transform, m_PropBackBtn.transform.localPosition, new Vector2(0f, 0f), callBack: ()=> {
            callBack?.Invoke();
            // 代表有物品已经用完了
            if (m_ObjCnt != m_Props.Count) {
                m_PropRotateBtn.transform.localScale = Vector2.one;
                m_PropBackBtn.transform.localScale = Vector2.one;
                UpdateProp(m_Props);
                m_PropRotateBtn.transform.localScale = Vector2.zero;
                m_PropBackBtn.transform.localScale = Vector2.zero;
            }
        });
    }

    public void UpdatePropCnt(int id, int cnt)
    {
        for (int i = 0; i < m_IdxMap.Count; i++)
        {
            if (m_ItemId[m_IdxMap[i]] == id)
            {
                m_Items[i].UpdateCount(cnt); 
                return;
            }
        }
    }
    #endregion
}
