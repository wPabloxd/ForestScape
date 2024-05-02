using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LookingInLastSeenPosition : AIState
{
    public UnityEvent onLastTimeSeenPositionReached;
    public Vector3 lastTimeSeenPosition;
    [SerializeField] float reachingDistance = 1f;
    private void Update()
    {
        navMeshAgent.SetDestination(lastTimeSeenPosition);
        if(Vector3.Distance(lastTimeSeenPosition, transform.position) < reachingDistance)
        {
            onLastTimeSeenPositionReached.Invoke();
        }
    }
}
