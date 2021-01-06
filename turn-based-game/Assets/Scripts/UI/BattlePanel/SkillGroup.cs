using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillGroup : MonoBehaviour
{
    private ItemBtn m_Btn_Skill_1;
    private ItemBtn m_Btn_Skill_2;
    private ItemBtn m_Btn_Skill_3;
    private Button m_Btn_SKillBack;
       
    private Vector3 m_PosBtnSkill_1;
    private Vector3 m_PosBtnSkill_2;
    private Vector3 m_PosBtnSkill_3;
    private Vector3 m_PosBtnSkillBack;
    private Vector3 m_PosSkillGroupMove;

    private Dictionary<ISkill,ItemBtn> m_SkillBtn = new Dictionary<ISkill, ItemBtn>();
    private List<ItemBtn> m_ItemBtnList = new List<ItemBtn>();
    private List<ISkill> m_Skills;
    private bool m_CanBeClicked = true;

    public Action btnSkillBackClicked = null;
    public Action<ISkill> OnClick = null;
    public Action<ISkill> OnLongPress = null;
    public Action OnLongPressUp = null;
    public Action OnBack = null;
    public void OnInit()
    {
        m_Btn_Skill_1 = transform.Find("Btn_Skill_1").GetComponent<ItemBtn>();
        m_Btn_Skill_1.OnInit(0);
        m_ItemBtnList.Add(m_Btn_Skill_1);
        m_Btn_Skill_2 = transform.Find("Btn_Skill_2").GetComponent<ItemBtn>();
        m_Btn_Skill_2.OnInit(1);
        m_ItemBtnList.Add(m_Btn_Skill_2);
        m_Btn_Skill_3 = transform.Find("Btn_Skill_3").GetComponent<ItemBtn>();
        m_Btn_Skill_3.OnInit(2);
        m_ItemBtnList.Add(m_Btn_Skill_3);

        m_Btn_SKillBack = transform.Find("Back").GetComponent<Button>();
        m_Btn_SKillBack.onClick.AddListener(()=>DisAppear(OnBack));

        m_PosSkillGroupMove = transform.Find("BtnMovePos").GetComponent<RectTransform>().localPosition; 
        m_PosBtnSkill_1 = m_Btn_Skill_1.transform.localPosition;
        m_PosBtnSkill_2 = m_Btn_Skill_2.transform.localPosition;
        m_PosBtnSkill_3 = m_Btn_Skill_3.transform.localPosition;
        m_PosBtnSkillBack = m_Btn_SKillBack.transform.localPosition;
    }
      
    public void UpdateSkills(List<ISkill> skills) {  
        this.m_Skills = skills;
        int len = Mathf.Min(skills.Count,m_ItemBtnList.Count);
        foreach (ItemBtn ItemBtn in m_ItemBtnList)
        {
            ItemBtn.Clear();
        }

        for (int i = 0; i < len; ++i) {
            if (m_SkillBtn.ContainsKey(skills[i]))
            {
                continue;
            }
            m_SkillBtn.Add(skills[i], m_ItemBtnList[i]); 
            m_ItemBtnList[i].OnClick = OnSkillBtnClick;
            m_ItemBtnList[i].OnLongPress = OnSkillBtnLongPress;
            m_ItemBtnList[i].OnLongPressUp = OnLongPressUp;
            Sprite sprite = ResFactory.instance.LoadSkillIcon(skills[i].icon);
            m_ItemBtnList[i].SetBtnInfo(sprite);
        }
    }
    public void OnSkillBtnClick(int idx) {
        if (m_CanBeClicked == false) return; 
        OnClick?.Invoke(m_Skills[idx]);
    }
    private void OnSkillBtnLongPress(int idx) {
        OnLongPress?.Invoke(m_Skills[idx]);
    }
 
     
    public void ChangeInteractable(bool interactable) {
        m_CanBeClicked = interactable;
        m_Btn_SKillBack.interactable = interactable;
    }

    public void UpdateSkillCD(ISkill skill, int cd)
    {
        m_SkillBtn[skill].UpdateCD(cd);
    }

    public void ClearAllSkillInfo()
    {
        m_SkillBtn.Clear();
        foreach (ItemBtn btn in m_ItemBtnList)
        {
            btn.Clear();
        }   
    }
    #region 动效
    public void OnReset() {
        
        Vector2 es = new Vector2(0, 0);

        QTool.SetLocalPosAndLocalScale(m_Btn_Skill_1.gameObject.transform, m_PosSkillGroupMove, es);
        QTool.SetLocalPosAndLocalScale(m_Btn_Skill_2.gameObject.transform, m_PosSkillGroupMove, es);
        QTool.SetLocalPosAndLocalScale(m_Btn_Skill_3.gameObject.transform, m_PosSkillGroupMove, es);
        QTool.SetLocalPosAndLocalScale(m_Btn_SKillBack.gameObject.transform, m_PosSkillGroupMove, es);
    }

    public void Appear() {
        QTool.DOLocalPosAndScale(m_Btn_Skill_1.transform, m_PosBtnSkill_1, new Vector2(1f, 1f));
        QTool.DOLocalPosAndScale(m_Btn_Skill_2.transform, m_PosBtnSkill_2, new Vector2(1f, 1f));
        QTool.DOLocalPosAndScale(m_Btn_Skill_3.transform, m_PosBtnSkill_3, new Vector2(1f, 1f));
        QTool.DOLocalPosAndScale(m_Btn_SKillBack.transform, m_PosBtnSkillBack, new Vector2(1f, 1f));
    }

    public void DisAppear(Action callBack) {
        QTool.DOLocalPosAndScale(m_Btn_Skill_1.transform, m_PosSkillGroupMove, new Vector2(0f, 0f));
        QTool.DOLocalPosAndScale(m_Btn_Skill_2.transform, m_PosSkillGroupMove, new Vector2(0f, 0f));
        QTool.DOLocalPosAndScale(m_Btn_Skill_3.transform, m_PosSkillGroupMove, new Vector2(0f, 0f));
        QTool.DOLocalPosAndScale(m_Btn_SKillBack.transform, m_PosSkillGroupMove, new Vector2(0f, 0f), callBack: callBack);
    }

 
    #endregion
}
