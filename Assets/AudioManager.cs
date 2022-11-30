using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public AudioMixerGroup mainMixer;
    public List<AudioSources> audioSources;
    public void Play(string name)
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            if (audioSources[i].name == name)
            {
                audioSources[i].source.Play();
            }
        }
    }

    [Serializable]
    public class AudioSources
    {
        public string name;
        public AudioSource source;
    }
}
