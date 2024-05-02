using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hitbox : MonoBehaviour
{
    [SerializeField] public UnityEvent onHit;
    [SerializeField] float damage = 1f;
    [SerializeField] public UnityEvent<Collider> onHitWithCollider;
    [SerializeField] public UnityEvent OnCollisionWithoutHit;
    private void OnTriggerEnter(Collider other)
    {
        CheckCollider(other);
    }
    private void OnCollisionEnter(Collision collision)
    {
        CheckCollider(collision.collider);
    }
    private void CheckCollider(Collider other)
    {
        other.GetComponent<Hurtbox>()?.NotifyHit(this, damage);
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox)
        {
            hurtbox.NotifyHit(this, damage);
            onHit.Invoke();
            onHitWithCollider.Invoke(other);
        }
        else
        {
            OnCollisionWithoutHit.Invoke();
        }
    }
}
