using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsCont : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;

    private void OnEnable()
    {
        PlayParticles();
    }

    private void OnDisable()
    {
        StopParticles();
    }

    public void PlayParticles()
    {
        foreach(ParticleSystem _p in particleSystems)
        {
            _p.Play();
        }
    }

    public void StopParticles()
    {
        foreach (ParticleSystem _p in particleSystems)
        {
            _p.Stop();
        }
    }
}
