using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RIPPlayerDamage : MonoBehaviour
{
    // Start is called before the first frame update
    public SphereCollider[] attackPoints;

    public string tagToHit;

    private RIPInputManager input;
    public bool canDamage;
    private GameObject col;
    void Start()
    {
        input = transform.GetComponentInParent<RIPInputManager>();
        attackPoints = transform.GetComponentsInChildren<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (SphereCollider collider in attackPoints)
        {
            if (collider.isTrigger && other.CompareTag(tagToHit))
            {
                Debug.Log("Hit");
                col = other.gameObject;
                break;
            }
            else
            {
                Debug.Log("Null");
            }
        }
    }

    public void OnHit()
    {
        if (col)
        {
            col.GetComponent<Health>().LoseHealth(input.currentAttack.damage);
            col.GetComponent<Rigidbody>().velocity = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z) * input.currentAttack.knockBackStrength;
            col = null;
        }
        
    }
}
