using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEnable : MonoBehaviour
{
    public AudioSource audio;

    private void OnEnable()
    {
        audio.Play();
    }
}
