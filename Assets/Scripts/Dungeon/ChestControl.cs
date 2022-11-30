using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ChestControl : MonoBehaviour
{
    public ParticleSystem coinExplosion;

    private Animator anim;
    private BoxCollider chestCollider;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        chestCollider = GetComponent<BoxCollider>();
    }

    public void RandomFunction()
    {
        int func = UnityEngine.Random.Range(0, 2);

        switch(func)
        {
            case 0:
                NormalChest();
                break;
            case 1:
                GoldChest();
                break;
        }
    }

    public void NormalChest()
    {
        // Pick a random Essence and play the tier's respective animation.
        // Change StateName to respective tier.
        ModifierEventManager.SpawnModifier(transform);
        anim.Play("Modifier");
        DisableCollider();
    }
    public void GoldChest()
    {
        // Pick a random amount of gold within a range and play animation.
        var amountOfGold = UnityEngine.Random.Range(0, 20);
        var burst = coinExplosion.emission.GetBurst(0);
        var newBurst = new ParticleSystem.Burst(burst.time, amountOfGold, burst.cycleCount, burst.repeatInterval);
        coinExplosion.emission.SetBurst(0, newBurst);
        anim.Play("Gold");
        DisableCollider();


    }


    private void DisableCollider()
    {
        chestCollider.enabled = false;
    }


}
