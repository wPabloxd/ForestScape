using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] bool debugStartWave;
    [SerializeField] bool debugHasFinished;
    void Awake()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    private void OnValidate()
    {
        if (debugStartWave)
        {
            debugStartWave = false;
            StartWave();
        }
    }
    private void Update()
    {
        debugHasFinished = HasFinished();
    }
    public void StartWave()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    public bool HasFinished()
    {
        return transform.childCount == 0;
    }
}
