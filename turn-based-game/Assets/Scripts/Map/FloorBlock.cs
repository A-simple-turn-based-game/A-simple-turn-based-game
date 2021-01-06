using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloorBlock :BaseBlock
{

    SpriteRenderer m_SelectRect;
    SpriteRenderer m_Occupy;
    Tweener m_FadeTwn = null;

    private void Awake()
    {
        m_PointerListener = GetComponent<PointerListener>();
        m_PointerListener.onClick = (_) => ceil.OnSelected();

        this.canBeBuilt = true; // TODO 等待更多多元化地板
        this.blockType = BlockType.FLOOR;

        this.m_SelectRect = transform.Find("rect").GetComponent<SpriteRenderer>();
        m_SelectRect.gameObject.SetActive(false);
        this.m_Occupy = transform.Find("occupy").GetComponent<SpriteRenderer>();
        m_Occupy.gameObject.SetActive(false);

        //Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

    }


    public override void BlockSelected()
    {
        m_SelectRect.gameObject.SetActive(true);
        m_SelectRect.transform.localScale = new Vector3(3.8f, 3.8f, 3.8f);
        m_FadeTwn = m_SelectRect.transform.DOScale(new Vector3(4.2f,4.2f,4.2f),0.5f).SetLoops(-1, LoopType.Yoyo);
    }
    public override void BlockCancelSelected()
    {
        m_FadeTwn?.Kill();
        m_SelectRect.gameObject.SetActive(false);
    }


    public override void BlockCancelMoveSelected()
    {
        //m_SelecttRect.DOFade(,); 
        m_FadeTwn?.Kill();
        m_SelectRect.gameObject.SetActive(false);
    }

    public override void BlockMoveSelected()
    {
        base.BlockMoveSelected();
        m_SelectRect.color = new Color(0.8f,1f,0.4f,0.1f);
        m_SelectRect.gameObject.SetActive(true);
        m_FadeTwn = m_SelectRect.DOFade(0.8f, 1).SetLoops(-1, LoopType.Yoyo);
        
    }



}
 