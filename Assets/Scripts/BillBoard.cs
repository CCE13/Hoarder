using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Transform _cam;

    public bool isMoving;

    public void Start()
    {
        _cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        transform.LookAt(transform.position + _cam.forward);
    }

    public void LateUpdate()
    {
        if (isMoving) transform.LookAt(transform.position + _cam.forward);
    }
}
