using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPoint : MonoBehaviour
{
    public List<Light> lights;
    public List<ParticleSystem> particleSystems;
    void Start()
    {
        Disable();
    }

    public void Activate()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Play(true);
        }
        foreach (Light l in lights)
        {
            l.enabled = true;
        }
    }

    public void Disable()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        foreach (Light l in lights)
        {
            l.enabled = false;
        }
    }
}
