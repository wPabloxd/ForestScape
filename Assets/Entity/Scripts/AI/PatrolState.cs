using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : AIState
{
    [SerializeField] Transform patrolPointsParent;
    [SerializeField] float reachingDistance = 1;
    int currentPoint;

    public override void Enter()
    {
        
    }
    public override void Exit()
    {
        
    }
    private void Update()
    {
        Vector3 destination = patrolPointsParent.GetChild(currentPoint).position;
        navMeshAgent.destination = destination;
        if((transform.position - destination).sqrMagnitude < (reachingDistance * reachingDistance))
        {
            currentPoint++;
            if(currentPoint >= patrolPointsParent.childCount)
            {
                currentPoint = 0;
            }
        }
    }
}   