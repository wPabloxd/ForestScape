using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : AIState
{
    void Update()
    {
        navMeshAgent.destination = transform.position;   
    }
}
