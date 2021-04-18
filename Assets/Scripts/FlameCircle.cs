using System;
using System.Collections.Generic;
using UnityEngine;

public class FlameCircle : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;
    public float flameSpeed = 200;
    public float activeTime = 5;
    
    public float angle;
    public CapsuleCollider flamesCollider;
    public int collidersCount;
    
    float radius = 3f;

    public float Radius
    {
        get => radius;
        set => radius = Mathf.Clamp(value, 1, 3);
    }
    

    new bool enabled;
    bool ready;
    float timeLeft;
    void Start()
    {
        Activate();
        transform.Rotate(Vector3.up, angle);

        for (var i = 0; i < collidersCount; i++)
        {
            var collider = gameObject.AddComponent(flamesCollider.GetType()) as CapsuleCollider;
            if (collider is null) continue;
            
            collider.center = new Vector3(
                Mathf.Cos(2f*Mathf.PI * i/collidersCount) * Radius, 
                flamesCollider.center.y,
                Mathf.Sin(2f*Mathf.PI * i/collidersCount) * Radius);
            collider.radius = flamesCollider.radius;
            collider.height = flamesCollider.height;
            collider.isTrigger = flamesCollider.isTrigger;
        }
        
        flamesCollider.enabled = false;
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
            shape.radius = Radius;
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
                Destroy(gameObject);
            }
        }
    }
}
