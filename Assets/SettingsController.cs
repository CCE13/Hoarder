using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    private void Awake()
    {
        float volumeToSetSlider = PlayerPrefs.GetFloat("VolumeSlider");
        volumeSlider.value = volumeToSetSlider;


        audioMixer.SetFloat("Volume", PlayerPrefs.GetFloat("VolumeAudio"));
    }

    public void VolumeChange(float volume)
    {
        var volumeToSet = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("Volume",volumeToSet );

        PlayerPrefs.SetFloat("VolumeSlider", volume);
        PlayerPrefs.SetFloat("VolumeAudio",volumeToSet);
    }
}
