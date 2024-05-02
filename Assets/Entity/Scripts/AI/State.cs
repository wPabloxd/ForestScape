using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIState : MonoBehaviour
{
    public AIDecisionMaker decissionMaker;
    protected EntityWeapons entityWeapons;
    protected NavMeshAgent navMeshAgent;
    protected Animator animator;
    public Transform target;
    private void Awake()
    {
        entityWeapons = GetComponent<EntityWeapons>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }
    public virtual void Enter() { }
    public virtual void Exit() { }
}