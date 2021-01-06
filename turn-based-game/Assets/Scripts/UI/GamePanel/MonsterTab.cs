using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterTab : MonoBehaviour
{
    private Image m_MonsterIcon;
    private Text m_Description;
    private Text m_Name;
    private Text m_Exp;
    private Text m_Gold;
    private Text m_Atk;
    private Text m_HP;
    private Text m_Def;
    private Text m_Crit;
    private Text m_CritDamage;
    private Transform m_Content;
    internal void OnInit()
    {
        m_MonsterIcon = transform.Find("MonsterIcon/Icon").GetComponent<Image>();
        m_HP = transform.Find("MonsterInfo/Hp/Text").GetComponent<Text>();
        m_Name = transform.Find("MonsterInfo/Name/Text").GetComponent<Text>();
        m_Exp = transform.Find("MonsterInfo/Exp/Text").GetComponent<Text>();
        m_Gold = transform.Find("MonsterInfo/Gold/Text").GetComponent<Text>();
        m_Atk = transform.Find("MonsterInfo/Atk/Text").GetComponent<Text>();
        m_Def = transform.Find("MonsterInfo/Def/Text").GetComponent<Text>();
        m_Crit = transform.Find("MonsterInfo/Crit/Text").GetComponent<Text>();
        m_CritDamage = transform.Find("MonsterInfo/CritDamage/Text").GetComponent<Text>();
        m_Description = transform.Find("MonsterDescription/Text").GetComponent<Text>();
        m_Content = transform.Find("Scroll View/Viewport/Content").GetComponent<Transform>();
    }

    private void UpdateMonsterInfo(int cnt) {
        CharacterCfg monster = ResFactory.instance.GetMonsterCfgById(cnt); 
        m_Name.text = monster.name;
        m_HP.text = monster.hp.ToString();
        m_Exp.text = monster.exp[0].ToString();
        m_Gold.text = monster.gold.ToString(); 
        m_Atk.text = monster.atk.ToString();
        m_Def.text = monster.def.ToString();
        m_Crit.text = monster.crit.ToString();
        m_CritDamage.text = monster.criticalDamage.ToString();
        m_Description.text = monster.description;
        m_MonsterIcon.sprite = ResFactory.instance.LoadCharacterIcon(monster.icon);
    }
    internal void UpdateMonstersInfo(List<int> monsters)
    {
        int cnt = monsters.Count;
        Grid[] grids = m_Content.GetComponentsInChildren<Grid>();
        int len = grids.Length;
        int maxv = Mathf.Max(len, cnt);
        for (int i = 0; i < maxv; i++)
        {
            Grid tmp = null;
            if (i < len && i < cnt)
            {
                tmp = grids[i];
                CharacterCfg monster = ResFactory.instance.GetMonsterCfgById(monsters[i]);
                Sprite icon = ResFactory.instance.LoadCharacterIcon(monster.icon);
                tmp.SetGridInfo(monsters[i], icon);
                tmp.callBack = UpdateMonsterInfo;
            }
            else if (i < cnt)
            {
                tmp = ResFactory.instance.LoadUIPrefabs("Grid").GetComponent<Grid>();
                tmp.transform.SetParent(m_Content,false);
                CharacterCfg monster = ResFactory.instance.GetMonsterCfgById(monsters[i]);
                Sprite icon = ResFactory.instance.LoadCharacterIcon(monster.icon);

                tmp.SetGridInfo(monsters[i],icon);
                tmp.callBack = UpdateMonsterInfo;
            }
            else
            {
                grids[i].OnRecycle();
            }
        }
        if (cnt != 0) UpdateMonsterInfo(monsters[0]);
    }
}
