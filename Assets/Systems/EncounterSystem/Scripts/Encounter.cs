using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Encounter : MonoBehaviour
{
    [SerializeField] Transform encounterDoorParent;
    [SerializeField] Transform encounterLimitsParent;
    [SerializeField] Transform target;
    [SerializeField] UnityEvent onEncounterFinished;
    Wave[] waves;
    int currentWave;
    [Header("Debug")]
    [SerializeField] bool debugStartEncounter;
    [SerializeField] bool debugHasFinished;
    private void OnValidate()
    {
        if (debugStartEncounter)
        {
            debugStartEncounter = false;
            StartEncounter();
        }
    }
    private void Awake()
    {
        SetDoorsActivation(false);
        waves = GetComponentsInChildren<Wave>();
        //Enemy[] enemies = GetComponentsInChildren<Enemy>(true);
        //foreach (Enemy e in enemies)
        //{
        //    e.target = target;
        //}
    }
    void StartEncounter()
    {
        SetDoorsActivation(true);
        waves[currentWave].StartWave();
    }
    private void Update()
    {
        if(currentWave < waves.Length)
        {
            if (waves[currentWave].HasFinished())
            {
                currentWave++;
                if (currentWave < waves.Length)
                {
                    waves[currentWave].StartWave();
                }
                else
                {
                    onEncounterFinished.Invoke();
                    SetDoorsActivation(false);
                }
            }
        }
        debugHasFinished = HasFinished();
    }
    public bool HasFinished()
    {
      
        return currentWave >= waves.Length;
    }
    void SetDoorsActivation(bool activation)
    {
        foreach (Transform door in encounterDoorParent)
        {
            door.gameObject.SetActive(activation);
        }
    }
    internal void NotifyTriggered()
    {
        StartEncounter();
    }
}
