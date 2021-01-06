using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tips : RecoverableObject
{
    Image m_BG;
    Text m_Text;

    private void Awake()
    {
        m_BG = transform.GetComponent<Image>();
        m_Text = transform.GetComponentInChildren<Text>();
    }

    public override void OnGenerate()
    {
        base.OnGenerate();
        gameObject.SetActive(true);

        transform.localPosition = new Vector3(0, 0, 0);
        m_BG.color = new Color(1, 1, 1, 1);
        m_Text.color = new Color(1, 1, 1, 1);
    }
    public void SetText(string msg) {
        m_Text.text = msg;
    }
    public override void OnRecycle()
    {
        base.OnRecycle();
        gameObject.SetActive(false);
    }
}
