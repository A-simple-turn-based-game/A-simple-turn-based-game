using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryMonsterAI : ICharacterAI
{
    public override void MapActionAI(MapSystem mapSystem)
    {
        character.isEndMapRound = true;
        return;
    }
}
