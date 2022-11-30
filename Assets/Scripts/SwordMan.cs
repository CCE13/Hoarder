using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Player
{
    public class SwordMan : PlayerController,IAttack
    {
        [Space(10)]

        private float turnSmoothVelocity;
        private Transform cam;
        private Controls controls;
        public static event Action<Modifier,PlayerController> ModifierCollected;

        public WeaponDamager weaponDamager;

        private void Awake()
        {
            controls = InputManager.instance.control;
            controls.SwordsMan.Movement.performed += ctx => Move(ctx.ReadValue<Vector3>());
            controls.SwordsMan.Attack.performed += ctx => Attack();
            controls.SwordsMan.SelectEnemy.performed += ctx => GetEnemy(false);
            controls.SwordsMan.Roll.performed += ctx => Roll();
            cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
            _rb = GetComponent<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
            health = GetComponent<Health>();   
            PlayerDamageStats.modifierRemoved += UpdateModifierCount;
            Monuments.Reviving += Revived;
        }

        void Start()
        {

            modifier = new Dictionary<string, int>();
            canMove = true;
        }
        private void OnDestroy()
        {
             PlayerDamageStats.modifierRemoved += UpdateModifierCount;
             Monuments.Reviving -= Revived;
        }


        private void OnEnable()
        {
            controls.Enable();
        }


        public void Revived(PlayerController player,Transform positionToSpawn)
        {
            if (player != this) { return; }
            isDead = false;
            canAttack = true;
            canMove = true;
            reviveVfx.PlayParticles();
            anim.Play("Idle");
            health.ResetHealth();
            transform.position = positionToSpawn.position + Vector3.one;
        }

        // Update is called once per frame
        public void FixedUpdate()
        {
            if(isDead)
            {
                _direction = Vector3.zero;
                _rb.velocity = Vector3.zero;
                return;
            }
            isAttacking = anim.GetBool("isAttacking");
            bool isRolling = anim.GetBool("isRolling");
            if (isRolling) { return; }
            if (!canMove) { return; }
            if (isAttacking) { return; }
            //move sthe _player based off the camera angle.
            if (_direction.magnitude > 0.1f)
            { //only update rotation if _player is moving
                idleTime = 0f;
                clickCount = 0;
                float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _rb.velocity = moveDir * speed;
                // Does character go out of view?
                anim.SetBool("isAttacking", false);
                anim.SetBool("Slash2", false);
                anim.SetBool("Slash3", false);
                anim.Play("Walk");    
            }
            else
            {
                Idle();
                clickCount = 0;
                _rb.velocity = Vector3.zero;
                anim.SetBool("isAttacking", false);
                anim.SetBool("Slash2", false);
                anim.SetBool("Slash3", false);
                
            }

        }
        private void OnTriggerEnter(Collider other)
        {
            //when players collects a modifiersCollected, add to the list
            if (other.CompareTag("Modifier"))
            {
                ModiferCollectable modifierCol = other.GetComponent<ModiferCollectable>();
                if (modifierCol._hasCollected) return;

                modifierCol._hasCollected = true;
                Modifier modifierToAdd = other.GetComponent<ModiferCollectable>().modifier;
                modifiersCollected.Add(modifierToAdd);

                ModifierCounter(modifierToAdd);

                ModifierCollected?.Invoke(modifierToAdd,this);
                modifierUI.ReloadList(this);
                //add to lists
                Destroy(other.gameObject);
            }
        }


        protected override void ModifierCounter(Modifier modifierToAdd)
        {
            if (modifier.ContainsKey(modifierToAdd.name))
            {
                modifier[modifierToAdd.name] = modifier[modifierToAdd.name] + 1;
            }
            else
            {
                modifier[modifierToAdd.name] = 1;
                
            }
            Debug.Log(modifier[modifierToAdd.name] + modifierToAdd.name);
        }

        public void Attack()
        {
            if (!canAttack) return;
            if (isDead) return;
            if (enemySelected && enemySelected.GetComponent<Health>().currentHealth <= 0 || enemySelected && !enemySelected.activeSelf)
            {
                enemySelected = null;
            }

            if (!enemySelected) GetEnemy(false);
            if (!enemySelected)
            {
                Debug.Log("No enemy");
            }

            if (enemySelected)
            {
                transform.LookAt(new Vector3(enemySelected.transform.position.x, transform.position.y, enemySelected.transform.position.z));
            }

            //increase clickcount by one
            // reset the idle time to 0
            clickCount++;
            idleTime = 0f;

            //play animation based on the nuber of clicks registered
            if (clickCount == 1)
            {
                anim.Play("Slash");
                weaponDamager.multiplier = 1f;
            }
            if(clickCount >= 2 && anim.GetCurrentAnimatorStateInfo(0).length> 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("Slash"))
            {
                anim.SetBool("Slash2", true);
                weaponDamager.multiplier = 1.5f;
            }
            if (clickCount >= 3 && anim.GetCurrentAnimatorStateInfo(0).length > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("Slash1"))
            {
                anim.SetBool("Slash3", true);
                weaponDamager.multiplier = 2f;
            }
        }
    }
}

