using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audible : MonoBehaviour
{
    public float range = 10f;
    [SerializeField] string allegiance;
    [SerializeField] float emissionFrequency = 5f;
    [SerializeField] float speedThresholdToEmit = 0f;
    [SerializeField] bool emitOnce;
    float lastEmissionTime;
    Vector3 lastEmissionPosition;
    private void Start()
    {
        lastEmissionTime = Time.time + Random.Range(0f, 1f / emissionFrequency);
        lastEmissionPosition = transform.position;
    }
    private void OnEnable()
    {
        if (emitOnce)
        {
            Emit();
        }
    }
    private void Update()
    {
        float currentSpeed = (transform.position - lastEmissionPosition).magnitude * Time.deltaTime;
        if (currentSpeed >= speedThresholdToEmit && !emitOnce)
        {
            if (Time.time - lastEmissionTime > (1f / emissionFrequency))
            {
                lastEmissionTime = Time.time;
                Emit();
            }
                lastEmissionPosition = transform.position;
        }
    }
    private void Emit()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);
        foreach(Collider c in colliders)
        {
            c.GetComponentInParent<EntityAudition>()?.NotifyAudible(this);
        }
        if(emitOnce)
        {
            enabled = false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    public string GetAllegiance()
    {
        return allegiance;
    }
}