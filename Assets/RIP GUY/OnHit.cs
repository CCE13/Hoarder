using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHit : MonoBehaviour
{
    private RIPPlayerDamage damage;
    // Start is called before the first frame update
    void Start()
    {
        damage = transform.parent.GetComponent<RIPPlayerDamage>();
    }

    // Update is called once per frame

    public void Hitting()
    {
        damage.OnHit();
    }
}
