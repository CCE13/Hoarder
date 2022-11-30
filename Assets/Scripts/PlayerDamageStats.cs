using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.UI;
using System;
using TMPro;
public class PlayerDamageStats : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("CheckDamageType")]
    [Range(0,100)]public float critChance;
    public int lifeSteal;
    public float critMultiplier;
    public int baseDamage;
    public float KnockBackIntensity;
    public ModifierHolder modifierHolder;
    public GameObject modifierDescription;

    [HideInInspector]
    public bool isCrit;

    private PlayerController _player;
    private Health _health;
    public static event Action<Modifier,PlayerController> modifierRemoved;
    private void Start()
    {
        _player = GetComponent<PlayerController>();
        _health = GetComponent<Health>();
        modifierDescription.SetActive(false);
        SwordMan.ModifierCollected += ApplyModifier;
        Ranger.ModifierCollected += ApplyModifier;
        WeaponDamager.lifeSteal += LifeSteal;
    }

    private void OnDestroy()
    {
        SwordMan.ModifierCollected -= ApplyModifier;
        Ranger.ModifierCollected -= ApplyModifier;
        WeaponDamager.lifeSteal -= LifeSteal;
    }

    private IEnumerator ShowDescription(Modifier modifierToApply)
    {
        modifierDescription.SetActive(true);
        modifierDescription.transform.GetChild(1).GetComponent<Image>().sprite = modifierToApply.spriteToShow;
        modifierDescription.GetComponentInChildren<TMP_Text>().text = modifierToApply.modifierDescription;
        yield return new WaitForSeconds(3f);
        modifierDescription.SetActive(false);
    }

    /// <summary>
    /// checks the type of modifier being collected and modifys accordingly 
    /// </summary>
    /// <param name="modifierToApply"></param>
    private void ApplyModifier(Modifier modifierToApply, PlayerController player)
    {
        if(player != _player) { return; }
        StartCoroutine(ShowDescription(modifierToApply));
        if (modifierHolder.damage.Contains(modifierToApply))
        {
            baseDamage += modifierToApply.damageToAdd;
        }
        if (modifierHolder.lifeSteal.Contains(modifierToApply))
        {
            lifeSteal += modifierToApply.lifeStealToAdd;
        }
        if (modifierHolder.healthIncrease.Contains(modifierToApply))
        {
            _health.SetNewMaxHealth(modifierToApply.healthAmountToGain);
        }
        if (modifierHolder.critChance.Contains(modifierToApply))
        {
            critChance += modifierToApply.critChanceToAdd;
        }
        if (modifierHolder.critMultiplier.Contains(modifierToApply))
        {
            critMultiplier += modifierToApply.critMultiplierToAdd;
        }
        if(modifierHolder.healthRegen.Contains(modifierToApply))
        {
            _health.HealthToRegen += modifierToApply.healthToRegenToAdd;
        }

        Debug.Log($"{modifierToApply} Modifier applied!!");
    }

    public void LifeSteal(PlayerController player)
    {
        if(player != _player) { return; }
        foreach (Modifier lifeStealModifier in modifierHolder.lifeSteal)
        {
            if (player.modifiersCollected.Contains(lifeStealModifier))
            {
                _health.AddHealth(lifeSteal);
            }
        }
       
    }

    /// <summary>
    /// Removed a random modifier from the _player if the _player has any
    /// If there is, choose a random index and remove that modifier of that index.
    /// </summary>
    /// 

    [ContextMenu("chonchingchingching")]
    public void RemoveRandomModifier()
    {
        if(_player.modifiersCollected.Count == 0) { Debug.Log("No Modifiers To Remove"); return; }
        var newPopUp = ObjectPool.SharedInstance.GetPopUp();
        newPopUp.transform.position = transform.position;

        var _nText = newPopUp.GetComponent<TMP_Text>();
        _nText.text = "Modifier Removed!!!";
        _nText.fontSize = 6.8f;
        newPopUp.SetActive(true);

        int randomModifier = UnityEngine.Random.Range(0, _player.modifiersCollected.Count);
        Modifier modifierSelected = _player.modifiersCollected[randomModifier];     
        Debug.Log(modifierSelected);
        modifierRemoved?.Invoke(modifierSelected, _player);
        _player.modifiersCollected.RemoveAt(randomModifier);
        _player.modifierUI.ReloadList(_player);

        Debug.Log(_player.modifiersCollected.Count);
    }


    /// <summary>
    /// Returns the damageToAdd to deal according to the crit multiplier
    /// </summary>
    /// <returns></returns>
    public int DamageToDeal()
    {
        if (CritChanceChecker())
        {
            int damageToDeal = Mathf.CeilToInt(baseDamage * critMultiplier);
            isCrit = true;
            return damageToDeal;
        }
        else
        {
            isCrit = false;
            return baseDamage;
        }
    }


    /// <summary>
    /// Checks if the _player is able to crit
    /// </summary>
    /// <returns></returns>
    private bool CritChanceChecker()
    {
        float critChances = UnityEngine.Random.Range(0f, 101f);
        if(critChance >= critChances)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
