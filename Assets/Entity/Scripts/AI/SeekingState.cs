using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekingState : AIState
{
    private void Update()
    {
        navMeshAgent.SetDestination(decissionMaker.target.transform.position);
    }
}
