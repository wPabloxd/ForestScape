using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingState : AIState
{
    [SerializeField] float shotPerSecondsInSemi = 2;
    float lastShotTime = 0;
    protected bool isDead;
    public override void Enter()
    {
        navMeshAgent.SetDestination(transform.position);
        if (entityWeapons.GetCurrentWeapon().shotMode == Weapon.ShotMode.fullauto)
        {
            entityWeapons.StartShooting();
        }
    }
    void Update()
    {
        PreUpdate();
        UpdateOrientation();
        UpdateShoot();
        PostUpdate();
    }
    protected virtual void PreUpdate() { }
    protected virtual void PostUpdate() { }
    public void HasDied()
    {
        isDead = true;
        Exit();
        this.enabled = false;
    }
    private void UpdateShoot()
    {
        if (entityWeapons.GetCurrentWeapon().shotMode == Weapon.ShotMode.semiauto && !isDead)
        {
            if ((Time.time - lastShotTime) > (1f / shotPerSecondsInSemi))
            {
                entityWeapons.Shot();
                lastShotTime = Time.time;
            }
        }
    }
    private void UpdateOrientation()
    {
        Vector3 desiredDirection = decissionMaker.target.position - transform.position;
        desiredDirection = Vector3.ProjectOnPlane(desiredDirection, Vector3.up);

        float angularDistance = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);
        float angleToApply = navMeshAgent.angularSpeed * Time.deltaTime;

        angleToApply = Mathf.Min(angleToApply, Mathf.Abs(angularDistance));
        angleToApply *= Mathf.Sign(angularDistance);

        Quaternion rotationToApply = Quaternion.AngleAxis(angleToApply, Vector3.up);
        transform.rotation = rotationToApply * transform.rotation;
    }
    public override void Exit()
    {
        if (entityWeapons.GetCurrentWeapon().shotMode == Weapon.ShotMode.fullauto)
        {
            entityWeapons.StopShooting();
        }
    }
}
