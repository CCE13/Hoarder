using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class ShootArrow : MonoBehaviour
{
    public Ranger ranger;
    // Start is called before the first frame update
    void Start()
    {
        ranger = GetComponentInParent<Ranger>();
    }

    public void ArrowShot()
    {
        ranger.ShootArrow();
    }
}
