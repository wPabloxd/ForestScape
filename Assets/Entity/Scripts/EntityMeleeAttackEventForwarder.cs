using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMeleeAttackEventForwarder : MonoBehaviour
{
    EntityMeleeAttack entityMeleeAttack;

    private void Awake()
    {
        entityMeleeAttack = GetComponentInParent<EntityMeleeAttack>();
    }
    public void Attack()
    {
        Debug.Log("PEGA");
        entityMeleeAttack.PerformAttack();
    }
}
