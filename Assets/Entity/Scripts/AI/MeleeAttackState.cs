using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeAttackState : AIState
{
    [SerializeField] float attackDistance = 1;
    void Update()
    {
        Vector3 destination = decissionMaker.target ? decissionMaker.target.position : transform.position;
        navMeshAgent.SetDestination(destination);
        if (decissionMaker.target)
        {
            animator.SetBool("IsAttacking", decissionMaker.target ? (Vector3.Distance(transform.position, decissionMaker.target.position) < attackDistance) : false);
        }
    }
}