using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoundController 
{
    private List<ICharacter> characterList = new List<ICharacter>();

    private MapSystem m_MpaSystem;

    private int currIdx = -1;

    public MapRoundController(MapSystem mapSystem) {
        this.m_MpaSystem = mapSystem;
    }

    public void AddCharacter(ICharacter character) { characterList.Add(character); }
    public void RemoveCharacter(ICharacter character) {
        if (character == characterList[currIdx])
        {
            --currIdx;
            characterList.Remove(character);
            NextTurn();
        }
        else { 
            characterList.Remove(character);
        } 
    }

     
    public void NextTurn() { 
        currIdx = (currIdx + 1) % characterList.Count;
        characterList[currIdx].isEndMapRound = false;
        // 开始行动
        characterList[currIdx].MapRound(m_MpaSystem);
    }
    public void OnInit() { 
        currIdx = -1;
        characterList.Clear();
        characterList.Add(m_MpaSystem.player);
        foreach (KeyValuePair<ICharacter,Ceil> item in m_MpaSystem.characterCeilDict)
        {
            if (item.Key == m_MpaSystem.player) continue;
            characterList.Add(item.Key);
        }
    }

    public void OnUpdate() {

        if (currIdx == -1 || characterList[currIdx].isEndMapRound)
        {
            NextTurn();
        }
    }

}
