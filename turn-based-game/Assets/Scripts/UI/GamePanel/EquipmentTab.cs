using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentTab : MonoBehaviour
{
    private Image m_Icon;
    private Text m_Name;
    private Text m_Description;
    private Transform m_Content; 
    internal void OnInit()
    {
        m_Icon = transform.Find("IconBorder/Icon").GetComponent<Image>();
        m_Name = transform.Find("Name").GetComponent<Text>();
        m_Description = transform.Find("Description/Viewport/Content").GetComponent<Text>();
        m_Content = transform.Find("Scroll View/Viewport/Content").GetComponent<Transform>();
        m_Description.text = "";
        m_Name.text = ""; 
    }
    private void UpdateEquipmentInfo(int id)
    {
        IEquipment equip = ResFactory.instance.GetEquipmentCfgById(id);
        m_Icon.sprite = ResFactory.instance.LoadItemIcon(equip.icon);
        m_Name.text = equip.name;
        m_Description.text = equip.GetEffectDescription();
    }
    internal void UpdateEquipmentsInfo(List<int> equipments)
    { 
        int cnt = equipments.Count;
        Grid[] grids = m_Content.GetComponentsInChildren<Grid>();
        int len = grids.Length;
        int maxv = Mathf.Max(len, cnt);
 

        for (int i = 0; i < maxv; i++)
        {
            Grid tmp = null;
            if (i < len && i < cnt)
            {
                tmp = grids[i];
                IEquipment equip = ResFactory.instance.GetEquipmentCfgById(equipments[i]);
                Sprite icon = ResFactory.instance.LoadItemIcon(equip.icon);
                tmp.SetGridInfo(equipments[i], icon);
                tmp.callBack = UpdateEquipmentInfo;
            }
            else if (i < cnt)
            {
                tmp = ResFactory.instance.LoadUIPrefabs("Grid").GetComponent<Grid>();
                tmp.transform.SetParent(m_Content, false);
                IEquipment equip = ResFactory.instance.GetEquipmentCfgById(equipments[i]);
                Sprite icon = ResFactory.instance.LoadItemIcon(equip.icon);
                tmp.SetGridInfo(equipments[i], icon);
                tmp.callBack = UpdateEquipmentInfo;
            }
            else
            {
                grids[i].OnRecycle();
            }
        }
        if (cnt != 0) UpdateEquipmentInfo(equipments[0]);
    }
}
