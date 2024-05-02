using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class Hurtbox : MonoBehaviour
{
    public UnityEvent<float> onHitNotified;
    public UnityEvent <float, Transform> onHitNotifiedWithOffender;
    public UnityEvent <float, Hitbox> onHitNotifiedWithHitbox;
    public UnityEvent <float, Barrel> onHitNotifiedWithBarrel;
    public UnityEvent <float, Explosion> onHitNotifiedWithExplosion;
    public UnityEvent <float, ParticleHit> onHitNotifiedWithParticleHit;
    public UnityEvent <float, EntityMeleeAttack> onHitNotifiedWithMeleeAttack;
    internal virtual void NotifyHit(Hitbox hitbox, float damage)
    {
        onHitNotified.Invoke(damage);
        onHitNotifiedWithOffender.Invoke(damage, hitbox.transform);
        onHitNotifiedWithHitbox.Invoke(damage, hitbox);
    }
    internal virtual void NotifyHit(Barrel barrelByRaycast, float damage)
    {
        onHitNotified.Invoke(damage);
        onHitNotifiedWithOffender.Invoke(damage, barrelByRaycast.transform);
        onHitNotifiedWithBarrel.Invoke(damage, barrelByRaycast);
    }

    internal void NotifyHit(Explosion explosion, float damage)
    {
        onHitNotified.Invoke(damage);
        onHitNotifiedWithOffender.Invoke(damage, explosion.transform);
        onHitNotifiedWithExplosion.Invoke(damage, explosion);
    }

    internal void NotifyHit(ParticleHit particleHit, float damage)
    {
        onHitNotified.Invoke(damage);
        onHitNotifiedWithOffender.Invoke(damage, particleHit.transform);
        onHitNotifiedWithParticleHit.Invoke(damage, particleHit);
    }
    internal void NotifyHit(EntityMeleeAttack entityMeleeAttack, float damage)
    {
        onHitNotified.Invoke(damage);
        onHitNotifiedWithOffender.Invoke(damage, entityMeleeAttack.transform);
        onHitNotifiedWithMeleeAttack.Invoke(damage, entityMeleeAttack);
    }
}