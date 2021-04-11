using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FlameCircle : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;
    public float flameSpeed = 200;
    public float activeTime = 5;

    new bool enabled;
    bool ready;
    float timeLeft;
    void Start()
    {
        Activate();
        transform.Rotate(Vector3.forward, Random.value * 360);
    }

    void OnEnable()
    {
        particleSystems ??= new List<ParticleSystem>();
    }

    public void Activate()
    {
        timeLeft = activeTime;
        ready = false;
        
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Play(true);
            var shape = ps.shape;
            shape.arc = 0;
        }
        
        enabled = true;
    }

    public void Disable()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        enabled = false;
    }

    void Update()
    {
        if (!enabled) return;
        
        foreach (ParticleSystem ps in particleSystems)
        {
            var shape = ps.shape;
            shape.arc += flameSpeed * Time.deltaTime;
            
            if (shape.arc >= 360)
            {
                shape.arc = 360;
                ready = true;
            }
        }

        if (!ready) return;
        
        timeLeft -= Time.deltaTime;
        if (timeLeft > 0) return;
        
        foreach (ParticleSystem ps in particleSystems)
        {
            var emission = ps.emission;
            emission.rateOverTimeMultiplier *= 0.95f;
            
            if (emission.rateOverTimeMultiplier <= 10)
            {
                Disable();
            }
        }
    }
}
