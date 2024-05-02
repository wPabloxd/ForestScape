using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterTrigger : MonoBehaviour
{
    Encounter encounter;
    private void Awake()
    {
        encounter = GetComponentInParent<Encounter>();
    }
    private void OnTriggerEnter(Collider other)
    {
        encounter.NotifyTriggered();
    }
}
