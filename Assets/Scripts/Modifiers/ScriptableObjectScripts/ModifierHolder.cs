using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ModifierTypeHolders")]
public class ModifierHolder : ScriptableObject
{
    
    public List<Modifier> critChance;
    public List<Modifier> critMultiplier;
    public List<Modifier> damage;
    public List<Modifier> healthRegen;
    public List<Modifier> healthIncrease;
    public List<Modifier> lifeSteal;
    public List<Modifier> damageOverTimeModifiers;
    public List<Modifier> AOEModifiers;
}
