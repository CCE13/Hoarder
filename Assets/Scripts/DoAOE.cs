using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoAOE : MonoBehaviour
{
    public DungeonBossTile tile;

    private void OnEnable()
    {
        tile.RandomAOE();
    }
}
