using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingMenu : MonoBehaviour
{
    [SerializeField] IngameMenu ingameMenu;
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            ingameMenu.FinishMenu();
        }
    }
}