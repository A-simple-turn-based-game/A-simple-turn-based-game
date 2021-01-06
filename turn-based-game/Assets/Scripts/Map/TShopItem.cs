using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TShopItem
{
    int GetCost();
    string GetName();
    string GetDescription();
    string GetIcon();
    string GetEffectDescription();
}
