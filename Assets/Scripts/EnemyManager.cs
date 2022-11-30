using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using TMPro;

/// <summary>
/// Manager for the enemies.
/// </summary>
public class EnemyManager : MonoBehaviour
{
    public AudioSource hitSFX;
    public ModifierHolder holders;
    public GameObject damagePopUp;
    private Health health;
    private Rigidbody rb;
    private DungeonEnemy dungeonEnemy;
    private DungeonBoss dungeonBoss;

    private Animator anim;

    public TMP_Text _popUpTxt;
    private int _acummulativeDmg;

    private void Start()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
        dungeonEnemy = GetComponent<DungeonEnemy>();
        dungeonBoss = GetComponent<DungeonBoss>();
        anim = GetComponentInChildren<Animator>();
;    }


    /// <summary>
    /// Deals normal base damageToAdd base on the <paramref name="damageDealt"/>, knockbacks the enemy towards the <paramref name="direction"/>
    /// </summary>
    /// <param name="damageDealt"></param>
    /// <param name="direction"></param>
    /// <param name="knockBackIntensity"></param>
    public void NormalDamage(int damageDealt, Vector3 direction, float knockBackIntensity, bool isCrit)
    {
        Damage(damageDealt);
        KnockBack(direction, knockBackIntensity);
        PopUp(damageDealt, isCrit);
    }

    #region Damage Overtime

    /// <summary>
    /// Checks if the _player has a damageToAdd overtime modifier in the <paramref name="modifier"/>
    /// If Yes, deal damageToAdd over time and _player the delegated particle effect.
    /// </summary>
    /// <param name="modifier"></param>
    /// <param name="direction"></param>
    /// <param name="knockBackIntensity"></param>
    public void DamageOvertimeSetup(PlayerController player, List<Modifier> modifier, Vector3 direction, float knockBackIntensity, float critMult)
    {
        foreach (Modifier damageOvertimeModifier in holders.damageOverTimeModifiers)
        {
            if (modifier.Contains(damageOvertimeModifier))
            {
                int numberOfModifier = player.modifier[damageOvertimeModifier.name];
                float rateTime = damageOvertimeModifier.DO_rateTime;
                float time = damageOvertimeModifier.DO_duration;
                int damage = (int)(damageOvertimeModifier.DO_damageToDeal * numberOfModifier * critMult);
                Debug.Log(damage);
                
                ParticleSystem particleEffect = damageOvertimeModifier.DO_particleEffect;
                ParticleEffect(particleEffect);
                StartCoroutine(DamageOverTime(time, rateTime, damage));
                KnockBack(direction, knockBackIntensity);
            }
        }
    }


    /// <summary>
    /// Deals damageToAdd overtime based on the <paramref name="time"/>, in every <paramref name="rateTime"/>
    /// </summary>
    /// <param name="time"></param>
    /// <param name="rateTime"></param>
    /// <param name="damage"></param>
    /// <returns></returns>
    private IEnumerator DamageOverTime(float time, float rateTime, int damage)
    {
        while (time >= rateTime)
        {
            if (!dungeonEnemy.isAlive) break;
            yield return new WaitForSeconds(rateTime);
            _acummulativeDmg += damage;
            _popUpTxt.text = _acummulativeDmg.ToString();
            _popUpTxt.gameObject.SetActive(true);

            Damage(damage);
            time -= rateTime;
        }

        _popUpTxt.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!_popUpTxt.gameObject.activeSelf) _acummulativeDmg = 0;
    }
    #endregion

    #region AOE

    /// <summary>
    /// Checks if the _player has collected an AOE modifier
    /// If yes, gets all the enemys in the explosion radius and deals AOE damageToAdd accordingly
    /// </summary>
    /// <param name="modifier"></param>
    public void AOESetup(PlayerController player,List<Modifier> modifier)
    {
        foreach (Modifier AOEmodifier in holders.AOEModifiers)
        {
            if (modifier.Contains(AOEmodifier))
            {
                int modifierCount = player.modifier[AOEmodifier.name];
                Collider[] collisions = Physics.OverlapSphere(transform.position, AOEmodifier.explosionRadius);
                foreach (Collider collider in collisions)
                {
                    Rigidbody enemyNear = collider.GetComponent<Rigidbody>();
                    EnemyManager enemyManager = collider.GetComponent<EnemyManager>();

                    if (enemyNear != null && enemyManager != null)
                    {
                        enemyNear.AddForce((enemyNear.transform.position - transform.position) * AOEmodifier.explosionVelocity, ForceMode.VelocityChange);
                        enemyManager.Damage(AOEmodifier.explosiveDamage * modifierCount);
                    }
                }
                break;
            }
        }
    }
    #endregion

    public void GetHit()
    {
        anim.Play("Hit");
    }

    /// <summary>
    /// Finds the particle effect in the child and plays
    /// </summary>
    /// <param name="particleEfect"></param>
    private void ParticleEffect(ParticleSystem particleEfect)
    {
        transform.Find(particleEfect.name).GetComponent<ParticleSystem>().Play();
    }

    /// <summary>
    /// Deals damageToAdd according to <paramref name="damage"/>
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(int damage)
    {
        PlayerController.damageDealt += damage;
        health.LoseHealth(damage);
        hitSFX.Play();
        if (health.currentHealth <= 0)
        {
            if(dungeonEnemy)
            {
                if(!dungeonEnemy.isAlive)
                {
                    return;
                }
                dungeonEnemy.Death();
                Points.enemyKill += 1;
            }
        }
    }

    /// <summary>
    /// Sets the damageToAdd to show based on the <paramref name="damage"/>, instantiates at the transform.
    /// </summary>
    /// <param name="damage"></param>
    private void PopUp(int damage, bool isCrit)
    {
        //_text.text = damageToAdd.ToString();
        //Instantiate(damagePopUp, transform.position, damagePopUp.transform.rotation);

        var newPopUp = ObjectPool.SharedInstance.GetPopUp();
        newPopUp.transform.position = transform.position;

        var _nText = newPopUp.GetComponent<TMP_Text>();
        _nText.text = damage.ToString();

        newPopUp.GetComponent<PopUp>().isCrit = isCrit;

        if(dungeonBoss)
        {
            newPopUp.GetComponent<PopUp>().isBoss();
        }

        newPopUp.SetActive(true);

    }

    /// <summary>
    /// deals knockback base don the <paramref name="direction"/>
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="knockBackIntensity"></param>
    private void KnockBack(Vector3 direction, float knockBackIntensity)
    {
        rb.velocity = knockBackIntensity * new Vector3(direction.x, 0f, direction.z);
    }
}
