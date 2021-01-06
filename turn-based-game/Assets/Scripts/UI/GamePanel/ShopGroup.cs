using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopGroup : MonoBehaviour
{
    private GamePanel m_GamePanel;
    private Button m_BuyBtn;
    private Button m_RefreshBtn;
    private Button m_Close;
    private Text m_Description;
    private Text m_ItemInfo;
    private Transform m_Content;
    private ShopItemBtn[] m_ShopItemBtn;
    private ShopItemBtn m_LastBtn = null;
    private Dictionary<RarityType, List<TShopItem>> m_ShopItemDict = new Dictionary<RarityType, List<TShopItem>>();

     
    public void OnInit(GamePanel gamePanel)
    {
        this.m_GamePanel = gamePanel;
        Transform shop = transform.Find("Shop");
        m_BuyBtn = shop.Find("BuyBtn").GetComponent<Button>();
        m_RefreshBtn = shop.Find("RefreshBtn").GetComponent<Button>();
        m_Content = shop.Find("Content").GetComponent<Transform>();
        m_Description = shop.Find("DescriptionBG/Description").GetComponent<Text>();
        m_ItemInfo = shop.Find("AdditionBG/Text").GetComponent<Text>();
        m_Close = shop.Find("Close").GetComponent<Button>();
        m_ShopItemBtn = m_Content.GetComponentsInChildren<ShopItemBtn>();
        foreach (ShopItemBtn item in m_ShopItemBtn)
        {
            item.callBack = ClickCallBack;  
        } 
        m_Close.onClick.AddListener(() => this.gameObject.SetActive(false)); ;
        m_BuyBtn.onClick.AddListener(OnBuyBtnClicked);
        m_RefreshBtn.onClick.AddListener(OnRefreshBtnClicked);

        //Debug.LogWarning("初始化商店");
    }

    /// <summary>
    /// 初始化商店可以刷新的物品池
    /// </summary>
    /// <param name="player"></param>
    public void InitShop(Player player) {
        m_ShopItemDict.Clear();
        m_Description.text = "";
        m_ItemInfo.text = "";
        Dictionary<int,IEquipment> equipmentCfg = ResFactory.instance.GetAllEquipmentCfg();
        EquipmentSystem equipmentSystem = player.GetEquipmentSystem();
        List<int> equipment = equipmentSystem.equipment;
        foreach (KeyValuePair<int,IEquipment> cfg in equipmentCfg)
        {
            if (equipment.Contains(cfg.Key)) { continue; }
            if (!m_ShopItemDict.ContainsKey(cfg.Value.rarity)) { m_ShopItemDict.Add(cfg.Value.rarity, new List<TShopItem>()); }
            m_ShopItemDict[cfg.Value.rarity].Add(cfg.Value);
        }
        RefreshShop();
    }

    public void RefreshShop() {
        int n = 0;
        List<TShopItem> buffer = new List<TShopItem>();
        // SSR 1 个
        List<TShopItem> shopItems = m_ShopItemDict[RarityType.SSR];
        if (shopItems.Count != 0) { 
            int idx = QTool.GetRandomInt(0,shopItems.Count-1);
            m_ShopItemBtn[n++].SetShopItem(shopItems[idx]);
        }
        // SR 2 个
        shopItems = m_ShopItemDict[RarityType.SR];
        int len = Mathf.Min(2, shopItems.Count);
        for (int i = 0; i < len; i++)
        {  
            int idx = QTool.GetRandomInt(0, shopItems.Count - 1);
            m_ShopItemBtn[n++].SetShopItem(shopItems[idx]);
            buffer.Add(shopItems[idx]);
            shopItems.RemoveAt(idx); 
        }
        foreach (TShopItem item in buffer)
        {
            shopItems.Add(item);
        }
        buffer.Clear();
        // R 3 个
        shopItems = m_ShopItemDict[RarityType.R];
        len = Mathf.Min(3,shopItems.Count);
        for (int i = 0; i < len; i++)
        { 
            int idx = QTool.GetRandomInt(0, shopItems.Count - 1);
            m_ShopItemBtn[n++].SetShopItem(shopItems[idx]);
            buffer.Add(shopItems[idx]);
            shopItems.RemoveAt(idx);
        }
        foreach (TShopItem item in buffer)
        {
            shopItems.Add(item);
        }
        buffer.Clear();

        //清理剩余空间
        for (int i = n; i < m_ShopItemBtn.Length; i++)
        {
            m_ShopItemBtn[i].Clear();
        }
    }

    private void OnEnable()
    {
        if (m_LastBtn != null) m_LastBtn.SetSelectedBGActive(false);  
        m_LastBtn = null; 
    }
    private void OnDisable()
    {
        if(m_LastBtn != null) m_LastBtn.SetSelectedBGActive(false);
        m_LastBtn = null;
    }
    private void OnDestroy()
    {
        foreach (ShopItemBtn item in m_ShopItemBtn)
        {
            item.gameObject.SetActive(true);
        }
    }

    private void ClickCallBack(ShopItemBtn shopItemBtn,TShopItem shopItem) {
        if (m_LastBtn != null) {
            m_LastBtn.SetSelectedBGActive(false);
        }
        shopItemBtn.SetSelectedBGActive(true);

        m_Description.text = shopItem.GetDescription();
        m_ItemInfo.text = shopItem.GetEffectDescription();

        m_LastBtn = shopItemBtn;
    }
    private void OnRefreshBtnClicked() {
        if (m_GamePanel.GetPlayer().gold >= 50)
        {
            m_GamePanel.GetPlayer().ChangeGold(-50);
            RefreshShop();
            EventCenter.Broadcast(EventType.TIPS, "刷新成功！");
        }
        else {
            EventCenter.Broadcast(EventType.TIPS,"金钱不足！");
        }
    }
    private void OnBuyBtnClicked()
    {
        if (m_LastBtn == null) { 
            EventCenter.Broadcast(EventType.TIPS, "未选择！");
            return;
        }
        TShopItem shopItem = m_LastBtn.GetShopItem();
        int cost = shopItem.GetCost();
        if (m_LastBtn.EverBuy == true) {
            EventCenter.Broadcast(EventType.TIPS, "已购买！");
        }
        else if (m_GamePanel.GetPlayer().gold >= cost)
        {
            m_GamePanel.GetPlayer().ChangeGold(-cost);
            if (shopItem is IEquipment) {
                IEquipment equip = (IEquipment)shopItem;
                m_ShopItemDict[equip.rarity].Remove(equip);
                m_GamePanel.GetPlayer().AddEquipment((IEquipment)shopItem);
            }
            else {
                m_GamePanel.GetPlayer().AddProp((IProp)shopItem);
            }
            m_LastBtn.Bought();
            EventCenter.Broadcast(EventType.TIPS, "购买成功！");
        }
        else
        {
            EventCenter.Broadcast(EventType.TIPS, "金钱不足！");
        }

    }
}
