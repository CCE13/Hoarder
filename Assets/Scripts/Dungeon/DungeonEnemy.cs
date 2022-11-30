using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DungeonEnemy : MonoBehaviour
{
    [Header("Enemy Logistics")]
    [Range(0,124)]
    public float detectDistance = 62;
    [Range(0,31)]
    public float distanceOffset;

    public float attackIntervals;
    public List<string> attackAnimStates;
    public int enemyDamage;
    public float rotateSpeed;
    public float projectileSpeed;
    public Transform projectileSpawnLocation;
    [HideInInspector]
    public bool isAlive = true;

    [HideInInspector]public DungeonTile _Tile;
    private NavMeshAgent agent;
    private DungeonController inst;

    public GameObject canvas;
    public List<EffectsCont> particleFX;

    [HideInInspector]
    public CapsuleCollider _cCollider;

    private Health p1Hp;
    private Health p2Hp;

    private Health hp;

    [HideInInspector]
    public Animator _anim;

    private bool _isAttacking;

    public SpriteRenderer targetMarker;
    public Color player1Target;
    public Color player2Target;

    private Rigidbody _rb;
    private DungeonBoss _db;

    private void Awake()
    {
        _Tile = GetComponentInParent<DungeonTile>();
        agent = GetComponent<NavMeshAgent>();
        _cCollider = GetComponent<CapsuleCollider>();
        _rb = GetComponent<Rigidbody>();
        _db = GetComponent<DungeonBoss>();

        _anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        inst = DungeonController.instance;

        p1Hp = inst.player1.GetComponent<Health>();
        p2Hp = inst.player2.GetComponent<Health>();
        hp = GetComponent<Health>();

        if(_Tile == null)
        {
            Debug.LogWarning("There is no tile on " + name);
        }
    }

    private void FixedUpdate()
    {
        if (!isAlive) return;
        if (p1Hp.currentHealth <= 0 && p2Hp.currentHealth <= 0)
        {
            _rb.velocity = Vector3.zero ;
            return;
        }
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;
        if (_isAttacking) return;

        // Check the distance between this enemy and _player
        float _p1Distance = Vector3.Distance(transform.position, inst.player1.transform.position);
        float _p2Distance = Vector3.Distance(transform.position, inst.player2.transform.position);
        Transform _target;

        if(_p1Distance < _p2Distance)
        {
            _target = inst.player1.transform;
        }
        else
        {
            _target = inst.player2.transform;
        }

        if (p1Hp.currentHealth <= 0) _target = inst.player2.transform;
        if (p2Hp.currentHealth <= 0) _target = inst.player1.transform;

        if (Vector3.Distance(transform.position, _target.position) > detectDistance) return;

        agent.destination = _target.position;

        transform.LookAt(_target);

        if (agent.remainingDistance < distanceOffset)
        {
            agent.isStopped = true;
            if (attackAnimStates.Count == 0) return;
            if (!_isAttacking) StartCoroutine(Attacking());
        }
        else
        {
            if (_anim) _anim.Play("Walk");
            agent.isStopped = false;
        }
    }

    public void SelectTarget(bool player)
    {
        targetMarker.gameObject.SetActive(true);

        if(!player)
        {
            targetMarker.color = player1Target;
        }
        else
        {
            targetMarker.color = player2Target;
        }
    }

    public void DeselectTarget()
    {
        targetMarker.gameObject.SetActive(false);
        targetMarker.color = Color.white;
    }

    private void OnEnable()
    {
        if (!_anim) return;

        if(hp)
        {
            hp.ResetHealth();
        }

        _anim.Play("Start");
    }

    private IEnumerator Attacking()
    {
        _isAttacking = true;
        int attackState = Random.Range(0, attackAnimStates.Count);

        _anim.Play(attackAnimStates[attackState]);

        yield return new WaitForSeconds(attackIntervals);

        _isAttacking = false;

    }

    public void ShootProjectile()
    {
        var projectile = ObjectPool.SharedInstance.GetEnemyProj();
        var projectileComponenet = projectile.GetComponent<EnemyDamager>();

        projectile.transform.position = projectileSpawnLocation.position;

        projectileComponenet.damage = enemyDamage;
        projectileComponenet.SetVelocity(transform.forward * projectileSpeed);

        projectile.SetActive(true);
    }

    public void Death()
    {
        _rb.velocity = Vector3.zero;
        isAlive = false;
        agent.isStopped = true;
        if(_Tile)
        {
            _Tile.enemyCount--;
            _Tile.EnemyCheck();
        }

        foreach (EffectsCont _ec in particleFX)
        {
            _ec.enabled = true;
        }

        StopAllCoroutines();

        if(_anim)
        {
            _anim.Play("Death");
        }

        if(_cCollider) _cCollider.enabled = false;
        canvas.SetActive(false);
        targetMarker.color = Color.white;
        targetMarker.gameObject.SetActive(false);

        if(_db)
        {
            _db.BossDeath();
            return;
        }

        Invoke("ActiveFalse", 5);
    }

    private void ActiveFalse()
    {
        isAlive = true;
        if (_cCollider) _cCollider.enabled = true;
        canvas.SetActive(true);
        agent.isStopped = false;
        _rb.velocity = Vector3.zero;
        _anim.Play("Idle");
        _isAttacking = false;
        gameObject.SetActive(false);
    }
}
