using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPoint : MonoBehaviour
{
    new ParticleSystem particleSystem;
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    public void Activate()
    {
        particleSystem.Play(true);
    }

    public void Disable()
    {
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
