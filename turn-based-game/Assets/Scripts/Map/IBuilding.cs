using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IBuilding : MonoBehaviour
{
    private IEvent m_Event;

    public void RegisterEvent(IEvent @event) {
        this.m_Event = @event;
    }

    public void EncounterEvent(ICharacter character,Action endCallBack)
    {
        // 如果是怪物触发，直接取消并回合结束
        if (character is Monster) {
            character.isEndMapRound = true;
            return;
        }
        EventCenter.Broadcast<ICharacter, IEvent, IBuilding>(EventType.EVENT, character , m_Event ,this);
    }

    public void OnSelected(MapCeilController mapController, Ceil ceil)
    {
  
    }
}
