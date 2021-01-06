using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleInfoViewer : MonoBehaviour
{
    private Text info;
    private void Awake()
    {
        info = transform.Find("Viewport/Content").GetComponent<Text>();
    }
    void Start()
    {
        EventCenter.AddListener<string>(EventType.BATTLEINFO,AddBattleInfo);
        EventCenter.AddListener(EventType.CLEAR_BATTLE_INFO, ClearInfo);
    }

    private void AddBattleInfo(string msg) {
        info.text += "\n" + msg;
    }
    private void ClearInfo( )
    {
        info.text = "";
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<string>(EventType.BATTLEINFO,AddBattleInfo);
        EventCenter.AddListener(EventType.CLEAR_BATTLE_INFO, ClearInfo);
    }
}
