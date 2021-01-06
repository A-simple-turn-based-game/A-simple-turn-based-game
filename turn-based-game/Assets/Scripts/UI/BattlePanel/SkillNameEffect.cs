using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillNameEffect : MonoBehaviour
{
    public RectTransform moveStartPos;
    public RectTransform moveEndPos;
    private Text m_SkillName;
    private RectTransform m_Transfrom;
    private Vector3 vec;
    public void OnInit()
    {
        m_SkillName = transform.Find("Text").GetComponent<Text>();
        m_Transfrom = transform.GetComponent<RectTransform>();
        vec = transform.localPosition; 
    }

    public void ShowSkillName(string name) {

        m_SkillName.text = name;

        m_Transfrom.localPosition = moveStartPos.localPosition;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(m_Transfrom.DOLocalMoveX(moveEndPos.localPosition.x, 0.5f).SetEase(Ease.InQuint));
        sequence.AppendInterval(2f);
        sequence.Append(m_Transfrom.DOLocalMoveX(moveStartPos.localPosition.x, 0.5f).SetEase(Ease.OutQuint));
    
    
    }
}
