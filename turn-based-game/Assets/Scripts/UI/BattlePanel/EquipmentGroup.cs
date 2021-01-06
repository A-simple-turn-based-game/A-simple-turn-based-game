using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentGroup : MonoBehaviour
{
    private CircleScrollRect m_CircleScrollRect = null;
    private List<ItemBtn> m_Items = new List<ItemBtn>();
    private Button m_EquipmentBackBtn;
    private RectTransform m_EquipmentRotateBtn;
    private Transform m_CheckPos = null;
    private List<int> m_ItemId = new List<int>();
    // 索引映射数组 
    private List<int> m_IdxMap = new List<int>(); 
    // 实际物品个数
    private int m_ObjCnt;
    // 装备 cd
    private Dictionary<int, int> m_EquipmentsCD;

    private bool m_CanBeClicked = true;

    public Action btnSkillBackClicked = null;
    public Action<IEquipment> OnClick = null;
    public Action<IEquipment> OnLongPress = null;
    public Action OnLongPressUp = null;
    public Action OnBack;
    public void OnInit() {
        m_EquipmentBackBtn = transform.Find("Back").GetComponent<Button>();
        m_EquipmentRotateBtn = transform.Find("RotateBtn").GetComponent<RectTransform>();
        m_CircleScrollRect = transform.Find("RotateBtn").GetComponent<CircleScrollRect>();
        m_CheckPos = transform.Find("CheckPos").GetComponent<Transform>();
        for (int i = 0; i < m_CircleScrollRect.transform.childCount; ++i)
        {
            ItemBtn btn = m_CircleScrollRect.transform.GetChild(i).GetComponent<ItemBtn>();
            btn.OnInit(i);
            btn.OnClick = OnEquipmentBtnClick;
            btn.OnLongPress = OnEquipmentBtnLongPress;
            btn.OnLongPressUp = ()=> OnLongPressUp?.Invoke();
            m_Items.Add(btn);
        } 
        m_CircleScrollRect.OnInit(m_CheckPos,m_Items,m_IdxMap); 
        m_CircleScrollRect.UpdateContent = UpdateContent;
         
        m_EquipmentBackBtn.onClick.AddListener(() => DisAppear(OnBack));
    }
    private void OnEquipmentBtnClick(int itemIdx)
    {
        if (m_CanBeClicked == false) return;
        int idx = m_IdxMap[itemIdx];
        if (idx < m_ObjCnt)
        {
            int id = m_ItemId[idx];
            IEquipment equipment = ResFactory.instance.GetEquipmentCfgById(m_ItemId[idx]);
            OnClick?.Invoke(equipment); 
        }

    }
    private void OnEquipmentBtnLongPress(int itemIdx)
    {
        int idx = m_IdxMap[itemIdx];
        if (idx < m_ObjCnt)
        {
            int id = m_ItemId[idx]; 
            IEquipment equipment = ResFactory.instance.GetEquipmentCfgById(m_ItemId[idx]);
            OnLongPress?.Invoke(equipment); 
        }
    }
    public void UpdateEquipment(List<int> equipments) {

        m_ObjCnt = equipments.Count;
        m_EquipmentsCD = new Dictionary<int, int>();
        m_ItemId = equipments;

        for (int i = 0; i < m_Items.Count; ++i) {
            if (i < m_ObjCnt)
            {
                IEquipment equipment = ResFactory.instance.GetEquipmentCfgById(m_ItemId[i]);
                Sprite icon = ResFactory.instance.LoadItemIcon(equipment.icon);
                if (equipment.Skill.passive == false) {
                    m_Items[i].SetBtnInfo(icon);
                }
                else {
                    m_Items[i].SetBtnInfo(icon,"被动");
                }
            }
            else
            {
                m_Items[i].Clear();
            } 
        }
        m_CircleScrollRect.Refresh(m_ObjCnt);
    }
    private void UpdateContent(int itemIdx,int objIdx) {

        Debug.Log(itemIdx + " - " + objIdx + "  " + m_ObjCnt) ;
        if (objIdx >= m_ObjCnt)
        {
            m_Items[itemIdx].Clear();
        }
        else {
            IEquipment equipment = ResFactory.instance.GetEquipmentCfgById(m_ItemId[objIdx]);
            Sprite icon = ResFactory.instance.LoadItemIcon(equipment.icon);
            if (equipment.Skill.passive == false)
            {
                if (m_EquipmentsCD.ContainsKey(m_ItemId[objIdx]))
                {
                    m_Items[itemIdx].SetBtnInfo(icon, m_EquipmentsCD[m_ItemId[objIdx]]);
                }
                else {
                    m_Items[itemIdx].SetBtnInfo(icon);
                } 
            }
            else
            {
                m_Items[itemIdx].SetBtnInfo(icon, "被动");
            }
        }
    }

    public void ChangeInteractable(bool interactable)
    {
        m_CanBeClicked = interactable;
        m_EquipmentBackBtn.interactable = interactable;
    }
    #region 动效
    public void OnReset()
    {
        m_EquipmentRotateBtn.transform.localScale = Vector2.zero;
        m_EquipmentBackBtn.transform.localScale = Vector2.zero;
    }

    public void Appear()
    {
        QTool.DOLocalPosAndScale(m_EquipmentRotateBtn, m_EquipmentRotateBtn.localPosition, new Vector2(1f, 1f));
        QTool.DOLocalPosAndScale(m_EquipmentBackBtn.transform, m_EquipmentBackBtn.transform.localPosition, new Vector2(1f, 1f));
    }

    public void DisAppear(Action callBack)
    {
        QTool.DOLocalPosAndScale(m_EquipmentRotateBtn.transform, m_EquipmentRotateBtn.localPosition, new Vector2(0f, 0f));
        QTool.DOLocalPosAndScale(m_EquipmentBackBtn.transform, m_EquipmentBackBtn.transform.localPosition, new Vector2(0f, 0f), callBack: callBack);
    }

    public void UpdateEquipmentCD(int id, int cd)
    {
        for (int i = 0; i < m_IdxMap.Count; i++)
        {
            if (m_ItemId[m_IdxMap[i]] == id) {
                m_Items[i].UpdateCD(cd);
                m_EquipmentsCD[id] = cd; 
                return;
            }
        }
    }
    #endregion
}
