using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IEntityAnimable
{
    NavMeshAgent agent;
    //EntitySight entitySight;
    //PatrolState patrolState;
    //MeleeAttackState meleeAttackState;
    void Awake()
    {
        //patrolState = GetComponent<PatrolState>();
        //meleeAttackState = GetComponent<MeleeAttackState>();
        //entitySight = GetComponentInChildren<EntitySight>();
        agent = GetComponent<NavMeshAgent>();
    }
    public Vector3 GetLastVelocity()
    {
        return agent.velocity;
    }
    public float GetVerticalVelocity()
    {
        return 0;
    }
    public float GetJumpSpeed()
    {
        return 0;
    }
    public bool IsGrounded()
    {
        return true;
    }
    public bool IsRunning()
    {
        return false;
    }
    public bool IsAiming()
    {
        return false;
    }
}