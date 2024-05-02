using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SetStartVelocity : MonoBehaviour
{
    [SerializeField] float startVelocity = 10f;

    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * startVelocity;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
