using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowValue : RecoverableObject
{
    private Text m_Text;

    public Color physicalDamageColor;

    public Color magicDamageColor;
    public Color healingColor;
    public Color manaColor;
    public Color noneColor;
    public Color critColor;

    private float speed = 0.5f;
    private float lifeTime = 1.0f;
    private float m_Time = 0f;
     

    private void Awake()
    {
        m_Text = GetComponent<Text>();
        transform.localScale = new Vector3(0,0);
        gameObject.SetActive(false);
    }
     
    public void SetValue(Value num,Action callBack = null) {

        // TODO 完善数字显示方式，和伤害类型
         
        switch (num.meaningType)
        {
            case ValueMeaningType.PHYSICAL:
                m_Text.color = physicalDamageColor;
                break;
            case ValueMeaningType.MAGIC:
                m_Text.color = magicDamageColor;
                break;
            case ValueMeaningType.CRITICAL_DAMAGE:
                m_Text.color = critColor;
                break;
            case ValueMeaningType.HEALING:
                m_Text.color = healingColor;
                break;
            case ValueMeaningType.MANA:
                m_Text.color = manaColor;
                break;
            case ValueMeaningType.NONE:
                m_Text.color = noneColor;
                break;
            default:
                break;
        }
        
        transform.localScale = new Vector3(0, 0);
        m_Text.text = "" + num;   
        Sequence sequence = DOTween.Sequence();
        sequence.Append(m_Text.transform.DOScale(new Vector3(1.5f,1.5f,1.5f),0.4f));
        sequence.AppendInterval(0.5f);
        sequence.Append(m_Text.DOFade(0f, 0.3f));
        sequence.AppendCallback(()=> {
            OnRecycle();
            callBack?.Invoke();
        });
    } 
    private void Update()
    {
        //this.transform.LookAt(Camera.main.transform);
        transform.localRotation = Quaternion.identity;
        if (m_Time >= lifeTime) {
            return; 
        } 
        transform.position += transform.up * speed * Time.deltaTime + transform.right * speed * Time.deltaTime;
        m_Time += Time.deltaTime;
    } 

    public override void OnGenerate()
    {
        base.OnGenerate(); 
        m_Time = 0f;
        gameObject.SetActive(true);
    }

    public override void OnRecycle()
    {
        base.OnRecycle();
        gameObject.SetActive(false);
    }
}
