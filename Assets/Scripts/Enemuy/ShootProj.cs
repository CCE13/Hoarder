using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProj : MonoBehaviour
{
    private DungeonEnemy de;

    private void Start()
    {
        de = GetComponentInParent<DungeonEnemy>();
    }

    public void Shoot()
    {
        de.ShootProjectile();
    }
}
