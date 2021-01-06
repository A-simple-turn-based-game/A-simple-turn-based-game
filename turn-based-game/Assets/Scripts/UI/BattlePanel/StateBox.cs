using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBox : MonoBehaviour
{ 
    // state ID  和  状态图标
    Dictionary<int, StateIcon> m_StateDict = new Dictionary<int, StateIcon>();


    public void UpdateBuff(int id,int round) {
        m_StateDict[id].UpdateRound(round);
    } 
    public void AddBuff(IBuff state,int round) {

        if (m_StateDict.ContainsKey(state.id))
        { 
            m_StateDict[state.id].UpdateRound(round);
            if(state.effectMode == BuffEffectMode.EFFECT_STACKING)
                m_StateDict[state.id].UpdateCnt();
        }
        else {
            StateIcon sicon = ResFactory.instance.LoadUIPrefabs("StateIcon").GetComponent<StateIcon>();
            sicon.transform.SetParent(transform,false);
            sicon.OnInit(state,round); 
            m_StateDict.Add(state.id,sicon);
        }
    }

    public void RemoveState(IBuff state) {
        
        if (m_StateDict.ContainsKey(state.id))
        { 
            m_StateDict[state.id].OnRecycle();
            m_StateDict.Remove(state.id);  
        }
        else
        {
            LogTool.LogError("正在尝试删除不存在的状态");
        }
    }


    public void ClearAllState() {
        foreach (KeyValuePair<int,StateIcon> kvp in m_StateDict) {
            kvp.Value.OnRecycle();
        }  
        m_StateDict.Clear();
    }

}
