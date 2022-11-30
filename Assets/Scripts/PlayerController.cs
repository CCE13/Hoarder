using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player 
{
    public abstract class PlayerController : MonoBehaviour
    {
        public List<Modifier> modifiersCollected;
        public EffectsCont reviveVfx;
        [SerializeField] protected float speed;
        protected Animator anim;
        protected Health health;

        public bool isDead;
        public float turnTime;
        [Header("AnimationStuff")]
        public List<string> boredAnimations;
        public List<string> deathAnimations;
        [SerializeField] protected float clickCount;
        [SerializeField] protected float idleTime;
        [SerializeField] protected float boredThreshHold;
        [SerializeField] protected bool isBored;
        [SerializeField] protected bool isAttacking;

        [Space(20)]
        public ModifierUI modifierUI;

        public Dictionary<string, int> modifier;

        protected GameObject enemySelected;
        public int targetingRange;
        private int enemyToSelect = 0;
        public Vector3 direction
        {
            get
            {
                return _direction;
            }
        }
        public bool canMove;
        public bool canAttack = true;

        public static int damageDealt;
        protected Vector3 _direction;
        protected Rigidbody _rb;
        // Start is called before the first frame update

        protected void GetEnemy(bool player)
        {
            if(enemySelected)
            {
                enemySelected.GetComponent<DungeonEnemy>().DeselectTarget();

                if (!enemySelected.activeInHierarchy) enemySelected = null;
            }

            LayerMask e_layers = LayerMask.GetMask("Enemy");
            Collider[] enemies = Physics.OverlapSphere(transform.position, targetingRange, e_layers, QueryTriggerInteraction.Ignore);

            if(enemies.Length == 0)
            {
                Debug.Log("There are no enemies around " + gameObject.name);

                if (enemySelected) enemySelected = null;
                return;
            }

            if (enemyToSelect >= enemies.Length)
            {
                enemyToSelect = 0;
            }

            if (enemySelected == enemies[enemyToSelect].gameObject)
            {
                if(enemyToSelect + 1 <= enemies.Length - 1)
                {
                    enemySelected = enemies[enemyToSelect + 1].gameObject;
                    enemySelected.GetComponent<DungeonEnemy>().SelectTarget(player);
                    enemyToSelect += 2;
                    return;
                }
                else
                {
                    enemySelected = enemies[0].gameObject;
                    enemySelected.GetComponent<DungeonEnemy>().SelectTarget(player);
                    enemyToSelect = 1;
                    return;
                }
            }

            enemySelected = enemies[enemyToSelect].gameObject;
            enemySelected.GetComponent<DungeonEnemy>().SelectTarget(player);
            
            if(enemyToSelect + 1 >= enemies.Length)
            {
                enemyToSelect = 0;
            }
            else
            {
                enemyToSelect++;
            }


        }

        protected void Move(Vector3 movement)
        {          
            _direction = movement;
            
        }
        protected void Roll()
        {
            bool isRolling = anim.GetBool("isRolling");
            if (isRolling) { return; }
            float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            transform.LookAt(transform.position + moveDir);
            _rb.velocity = Vector3.zero;
            anim.Play("Roll");
        }
        protected void UpdateModifierCount(Modifier modifierToCheck, PlayerController player)
        {
            if (player == this)
            {
                int count = modifier[modifierToCheck.name];
                int newCount = count - 1;
                if (newCount == 0)
                {
                    modifier.Remove(modifierToCheck.name);
                }
                if (newCount > 0)
                {
                    modifier[modifierToCheck.name] = newCount;
                }
            }
        }
        #region Animations
        protected void Idle()
        {
            if (!isBored)
            {
                BoredTimer();
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 > 0.98)
            {
                isBored = false;
                idleTime = 0;
                anim.Play("Idle");
            }
        }
        private void BoredTimer()
        {
            idleTime += Time.deltaTime;
            if (idleTime > boredThreshHold)
            {
                isBored = true;
                int index = UnityEngine.Random.Range(0, boredAnimations.Count);
                string animToPLay = boredAnimations[index];
                anim.Play(animToPLay);
            }

        }

        public void Death()
        {
            StopAllCoroutines();
            canAttack = false;
            isDead = true;
            canMove = false;
            int index = Random.Range(0, deathAnimations.Count);
            anim.Play(deathAnimations[index]);

        }
        #endregion

        /// <summary>
        /// Adds the modifier count to a dictionary.
        /// Checks if the dictionary contains a key of the modifier
        /// If it does, add 1 to the value of the key,
        /// If not, create a key and assign the value 1 to it
        /// </summary>
        /// <param name="modifierToAdd"></param>
        protected abstract void ModifierCounter(Modifier modifierToAdd);

        public void MovePlayer( float multiplier )
        {
            _rb.velocity = transform.forward * (speed / 2) * multiplier;
        }
    }
}


