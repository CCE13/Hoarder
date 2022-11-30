using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Player;

public class Health : MonoBehaviour
{

    public float currentHealth;
    public float maxHealth;
    public float HealthToRegen;

    public Gradient gradient;
    public bool isAPlayer;
    
    public Slider HealthBar;
    private TMP_Text healthCount;
    private Image fill;
    private float time = 1f;



    // Start is called before the first frame update
    private void Awake()
    {

        fill = HealthBar.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        currentHealth = maxHealth;
        HealthBar.maxValue = maxHealth;
        HealthBar.value = maxHealth;

        if (isAPlayer)
        {
            healthCount = HealthBar.GetComponentInChildren<TMP_Text>();
            healthCount.text = $"{currentHealth}/{maxHealth}";
        }
       
    }
    public void BossHpReset()
    {
        fill = HealthBar.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        maxHealth = 2000 + (3000 * DungeonController.instance.difficultyLevel);
        ResetHealth();
    }

    private void Update()
    {
        if (isAPlayer && GetComponent<PlayerController>().isDead) return;
        if (isAPlayer && currentHealth < maxHealth && HealthToRegen > 0)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                AddHealth(HealthToRegen);
                time = 1f;
            }
        }
    }

    /// <summary>
    /// Deducts the health base on the <paramref name="value"/>, and sets the slider value based on the current health.
    /// Changed the health bar colour base don the current health over the max health
    /// </summary>
    /// <param name="value"></param>
    public void LoseHealth(float value)
    {
        currentHealth -= value;
        HealthBar.value = currentHealth;
        fill.color =  gradient.Evaluate(currentHealth / maxHealth);

        if(currentHealth  <= 0)
        {
            currentHealth = 0;
        }

        if (isAPlayer)
        {
            healthCount.text = $"{currentHealth}/{maxHealth}";
        }

        if(isAPlayer && currentHealth <= 0)
        {
            SwordMan SM = GetComponent<SwordMan>();
            Ranger R = GetComponent<Ranger>();

            if (!R && !SM) Debug.Log("Bro wtf is this");

            if (R) R.Death();
            if (SM) SM.Death();
        }
        
    }
    public void AddHealth(float value)
    {
        currentHealth += value;
        HealthBar.value = currentHealth;
        fill.color = gradient.Evaluate(currentHealth / maxHealth);

        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (isAPlayer)
        {
            healthCount.text = $"{currentHealth}/{maxHealth}";
        }

    }
    public void SetNewMaxHealth(float value)
    {
        maxHealth += value;
        HealthBar.maxValue = maxHealth;
        fill.color = gradient.Evaluate(currentHealth / maxHealth);

        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (isAPlayer)
        {
            healthCount.text = $"{currentHealth}/{maxHealth}";
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        HealthBar.maxValue = maxHealth;
        HealthBar.value = currentHealth;
        fill.color = gradient.Evaluate(currentHealth / maxHealth);

        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (isAPlayer)
        {
            healthCount.text = $"{currentHealth}/{maxHealth}";
        }
    }
}
