using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextRGB : MonoBehaviour
{
    public TMP_Text text;
    public Gradient rgb;


    private void Update()
    {
        float rgbtq = Mathf.PingPong(Time.time / 3, 1);
        text.color = rgb.Evaluate(rgbtq);
    }
}
