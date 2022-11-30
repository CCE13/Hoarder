using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
public abstract class RIPInputManager : MonoBehaviour
{
    public string comboHolder;
    public float timer;
    public float speed;


    [Space(20)]
    public List<Combos> comboList;
    public Attacks currentAttack;

    protected bool _startTimer;
    protected float _startingTime;

    protected Animator animator;
    protected Rigidbody rb;
    protected Vector3 movement;
    protected RIPAttackManager _attackManager;                        
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        Application.targetFrameRate = 60;
    }

    public abstract void Move(string name, Vector3 direction);


    protected void Attack(Attacks attack,float timerMultiplier)
    {
        
        comboHolder += attack._name;
        currentAttack = attack;
        ResetTimer(timerMultiplier);
        if (animator.GetBool("AnimPlaying")) { return; }      
        animator.Play(attack._name);

        _startTimer = true;
    }
    protected void CombinedAttack(Attacks attack, float timerMultiplier)
    {
        if(comboHolder.Length>4)
        {
            comboHolder = comboHolder.Remove(comboHolder.Length - 4, 4);
        }  
        comboHolder += attack._name;
        currentAttack = attack;
        ResetTimer(timerMultiplier);

        Debug.Log(attack._name);

        animator.Play(attack._name);
        _startTimer = true;
    }
    //checks if the Inputted combosystem is contained in the string.
    private void CheckCombo(string comboName)
    {
        foreach (Combos combos in comboList)
        {
           if(combos._name == comboName)
            {
                currentAttack = combos;
                Debug.Log(comboName + " Combo Hit!");
                //playAnimation
                animator.Play(currentAttack._name);
                break;
            }
            else
            {
                Debug.Log("no have");
            }
        }

    }
    protected void ComboTimer()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            ResetTimer(1);
            _startTimer = false;
            CheckCombo(comboHolder);
            comboHolder = string.Empty;
        }
    }

    private void ResetTimer(float multiplier)
    {
        timer = _startingTime * multiplier;
    }
}
