using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPoint : MonoBehaviour
{
    new ParticleSystem particleSystem;
    void Start() {
        particleSystem.Stop();
    }

    void Activate()
    {
    }

    void Disable()
    {
        particleSystem.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
