using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Modifier Collectible Holder")]
public class ModifierCollectableHolder : ScriptableObject
{
    public List<ModiferCollectable> commonModifiers;

    public List<ModiferCollectable> rareModifiers;

    public List<ModiferCollectable> legendaryModifiers;


    public int commonCoinCost;
    public int rareCoinCost;
    public int legendaryCoinCost;
}

