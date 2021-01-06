using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventGroup : MonoBehaviour
{
    private Text m_Name;
    private Text m_Descriprion;
    private Text m_Result;
    private Image m_Icon;
    private Transform m_Options;

    private List<EventOptBtn> opts = new List<EventOptBtn>();

    private ICharacter m_CurrCharacter;
    private IEvent m_CurrEvent;
    private IBuilding m_CurrBuilding;
    private PointerListener m_PointerListener;
    private GamePanel m_GamePanel;
    public void OnInit()
    { 
        m_Name = transform.Find("Name").GetComponent<Text>();
        m_Descriprion = transform.Find("Description").GetComponent<Text>();
        m_Icon = transform.Find("Icon").GetComponent<Image>();
        m_Options = transform.Find("Options");
        m_Result = transform.Find("Result").GetComponent<Text>();

        m_PointerListener = transform.gameObject.AddComponent<PointerListener>();
        transform.gameObject.SetActive(false);
    }

    public void RegisterEvent(GamePanel gamePanel,ICharacter character,IEvent @event,IBuilding baseBuilding) {

        character.DisableMapMove();
        
        this.m_CurrCharacter = character;
        this.m_CurrEvent = @event;
        this.m_CurrBuilding = baseBuilding;
        this.m_PointerListener.onClick = null;
        this.m_GamePanel = gamePanel; 

        m_Options.gameObject.SetActive(true);
        m_Result.gameObject.SetActive(false);
         
        m_Name.text = @event.name;
        m_Descriprion.text = @event.description;
        if(!string.IsNullOrEmpty( @event.icon))
            m_Icon.sprite = ResFactory.instance.LoadEventIcon(@event.icon);
        int cnt = @event.shortOpts.Count;

        opts.Clear();
        for (int i = 0; i < cnt; ++i) { 
            EventOptBtn btn = ResFactory.instance.LoadUIPrefabs("EventOpt").GetComponent<EventOptBtn>();
            btn.transform.SetParent(m_Options);
            btn.OnInit(i,@event.shortOpts[i],@event.options[i]);
            btn.onClickEvent = OnOptClick;
            opts.Add(btn);
        }
    }

    private void OnOptClick(int i) {

        foreach (EventOptBtn opt in opts)
        {
            Destroy(opt.gameObject);
        }
        m_Options.gameObject.SetActive(false);
        //Debug.Log(i + "    "+ m_CurrEvent.results.Count + "   " +m_CurrEvent.effects.Count) ;
        m_Result.text = m_CurrEvent.results[i];
        m_Result.gameObject.SetActive(true);


        List<List<Value>> content = m_CurrEvent.effects[i];

        foreach (List<Value> item in content)
        { 
            EventEffectParser.Parser(m_GamePanel, m_CurrCharacter,item);
        }

        m_PointerListener.onClick = (obj) =>
        {
            m_CurrCharacter.EnableMapMove();
            transform.gameObject.SetActive(false);
            Destroy(m_CurrBuilding.gameObject);
        };
    } 
    // Start is called before the first frame update
    void Start()
    {
        m_PointerListener.onClick = null;
    }

 
}
