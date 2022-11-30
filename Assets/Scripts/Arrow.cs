using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    public bool isPiercing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //sets the object to inactive when he hits the enemy.
        if (other.CompareTag("Enemy") || other.CompareTag("Wall") || other.CompareTag("Door"))
        {
            if (isPiercing) return;

            rb.velocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            gameObject.SetActive(false);

        }
    }
}
