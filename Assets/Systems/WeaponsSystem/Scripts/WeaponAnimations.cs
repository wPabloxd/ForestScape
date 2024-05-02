using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAnimations : MonoBehaviour
{
    [SerializeField] protected bool isPlayer;
    protected IEntityAnimable entityAnimable;
    public bool reloading;
    public virtual void Moving(Vector3 lastVelocity, bool isRunning, bool isAiming) { }
    public virtual void Shooting() { }
    public virtual void Reloading(bool empty) { }
    public virtual void Empty() { }
    public virtual void EmptyShot() { }
    public virtual void CancelReload() { }
    public virtual void Holstering() { }
    public virtual void MeleeAttack() { }
    public virtual void EndReload() { }
}