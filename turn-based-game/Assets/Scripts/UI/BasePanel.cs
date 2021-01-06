using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BasePanel : MonoBehaviour
{ 
 
    public UIManager uiManager { get; set; }
  
    //界面被显示出来
    public virtual void OnEnter()
    {

    } 
    //界面暂停
    public virtual void OnPuase()
    {

    }

    //界面继续
    public virtual void OnResume()
    {

    } 
    //界面退出
    public virtual void OnExit()
    {

    } 
}
