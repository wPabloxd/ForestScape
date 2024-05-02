using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityRagdollizer : MonoBehaviour
{
    [SerializeField] bool startAsRagdoll;
    [SerializeField] bool isPlayer;
    Collider[] colliders;
    [SerializeField] Collider[] weaponColliders;
    Rigidbody[] rigidbodies;
    [SerializeField] Rigidbody[] weaponRigidbodies;
    [Header("Debug")]
    [SerializeField] bool debugRagdollize;
    [SerializeField] bool debugDeragdollize;
    [SerializeField] Vector3 debugDirection;
    [SerializeField] bool debugPush;
    [SerializeField] float debugMinForce;
    [SerializeField] float debugMaxForce;


    private void OnValidate()
    {
        if(debugRagdollize)
        {
            debugRagdollize = false;
            Ragdollize();
        }
        if (debugDeragdollize)
        {
            debugDeragdollize = false;
            Deragdollize();
        }
        if(debugPush)
        {
            debugPush = false;
            Push(debugDirection.normalized, debugMinForce, debugMaxForce);
        }
    }
    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
    }
    private void Start()
    {
        if (!startAsRagdoll)
        {
            Deragdollize();
        }
    }
    public void Ragdollize()
    {
        if (isPlayer)
        {
            foreach (Collider c in colliders)
            {
                c.enabled = true;
            }
        }
        foreach (Collider c in weaponColliders)
        {
            c.enabled = true;
        }
        foreach (Rigidbody r in rigidbodies)
        {
            r.isKinematic = false;
        }
        foreach (Rigidbody r in weaponRigidbodies)
        {
            r.gameObject.GetComponent<Animator>().enabled = false;
            r.isKinematic = false;
            Transform cachedTransform = r.gameObject.transform;
            r.gameObject.transform.parent = null;
            r.gameObject.transform.position = cachedTransform.position;
            r.gameObject.transform.rotation = cachedTransform.rotation;
            r.gameObject.transform.localScale = cachedTransform.localScale;
            if(!isPlayer)
            {
                r.GetComponent<AmmoPickUp>().enablePickUp = true;
            }
        }
    }
    public void Deragdollize()
    {
        if(isPlayer)
        {
            foreach (Collider c in colliders)
            {
                c.enabled = false;
            }
        }
        foreach(Rigidbody r in rigidbodies)
        {
            r.isKinematic = true;
        }
        foreach (Rigidbody r in weaponRigidbodies)
        {
            r.isKinematic = true;
        }
    }
    public void Push(Vector3 direction, float minForce, float maxForce)
    {
        foreach(Rigidbody r in rigidbodies)
        {
            r.AddForce(direction.normalized * UnityEngine.Random.Range(minForce, maxForce));
        }
    }
}