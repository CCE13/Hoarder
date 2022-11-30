using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Player;
using TMPro;


/// <summary>
/// SHOULD BE PLACED ON THE WEAPON THAT WILL DAMAGE THE OTHER
/// </summary>
public class WeaponDamager : MonoBehaviour
{
    public PlayerDamageStats playerDamage;
    public PlayerController player;
    public Vector3 directionToKnockBack;

    private GameObject _hit;
    public static event Action<PlayerController> lifeSteal;
    public float multiplier = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && other.gameObject != _hit)
        {

            _hit = other.gameObject;

            //gets the damageToAdd to deal and the enemy hit
            int damage = Mathf.RoundToInt(playerDamage.DamageToDeal() * multiplier);
            var _isCrit = playerDamage.isCrit;
            EnemyManager enemyHit = other.GetComponent<EnemyManager>();



            //check if its the swords man 
            if(GetComponentInParent<SwordMan>() != null)
            {
                directionToKnockBack = (other.transform.position - GetComponentInParent<SwordMan>().transform.position).normalized ;
            }
            //deals damageToAdd accordingly

            enemyHit.GetHit();

            enemyHit.NormalDamage(damage, directionToKnockBack, playerDamage.KnockBackIntensity, _isCrit);
            if(_isCrit)
            {
                enemyHit.DamageOvertimeSetup(player, player.modifiersCollected, directionToKnockBack, playerDamage.KnockBackIntensity, playerDamage.critMultiplier);
            }
            else
            {
                enemyHit.DamageOvertimeSetup(player, player.modifiersCollected, directionToKnockBack, playerDamage.KnockBackIntensity, 1);
            }
            enemyHit.AOESetup(player,player.modifiersCollected);
            lifeSteal?.Invoke(player);
        }
        if (other.CompareTag("Props"))
        {
            //play breaking prop animation
            int index = UnityEngine.Random.Range(0, 101);

            if(index < 95)
            {
                ModifierEventManager.CoinSplosion(other.gameObject, true);
                other.transform.GetChild(0).gameObject.SetActive(false);
                StartCoroutine(destroyProp(other.gameObject));
            }
            else
            {
                ModifierEventManager.SpawnModifier(other.transform);
                other.transform.GetChild(0).gameObject.SetActive(false);
                StartCoroutine(destroyProp(other.gameObject));
            }
            
        }
    }

    private void OnDisable()
    {
        _hit = null;
    }

    private IEnumerator destroyProp(GameObject prop)
    {
        yield return new WaitForSeconds(5);
        prop.SetActive(false);
    }
}
