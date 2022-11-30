using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using TMPro;
using UnityEngine.UI;

public class DungeonBoss : MonoBehaviour
{
    public enum DamageType
    {
        AOE,
        DOT,
        NormalDamage,
    }

    public float time;
    public float rateTime;
    public int damage;
    public DamageType damageType;
    [SerializeField]private float minTimeBewtweenAttacks;
    [SerializeField]private float maxTimeBetweenAttacks;

    public GameObject Exit;

    public List<string> attackAnimations;
    public List<GameObject> legendaries;

    private bool isAttacking;
    private Animator anim;
    private Health hp;
    private DungeonEnemy de;
    private DungeonController inst;
    private EndGame endgame;

    public Transform lootDrop;

    [HideInInspector]
    public bool isDead;

    private void Start()
    {
        inst = DungeonController.instance;
        endgame = Camera.main.GetComponent<EndGame>();
        anim = GetComponentInChildren<Animator>();
        de = GetComponent<DungeonEnemy>();
        hp = GetComponent<Health>();
        hp.HealthBar = Camera.main.transform.GetChild(0).GetChild(0).Find("Boss UI").GetComponent<Slider>();
        de.canvas = hp.HealthBar.gameObject;
        hp.BossHpReset();

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (!hp) return;
        hp.BossHpReset();
    }

    private void ChooseDamageType()
    {
        int randomType = Random.Range(0, 3);
        switch (randomType)
        {
            case 0:
                damageType = DamageType.NormalDamage;
                break;
            case 1:
                damageType = DamageType.DOT;
                break;
            case 2:
                damageType = DamageType.NormalDamage;
                break;
        }
    }

    public void CheckDamageType(PlayerController player)
    {
        if (damageType == DamageType.DOT)
        {
            DamageOvertimeSetup(player);
        }
        if (damageType == DamageType.NormalDamage)
        {
            damage = 30;
            Damage(player.GetComponent<Health>(),damage);
        }
    }

    private IEnumerator AttackAnimation()
    {
        isAttacking = true;
        
        float timeBetweenAttacks = Random.Range(minTimeBewtweenAttacks, maxTimeBetweenAttacks);
        yield return new WaitForSeconds(timeBetweenAttacks);
        Debug.Log("ATTACKING");
        int animationIndexToPlay = Random.Range(0, attackAnimations.Count);

        anim.Play(attackAnimations[animationIndexToPlay]);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f && !anim.IsInTransition(0) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
        Debug.Log("animation over");
        isAttacking = false;
    }
    /// <summary>
    /// Checks if the _player has a damageToAdd overtime modifier in the <paramref name="modifier"/>
    /// If Yes, deal damageToAdd over time and _player the delegated particle effect.
    /// </summary>
    /// <param name="modifier"></param>
    /// <param name="direction"></param>
    /// <param name="knockBackIntensity"></param>
    public void DamageOvertimeSetup(PlayerController player)
    {
        rateTime = 1;
        time = 5;
        damage = 3;
        StartCoroutine(DamageOverTime(player));
    }

    /// <summary>
    /// Deals damageToAdd overtime based on the <paramref name="time"/>, in every <paramref name="rateTime"/>
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="knockBackIntensity"></param>
    /// <param name="time"></param>
    /// <param name="rateTime"></param>
    /// <param name="damage"></param>
    /// <returns></returns>
    private IEnumerator DamageOverTime(PlayerController player)
    {
        while (time >= rateTime)
        {
            yield return new WaitForSeconds(rateTime);
            Damage(player.GetComponent<Health>(), damage);
            PopUp(damage,player);
            time -= rateTime;
        }
    }
    /// <summary>
    /// Sets the damageToAdd to show based on the <paramref name="damage"/>, instantiates at the transform.
    /// </summary>
    /// <param name="damage"></param>
    private void PopUp(int damage,PlayerController player)
    {
        //_text.text = damageToAdd.ToString();
        //Instantiate(damagePopUp, transform.position, damagePopUp.transform.rotation);

        var newPopUp = ObjectPool.SharedInstance.GetPopUp();
        newPopUp.transform.position = player.transform.position;

        var _nText = newPopUp.GetComponent<TMP_Text>();
        _nText.text = damage.ToString();
        _nText.color = Color.red;
        _nText.fontSize = 14.41f;
        newPopUp.SetActive(true);

    }

    private void Damage(Health health, int damage)
    {
        health.LoseHealth(damage);
    }

    public void BossDeath()
    {
        isDead = true;
        hp.HealthBar.gameObject.SetActive(false);   

        int randomLegendary = Random.Range(0, legendaries.Count);

        Instantiate(legendaries[randomLegendary], lootDrop.position, Quaternion.identity);
        GetComponentInParent<DungeonBossTile>().bossMusic.StopAudios();
        Invoke(nameof(Dead), 3);
    }    

    private void Dead()
    {
        Exit.SetActive(true);
        Camera.main.transform.GetChild(0).GetChild(0).Find("Coin UI").gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
