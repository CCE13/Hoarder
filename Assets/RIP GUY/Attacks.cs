using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Attacks")]
public class Attacks : ScriptableObject
{
    public string _name;
    public int damage;

    public float knockBackStrength;

}
