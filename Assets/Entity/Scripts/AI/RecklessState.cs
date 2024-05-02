using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RecklessState : ShootingState
{
    [SerializeField] float seekFactor = 1f;
    protected override void PreUpdate()
    {
        UpdateGetCloserAndMoveAround();
    }
    void UpdateGetCloserAndMoveAround()
    {
        if (!isDead)
        {
            Vector3 desiredPosition = transform.position + Vector3.right;
            if (Vector3.Distance(transform.position, decissionMaker.target.transform.position) > decissionMaker.recklessRange)
            {
                Vector3 direction = decissionMaker.target.position - transform.position;
                desiredPosition += direction.normalized * seekFactor;
            }
            navMeshAgent.SetDestination(desiredPosition);
        }
    }
}