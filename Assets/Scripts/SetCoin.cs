using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SetCoin : MonoBehaviour
{
    private int goldToAdd;
    public bool added;
    public static event Action<int> addGold;

    private ParticleSystem coinSplosion;

    private void Start()
    {
        Collider collider = GameObject.Find("Coin Count").GetComponent<Collider>();
        coinSplosion = GetComponent<ParticleSystem>();
        coinSplosion.trigger.AddCollider(collider);
        added = false;
    }

    private void OnEnable()
    {
        Collider collider = GameObject.Find("Coin Count").GetComponent<Collider>();
        coinSplosion = GetComponent<ParticleSystem>();
        coinSplosion.trigger.AddCollider(collider);
        added = false;
    }

    void OnParticleTrigger()
    {
        goldToAdd = (int)GetComponent<ParticleSystem>().emission.GetBurst(0).count.constant;
        if (added) { return; }
        ParticleSystem ps = GetComponent<ParticleSystem>();

        // particles
        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

        // get
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        // iterate
        for (int i = 0; i < numEnter; i++)
        {
            goldToAdd *= 2 + (DungeonController.instance.difficultyLevel * 5);
            Debug.Log(goldToAdd);
            addGold?.Invoke(goldToAdd);
            added = true;
            break;
        }

        // set
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
    }
}
