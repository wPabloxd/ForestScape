using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelByParticles : Barrel
{
    ParticleSystem.EmissionModule emission;

    private void OnValidate()
    {

    }
    private void Awake()
    {
        emission = GetComponentInChildren<ParticleSystem>().emission;
    }
    private void Start()
    {
        emission.enabled = false;
    }
    public override void StartShooting()
    {
        emission.enabled = true;
    }
    public override void StopShooting()
    {
        emission.enabled = false;
    }
}
