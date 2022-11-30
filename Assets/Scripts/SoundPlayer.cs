using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPlayer : MonoBehaviour
{
    public bool playOnStart;
    public AudioMixer audioMixer;
    public List<AudioSource> audioSources;

    public List<AudioSource> audioEntrys;
    public List<AudioSource> audioLoops;

    public void Start()
    {

        audioMixer.SetFloat("Volume", PlayerPrefs.GetFloat("VolumeAudio"));
        if (!playOnStart)
        {
            return;
        }

        StartCoroutine(PlayLoopings());
    }

    private IEnumerator PlayLoopings()
    {
        for (int i = 0; i < audioEntrys.Count; i++)
        {
            audioEntrys[i].Play();

            while(audioEntrys[i].isPlaying)
            {
                yield return null;
            }
        }

        for (int i = 0; i < audioLoops.Count; i++)
        {
            audioLoops[i].Play();

            while(audioLoops[i].isPlaying)
            {
                yield return null;
            }
        }
    }

    public void PlayAudio()
    {
        foreach (AudioSource _A in audioSources)
        {
            _A.Play();
        }
    }

    public void PlayLAudio()
    {
        StartCoroutine(PlayLoopings());
    }

    public void StopAudios()
    {
        foreach (AudioSource _a in audioSources)
        {
            _a.Stop();
        }

        foreach (AudioSource _a in audioEntrys)
        {
            _a.Stop();
        }

        foreach (AudioSource _a in audioLoops)
        {
            _a.Stop();
        }
    }
}
