using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Barrel : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool debugShot;
    [SerializeField] bool debugContinuousShot;

    private void OnValidate()
    {
        if (debugShot)
        {
            debugShot = false;
            Shot();
        }
        if (debugContinuousShot)
        {
            StartShooting();
        }
        else
        {
            StopShooting();
        }
    }
    public virtual void Shot() { }
    public virtual void StartShooting() { }
    public virtual void StopShooting() { }
    public virtual void BurstShooting() { }
    public virtual void Reload() { }
}
