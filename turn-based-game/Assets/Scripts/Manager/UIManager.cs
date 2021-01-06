
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class UIManager : BaseManager
{
    private Transform m_Canvastrans = null;
 
    private Dictionary<UIPanelType, BasePanel> PanelDict = new Dictionary<UIPanelType, BasePanel>();//保存所有实例化面板的游戏物体身上的BasePanel组件
    private Stack<BasePanel> panelStack = new Stack<BasePanel>();
  
    public UIManager(GameRoot gameRoot) : base(gameRoot)
    {
 
    } 
    public override void OnInit()
    {
        base.OnInit();
        m_Canvastrans = GameObject.Find("Canvas").transform; 
    }
  
    //把页面入栈，并显示在界面上
    public BasePanel PushPanel(UIPanelType panelType)
    { 
        //判断一下栈中是否存在页面
        if (panelStack.Count > 0){ 
            BasePanel topPanel = panelStack.Peek();
            //打开新的界面把旧的界面暂停掉
            topPanel.OnPuase();
        }
        BasePanel panel = GetPanel(panelType);
        //场景打开时触发
        panel.OnEnter();
        panelStack.Push(panel);
        return panel;
    }

    //出栈，从界面上移除
    public void PopPanel()
    {
        //判断一下栈中是否存在页面
        if (panelStack.Count <= 0)
        {
            return;
        }

        BasePanel topPanel = panelStack.Pop();

        topPanel.OnExit();

        //判断是否还有页面
        if (panelStack.Count <= 0)
        {
            return;
        }

        //恢复活动
        BasePanel topPanel2 = panelStack.Peek();
        topPanel2.OnResume();

    }

    //根据面板类型，得到实例化面板 -------- 内部调用
    public BasePanel GetPanel(UIPanelType paneltype)
    {
         
        BasePanel panel = null;
        PanelDict.TryGetValue(paneltype,out panel);

        if (panel == null)
        {
            //使用扩展方法
            GameObject instPanel = ResFactory.instance.LoadUIPrefabs(Enum.GetName(typeof(UIPanelType), paneltype));
            instPanel.transform.SetParent(m_Canvastrans, false);
            instPanel.GetComponent<BasePanel>().uiManager = this;

            PanelDict.Add(paneltype, instPanel.GetComponent<BasePanel>());

            return instPanel.GetComponent<BasePanel>();
            //return null;
        }
        else
        { 
            return panel;
        }


    }
 
}
