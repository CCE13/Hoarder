using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateModel : MonoBehaviour
{
    public float speed;
    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, Time.time * speed, 0);
    }

}
