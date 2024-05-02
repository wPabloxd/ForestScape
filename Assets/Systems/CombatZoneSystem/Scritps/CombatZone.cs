using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatZone : MonoBehaviour
{
    [SerializeField] GameObject[] enemies;
    private void Start()
    {
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            foreach (GameObject enemy in enemies)
            {
                enemy.SetActive(true);
            }
        }
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    foreach (Enemy enemy in enemies)
    //    {
    //        enemy.gameObject.SetActive(false);
    //    }
    //}
}
