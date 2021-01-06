using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropTab : MonoBehaviour
{
    private Image m_Icon;
    private Text m_Name;
    private Text m_Description;
    private Transform m_Content;
    private Dictionary<int, int> props;
    internal void OnInit()
    {
        m_Icon = transform.Find("IconBorder/Icon").GetComponent<Image>();
        m_Name = transform.Find("Name").GetComponent<Text>(); 
        m_Description = transform.Find("Description").GetComponent<Text>();
        m_Content = transform.Find("Scroll View/Viewport/Content").GetComponent<Transform>();
        m_Name.text = "";
        m_Description.text = "";
    }
    private void UpdatePropInfo(int id)
    {
        IProp prop = ResFactory.instance.GetPropCfgById(id);
        m_Icon.sprite = ResFactory.instance.LoadItemIcon(prop.icon);
        m_Name.text = prop.name; 
        m_Description.text = prop.description;
    }
    internal void UpdatePropsInfo(Dictionary<int,int> _props)
    {
        this.props = _props;
        int cnt = props.Count;
        Grid[] grids = m_Content.GetComponentsInChildren<Grid>();
        int len = grids.Length;
        int maxv = Mathf.Max(len, cnt);

        List<int> propId = new List<int>();
        foreach (KeyValuePair<int,int> prop in props)
        {
            propId.Add(prop.Key);
        }

        for (int i = 0; i < maxv; i++)
        {
            Grid tmp = null;
            if (i < len && i < cnt)
            {
                tmp = grids[i];
                IProp prop = ResFactory.instance.GetPropCfgById(propId[i]);
                Sprite icon = ResFactory.instance.LoadItemIcon(prop.icon);
                tmp.SetGridInfo(propId[i], icon,props[propId[i]]);
                tmp.callBack = UpdatePropInfo;
            }
            else if (i < cnt)
            {
                tmp = ResFactory.instance.LoadUIPrefabs("Grid").GetComponent<Grid>();
                tmp.transform.SetParent(m_Content, false);
                IProp prop = ResFactory.instance.GetPropCfgById(propId[i]);
                Sprite icon = ResFactory.instance.LoadItemIcon(prop.icon);
                tmp.SetGridInfo(propId[i], icon, props[propId[i]]);
                tmp.callBack = UpdatePropInfo;
            }
            else
            {
                grids[i].OnRecycle();
            }
        }
        if (cnt != 0) UpdatePropInfo(propId[0]);
    }
}
