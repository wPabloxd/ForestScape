using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Camera mainCam;
    private void Start()
    {
        mainCam = Camera.main;
    }
    void Update()
    {
        transform.LookAt(mainCam.transform);
    }
}
