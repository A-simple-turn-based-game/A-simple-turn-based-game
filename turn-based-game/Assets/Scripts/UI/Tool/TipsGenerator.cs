using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsGenerator : MonoBehaviour
{
    private void Awake()
    {
        EventCenter.AddListener<string>(EventType.TIPS, Tips);
        EventCenter.AddListener<string>(EventType.CFGERROR, CfgError);
    }

    void Tips(string msg)
    {
        transform.SetAsLastSibling();


        Tips tips = ResFactory.instance.LoadUIPrefabs("Tips").GetComponent<Tips>();
        tips.SetText(msg);
        tips.transform.SetParent(transform, false);
        //tips.transform.localScale = new Vector3(1,1);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(tips.transform.DOLocalMoveY(150f, 0.3f));
        sequence.Append(tips.transform.GetComponent<Image>().DOBlendableColor(new Color(1, 1, 1, 0), 0.2f));
        sequence.Append(tips.transform.GetComponentInChildren<Text>().DOBlendableColor(new Color(1, 1, 1, 0), 0.5f));
        // TODO 完善资源回收机制
        sequence.AppendCallback(() => tips.OnRecycle());

    }

    void CfgError(string error) {
        transform.SetAsLastSibling();
        Tips tips = ResFactory.instance.LoadUIPrefabs("Tips").GetComponent<Tips>();
        tips.SetText(error);
        tips.transform.SetParent(transform, false);
        tips.gameObject.SetActive(true);
        tips.transform.localScale = new Vector3(4,4,4);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<string>(EventType.TIPS, Tips);
        EventCenter.RemoveListener<string>(EventType.CFGERROR, CfgError);
    }
}
