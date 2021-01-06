using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginGroup : MonoBehaviour
{
    private InputField m_AccountInputField;
    private InputField m_PassswordInputField;
    private Button m_RegisterBtn;
    private Button m_LoginBtn;
    private Button m_CloseBtn;

    public void OnInit() {
        m_AccountInputField = transform.Find("Account").GetComponent<InputField>();
        m_PassswordInputField = transform.Find("Password").GetComponent<InputField>();
        m_LoginBtn = transform.Find("LoginBtn").GetComponent<Button>();
        m_RegisterBtn = transform.Find("RegisterBtn").GetComponent<Button>();
        m_CloseBtn = transform.Find("CloseBtn").GetComponent<Button>();
 
    }

    public string GetAccount() {
        return m_AccountInputField.text;
    }
    public string GetPassword() {
        return m_PassswordInputField.text;
    }
    public void RegisterLoginEvent(UnityAction call) {
        m_LoginBtn.onClick.AddListener(call);
    }

    public void RegisterRegisterEvent(UnityAction call)
    {
        m_RegisterBtn.onClick.AddListener(call);
    }

    public void RegisterCloseEvent(UnityAction call) {
        m_CloseBtn.onClick.AddListener(call);
    }
 
}
