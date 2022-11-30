using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static InputManager instance;

    public Controls control;

    private void Awake()
    {
        instance = this;
        control = new Controls();
        control.Enable();
    }
}
