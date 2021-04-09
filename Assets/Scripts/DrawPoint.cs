using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPoint : MonoBehaviour
{
    new ParticleSystem particleSystem;
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Stop();
    }

    void Activate()
    {
        particleSystem.Play();
    }

    void Disable()
    {
        particleSystem.Stop();
    }
}
