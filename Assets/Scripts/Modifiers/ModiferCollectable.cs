using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModiferCollectable : MonoBehaviour
{
    public Modifier modifier;

    public bool _hasCollected = false;

    private void OnValidate()
    {
        gameObject.name = modifier.name;
        gameObject.GetComponent<SpriteRenderer>().sprite = modifier.spriteToShow;
    }
}
