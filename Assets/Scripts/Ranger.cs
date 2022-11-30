using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Player
{
    public class Ranger : PlayerController,IAttack
    {     
        [Space(10)]
        public float bulletSpeed;

        private float turnSmoothVelocity;
        private Transform cam;
        private Controls controls;

        private Transform shootPoint;
        public static event Action<Modifier,PlayerController> ModifierCollected;

        private void Awake()
        {
            controls = InputManager.instance.control;
            controls.Ranger.Movement.performed += ctx => Move(ctx.ReadValue<Vector3>());
            controls.Ranger.Attack.performed += ctx => Attack();
            controls.Ranger.SelectEnemy.performed += ctx => GetEnemy(true);
            controls.Ranger.Roll.performed += ctx => Roll();
            cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
            _rb = GetComponent<Rigidbody>();
            shootPoint = transform.Find("Shooting Point");
            anim = GetComponentInChildren<Animator>();
            health = GetComponent<Health>();
        }

        private void Start()
        {
            modifier = new Dictionary<string, int>();
            canMove = true;
            PlayerDamageStats.modifierRemoved += UpdateModifierCount;
            Monuments.Reviving += Revived;
        }

        private void OnEnable()
        {
            controls.Enable();
        }

        private void OnDestroy()
        {
            PlayerDamageStats.modifierRemoved += UpdateModifierCount;
            Monuments.Reviving -= Revived;
        }

        public void Revived(PlayerController player,Transform posToRevive)
        {
            if(player != this) { return; }
            isDead = false;
            canAttack = true;
            canMove = true;
            reviveVfx.PlayParticles();
            anim.Play("Idle");
            health.ResetHealth();
            transform.position = posToRevive.position + Vector3.one;
        }


        /// <summary>
        /// Uses the object pool to get arrow and attack
        /// </summary>
        public void Attack()
        {
            if (!canAttack) return;
            if (enemySelected && enemySelected.GetComponent<Health>().currentHealth <= 0)
            {
                enemySelected = null;
            }

            // Check if there is any attack animations playing.

            if(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1") || anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
            {
                return;
            }

            _rb.velocity = Vector3.zero;

            // Rotates the _player so that the _player faces enemy.
            if (enemySelected)
            {
                transform.LookAt(new Vector3(enemySelected.transform.position.x, transform.position.y, enemySelected.transform.position.z));
            }

            if(clickCount == 2 && enemySelected)
            {
                anim.Play("Attack2");
                return;
            }

            anim.Play("Attack1");
        }

        public void ShootArrow()
        {
            if(clickCount == 2)
            {
                clickCount = 0;
                // Check if there's an enemy selected, if not try to get enemy, if unable to do so, return.
                if (!enemySelected) GetEnemy(true);
                if (!enemySelected)
                {
                    Debug.Log("No enemy");
                }

                if(enemySelected)
                {
                    ComboShot(enemySelected);
                    return;
                }
            }

            GameObject arrowToShoot = ObjectPool.SharedInstance.GetArrow();
            arrowToShoot.transform.position = shootPoint.position;
            arrowToShoot.transform.rotation = transform.rotation;
            arrowToShoot.SetActive(true);

            if(enemySelected)
            {
                StartCoroutine(Attack_C(arrowToShoot, shootPoint.position, enemySelected));
                clickCount++;
                return;
            }

            ArrowSetup(arrowToShoot);

            clickCount++;
        }

        public void ComboShot(GameObject _enemy)
        {

            // Spawn arrows
            int numberOfArrows = 5;
            int arrowSpawned = 0;
            float pivotOffsetX = -10;

            // While loop to spawn arrows.
            while(arrowSpawned < numberOfArrows)
            {
                GameObject arrowToShoot = ObjectPool.SharedInstance.GetArrow();
                arrowToShoot.transform.SetPositionAndRotation(shootPoint.position, transform.rotation);
                arrowToShoot.GetComponent<Arrow>().isPiercing = true;
                arrowToShoot.SetActive(true);

                StartCoroutine(Combo_C(arrowToShoot, shootPoint.position, _enemy, pivotOffsetX));

                pivotOffsetX += 5;

                arrowSpawned++;
            }
        }

        IEnumerator Attack_C(GameObject arrow, Vector3 start, GameObject target)
        {
            // Lerp the arrow so that it never misses.
            float timePassed = 0;
            float timeToTake = Vector3.Distance(start, target.transform.position) / 30;

            if (timeToTake < 0.1) timeToTake = 0.1f;

            // Transferring references.
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            WeaponDamager damager = arrow.GetComponent<WeaponDamager>();
            damager.directionToKnockBack = rb.velocity.normalized;
            damager.player = this;
            damager.playerDamage = GetComponent<PlayerDamageStats>();

            // Lerping the arrow so that it never misses the target.
            Vector3 prevPos = arrow.transform.position;

            while (timePassed < timeToTake)
            {
                prevPos = arrow.transform.position;

                arrow.transform.position = Vector3.Lerp(start, target.transform.position, timePassed / timeToTake);
                arrow.transform.position = new Vector3(arrow.transform.position.x, shootPoint.position.y, arrow.transform.position.z);

                Vector3 direction = (arrow.transform.position - prevPos).normalized;
                arrow.transform.rotation = Quaternion.LookRotation(direction);

                timePassed += Time.deltaTime;
                yield return null;
            }

            arrow.transform.rotation = Quaternion.Euler(Vector3.zero);
            arrow.SetActive(false);

        }

        IEnumerator Combo_C(GameObject arrow, Vector3 start ,GameObject target, float pivotOffset)
        {
            // Slerp the arrow based on the pivot offset relative to direction.

            float timePassed = 0;
            float timeToTake = Vector3.Distance(start, target.transform.position) / 25;

            if (timeToTake < 0.1) timeToTake = 0.1f;

            Vector3 centerPivot = (start + target.transform.position) * 0.5f;

            centerPivot += new Vector3(pivotOffset, 0, pivotOffset) ;

            var relativeStart = start - centerPivot;
            var relativeEnd = target.transform.position - centerPivot;

            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            WeaponDamager damager = arrow.GetComponent<WeaponDamager>();
            damager.directionToKnockBack = rb.velocity.normalized;
            damager.player = this;
            damager.playerDamage = GetComponent<PlayerDamageStats>();

            Vector3 prevPos = arrow.transform.position;

            while (timePassed < timeToTake)
            {
                prevPos = arrow.transform.position;

                arrow.transform.position = Vector3.Slerp(relativeStart, relativeEnd, timePassed / timeToTake) + centerPivot;
                arrow.transform.position = new Vector3(arrow.transform.position.x, shootPoint.position.y, arrow.transform.position.z);

                Vector3 direction = (arrow.transform.position - prevPos).normalized;
                arrow.transform.rotation = Quaternion.LookRotation(direction);

                timePassed += Time.deltaTime;
                yield return null;
            }

            arrow.transform.rotation = Quaternion.Euler(Vector3.zero);
            arrow.GetComponent<Arrow>().isPiercing = false;
            arrow.SetActive(false);
        }

        /// <summary>
        /// Moves the <paramref name="arrowToShoot"/> forward and stores the assigns the variables into the arrow.
        /// </summary>
        /// <param name="arrowToShoot"></param>
        private void ArrowSetup(GameObject arrowToShoot)
        {
            Rigidbody rb = arrowToShoot.GetComponent<Rigidbody>();
            rb.velocity = transform.forward * bulletSpeed;
            WeaponDamager damager = arrowToShoot.GetComponent<WeaponDamager>();
            damager.directionToKnockBack = rb.velocity.normalized;
            damager.player = this;
            damager.playerDamage = GetComponent<PlayerDamageStats>();
        }

        // Update is called once per frame
        public void FixedUpdate()
        {
            if (isDead)
            {
                _direction = Vector3.zero;
                _rb.velocity = Vector3.zero;
                return;
            }
            if (!canMove) { return; }
            if (anim.GetBool("isAttacking")) { return;}
            //Cheks if the _player is moving
            //If is moving, move the _player based on the camera angle
            if (_direction.magnitude > 0.3f)
            { //only update rotation if _player is moving

                idleTime = 0f;

                float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _rb.velocity = moveDir * speed;

                anim.Play("Walk");
            }
            else
            {
                Idle();
                _rb.velocity = Vector3.zero;
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
                Debug.Log(modifier[modifierToAdd.name] + modifierToAdd.name);
            }
            else
            {
                modifier[modifierToAdd.name] = 1;
            }
        }
    }
}

