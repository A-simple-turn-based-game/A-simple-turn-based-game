using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTaskGroup : MonoBehaviour
{
    private Transform m_TaskGroup;
    private Dictionary<int,Toggle> m_TaskList = new Dictionary<int, Toggle>();
    private void Awake()
    {
        EventCenter.AddListener<int,string>(EventType.ADD_TASK,AddTask);
        EventCenter.AddListener<int>(EventType.FINISH_TASK, FinishTask);
        EventCenter.AddListener(EventType.CLEAR_TASK, Clear);
    }
    private void Start()
    {
        
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<int, string>(EventType.ADD_TASK,AddTask);
        EventCenter.RemoveListener<int>(EventType.FINISH_TASK, FinishTask);
        EventCenter.RemoveListener(EventType.CLEAR_TASK, Clear);
    }

    public void OnInit() {
        m_TaskGroup = transform.Find("TaskGroup");
    }

    private void AddTask(int idx,string content) {
        Toggle toggle = ResFactory.instance.LoadUIPrefabs("Task").GetComponent<Toggle>();
        toggle.transform.SetParent(m_TaskGroup,false);

        Text text = toggle.transform.Find("Label").GetComponent<Text>();
        text.text = content;

        m_TaskList.Add(idx,toggle);
    }

    private void FinishTask(int idx) {
        m_TaskList[idx].isOn = true;
    }

    public void Clear() {

        foreach (KeyValuePair<int,Toggle> kvp in m_TaskList)
        {
            Destroy(kvp.Value.gameObject);
        }
        m_TaskList.Clear();
    }
}
