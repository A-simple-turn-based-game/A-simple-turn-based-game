using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemBtn : MonoBehaviour
{
    private Image m_Icon = null;
    private GameObject m_SelectedBG;
    private GameObject m_Bought;
    private Text m_Gold;
   
    private PointerListener m_PointerListener;
    private TShopItem m_ShopItem;

    public bool EverBuy = false;

    public Action<ShopItemBtn,TShopItem> callBack;
    
    public void OnInit() {
        m_SelectedBG = transform.Find("SelectBG").gameObject;
        m_Bought = transform.Find("Bought").gameObject;
        m_Icon = transform.Find("Icon").GetComponent<Image>();
        m_Gold = transform.Find("Gold/Text").GetComponent<Text>();
        m_PointerListener = gameObject.AddComponent<PointerListener>();
        m_PointerListener.onClick = (_) => { callBack(this,m_ShopItem); };
    }
    public TShopItem GetShopItem() { return m_ShopItem; }
    public void SetSelectedBGActive(bool active) {
        m_SelectedBG.gameObject.SetActive(active);
    }
    public void SetShopItem(TShopItem shopItem)
    {
        if (m_Icon == null) OnInit();

        this.gameObject.SetActive(true);
        this.EverBuy = false;
        this.m_ShopItem = shopItem;

        m_Icon.sprite = ResFactory.instance.LoadItemIcon(shopItem.GetIcon());
        m_Gold.text = ""+shopItem.GetCost();
        
        m_SelectedBG.gameObject.SetActive(false);
        m_Bought.gameObject.SetActive(false);
    }
    public void Clear() {
        this.gameObject.SetActive(false);
    } 

    public void Bought()
    {
        this.EverBuy = true;
        m_Bought.SetActive(true);
    }
}
