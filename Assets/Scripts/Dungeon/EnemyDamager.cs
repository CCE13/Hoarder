using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamager : MonoBehaviour
{
    private Rigidbody _rb;

    public int damage;

    private bool isRanged;

    private Health p1Hp;
    private Health p2Hp;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        var inst = DungeonController.instance;
        p1Hp = inst.player1.GetComponent<Health>();
        p2Hp = inst.player2.GetComponent<Health>();
    }

    public void SetVelocity(Vector3 amount)
    {
        isRanged = true;
        _rb.velocity = amount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            var playerHit = other.GetComponent<Health>();

            if (playerHit == p1Hp && p1Hp.currentHealth > 0)
            {
                p1Hp.LoseHealth(damage + DungeonController.instance.difficultyLevel);
            }
            if (playerHit == p2Hp && p2Hp.currentHealth > 0)
            {
                p2Hp.LoseHealth(damage + DungeonController.instance.difficultyLevel);
            }
        }

        if (!isRanged) return;

        if( other.CompareTag("Wall") || other.CompareTag("Door"))
        {
            _rb.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}
