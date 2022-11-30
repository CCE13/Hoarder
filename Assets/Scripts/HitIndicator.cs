using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitIndicator : MonoBehaviour
{
    public Color start;
    public Color end;

    private float status;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        status = Mathf.PingPong(Time.fixedTime * 2, 1);
        var output = Color.Lerp(start, end, status);
        sr.color = output;
    }

    private void OnEnable()
    {
        sr.color = start;   
    }
}
