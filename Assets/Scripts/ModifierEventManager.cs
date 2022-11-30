using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ModifierEventManager : MonoBehaviour
{
    public static bool RemoveModifierChosen;
    public static bool AddModifierChosen;
    public static UnityAction Event(PlayerDamageStats player,Transform positionToSpawn)
    {
        int index = UnityEngine.Random.Range(1, 101);

        if (index < 50)
        {
            if (AddModifierChosen)
            {
                return () => RemoveModifier(player);
            }
            AddModifierChosen = true;
            return () => SpawnModifier(positionToSpawn);
        }
        else if (index > 50)
        {
            if (RemoveModifierChosen)
            {
                return () => SpawnModifier(positionToSpawn);
            }
            RemoveModifierChosen = true;
            return () => RemoveModifier(player);
        }
        else
        {
            return null;
        }
    }


    public static void SpawnModifier(Transform positionToSpawn)
    {
        GameObject modifier = WeightProbabilityManager.RandomCollectibleOfRarity();
        Instantiate(modifier, positionToSpawn.position + Vector3.up + Vector3.back * 3,Quaternion.identity);
        Debug.Log(modifier);
    }
    public static void SpawnModifier(Transform positionToSpawn, GameObject modifier)
    {
        Instantiate(modifier, positionToSpawn.position + Vector3.up + Vector3.back * 3, Quaternion.identity);
        Debug.Log(modifier);
    }

    public static void RemoveModifier(PlayerDamageStats player)
    {
        player.RemoveRandomModifier();
    }

    public static void CoinSplosion(GameObject gameObject,bool inChild)
    {
        if (inChild)
        {
            gameObject.GetComponentInChildren<EffectsCont>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<EffectsCont>().enabled = true;
        }
    }


}