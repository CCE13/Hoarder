using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RIPPlayer1 : RIPInputManager
{
    Controls controls;

    private void Awake()
    {
        controls = new Controls();

        controls.RIPGUY.LA.performed += ctx => Attack(_attackManager.LA, 1f);
        controls.RIPGUY.HA.performed += ctx => Attack(_attackManager.HA, 1.5f);
        controls.RIPGUY.LH.started += ctx => CombinedAttack(_attackManager.LH, 1.5f);
        controls.RIPGUY.Movement.performed += ctx => Move(controls.SwordsMan.Movement.name, ctx.ReadValue<Vector3>());
    }
    private void OnEnable() 
    {
        controls.SwordsMan.Enable();
    }

    private void OnDisable()
    {
        controls.SwordsMan.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        _startingTime = timer;
        animator = transform.GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (_startTimer)
        {
            ComboTimer();
        }     
    }

    public override void Move(string name, Vector3 direction)
    {
        movement = direction;
    }

    public void FixedUpdate()
    {
        rb.velocity = new Vector3(movement.x, movement.y,movement.z) * Time.fixedDeltaTime * speed;
    }

}
