using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDecisionMaker : MonoBehaviour, IVisible
{
    public enum RangedEnemyType
    {
        NonRanged,
        Reckless,
        Cautious,
        Ambusher,
        Guardian
    }
    [SerializeField] RangedEnemyType rangedEnemyType = RangedEnemyType.NonRanged;
    [SerializeField] string allegiance = "Enemy";
    [SerializeField] float shootRange = 15f;
    [SerializeField] public float recklessRange = 5f;
    [SerializeField] AIState startState;
    EntitySight entitySight;
    IdleState idleState;
    EntityAudition entityAudition;
    MeleeAttackState meleeAttackState;
    PatrolState patrolState;
    EntityWeapons entityWeapons;
    SeekingState seekState;
    RecklessState recklessState;
    ShootingState shootingState;
    LookingInLastSeenPosition lookingInLastSeenPosition;

    Vector3 lastTimeSeenPosition;
    bool hasLastTimeSeenPosition;
    public bool playerDetected;
    bool alreadyAdded;

    AIState[] aiStates;
    AIState currentState;
    public Transform target { get; private set; }
    private void Awake()
    {
        entityAudition = GetComponentInChildren<EntityAudition>();
        lookingInLastSeenPosition = GetComponent<LookingInLastSeenPosition>();
        entityWeapons = GetComponent<EntityWeapons>();
        idleState = GetComponent<IdleState>();
        recklessState = GetComponent<RecklessState>();
        shootingState = GetComponent<ShootingState>();
        seekState = GetComponent<SeekingState>();
        aiStates = GetComponents<AIState>();
        patrolState = GetComponent<PatrolState>();
        entitySight = GetComponentInChildren<EntitySight>();
        meleeAttackState = GetComponent<MeleeAttackState>();
        lookingInLastSeenPosition.onLastTimeSeenPositionReached.AddListener(OnLastTimeSeenPositionReached);
    }
    void OnLastTimeSeenPositionReached()
    {
        hasLastTimeSeenPosition = false;
    }
    void Start()
    {
        foreach(AIState s in aiStates)
        {
            s.decissionMaker = this;
        }
        SetState(startState);
    }
    void Update()
    {
        Transform visibleTarget = entitySight.visiblesInSight.Find((x) => x.GetAllegiance() != GetAllegiance())?.GetTransform();
        Transform audibleTarget = rangedEnemyType == RangedEnemyType.Ambusher ? null : entityAudition.heardAudibles.Find((x) => x.GetAllegiance() != GetAllegiance())?.audible.transform;
        target = null;
        //if (!visibleTarget)
        //{
        //    target = audibleTarget;
        //}
        //else if (audibleTarget)
        //{
        //    target = Vector3.Distance(visibleTarget.position, transform.position) < Vector3.Distance(audibleTarget.position, transform.position) ? visibleTarget : audibleTarget;
        //}
        //else
        //{
        //    target = visibleTarget;
        //}
        if (visibleTarget)
        {
            target = visibleTarget;
        }
        else if (audibleTarget)
        {
            target = audibleTarget;
        }
        bool canSeeTarget = entitySight.visiblesInSight.Find((x) => x.GetTransform() == target) != null;
        bool canHearTarget = entityAudition.heardAudibles.Find((x) => x.audible.transform == target) != null;
        //Debug.Log(target);
        if (target)
        {
            playerDetected = true;
            if (rangedEnemyType == RangedEnemyType.Ambusher)
            {
                rangedEnemyType = RangedEnemyType.Reckless;
            }
            lastTimeSeenPosition = target.position;
            hasLastTimeSeenPosition = rangedEnemyType != RangedEnemyType.Guardian;
            if (entityWeapons && rangedEnemyType != RangedEnemyType.NonRanged)
            {
                if (Vector3.Distance(target.position, transform.position) < shootRange)
                {

                    if (canSeeTarget)
                    {
              
                        if (rangedEnemyType == RangedEnemyType.Reckless)
                        {
                            SetState(recklessState);
                        }
                        else
                        {
                            SetState(shootingState);
                        }
                    }
                    else
                    {
                        SetState(seekState);
                    }            
                }
                else
                {
                    if(rangedEnemyType == RangedEnemyType.Guardian)
                    {
                        SetState(shootingState);
                    }
                    else
                    {
                        SetState(seekState);
                    }
                }
            }
            else
            {
                SetState(meleeAttackState);
            }
        }
        else if (hasLastTimeSeenPosition)
        {
            lookingInLastSeenPosition.lastTimeSeenPosition = lastTimeSeenPosition;
            SetState(lookingInLastSeenPosition);
        }
        else if(rangedEnemyType == RangedEnemyType.Ambusher)
        {
            playerDetected = false;
            SetState(idleState);
        }
        else
        {
            playerDetected = false;
            SetState(patrolState);
        }
        UpdatePlayerDetection();
    }
    void UpdatePlayerDetection()
    {
        if(playerDetected)
        {
            if(!alreadyAdded)
            {
                GameManager.Instance.playerDetectedCounter++;
                alreadyAdded = true;
            }
        }
        else
        {
            if (alreadyAdded)
            {
                GameManager.Instance.playerDetectedCounter--;
                alreadyAdded = false;
            }
        }
    }
    public void Died()
    {
        if (alreadyAdded)
        {
            GameManager.Instance.playerDetectedCounter--;
            alreadyAdded = false;
        }
    }
    private void SetState(AIState newState)
    {
        if(currentState != newState)
        {
            currentState?.Exit();
            foreach (AIState s in aiStates)
            {
                if (s == newState)
                {
                    s.enabled = true;
                    s.Enter();
                }
                else
                {
                    s.enabled = false;
                }
            }
            currentState = newState;
        }    
    }
    #region IVisible Implementation
    public Transform GetTransform()
    {
        return transform;
    }
    public string GetAllegiance()
    {
        return allegiance;
    }
    #endregion
}