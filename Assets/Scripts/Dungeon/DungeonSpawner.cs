using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSpawner : MonoBehaviour
{
    private DungeonController inst;

    private void Start()
    {
        inst = DungeonController.instance;
    }

    private void FixedUpdate()
    {
        if (!inst) inst = DungeonController.instance;

        if (Vector3.Distance(inst.player1.transform.position, transform.position) > 62 && inst.spawnLocations.Contains(transform) || 
            Vector3.Distance(inst.player2.transform.position, transform.position) > 62 && inst.spawnLocations.Contains(transform))
        {
            inst.spawnLocations.Remove(transform);
            return;
        }

        if (inst.spawnLocations.Contains(transform)) return;

        if (Vector3.Distance(inst.player1.transform.position, transform.position) < 62 || Vector3.Distance(inst.player2.transform.position, transform.position) < 62)
        {
            inst.spawnLocations.Add(transform);
        }
    }

    private void OnDisable()
    {
        if (!inst) inst = DungeonController.instance;
        if (inst.spawnLocations.Contains(transform))
        {
            inst.spawnLocations.Remove(transform);
        }
    }
}
