using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateAroundCamera : MonoBehaviour
{
    [SerializeField] InputActionReference rotateLeft;
    [SerializeField] InputActionReference rotateRight;

    [SerializeField] float acceleration = 1440;
    [SerializeField] float decceleration = 720;
    [SerializeField] float maxSpeed = 720;
    [SerializeField] float currentSpeed;
    private void OnEnable()
    {
        rotateLeft.action.Enable();
        rotateRight.action.Enable();
    }
    private void Update()
    {
        if(rotateLeft.action.IsPressed())
        {
            currentSpeed -= acceleration * Time.deltaTime;
            ClampMaxSpeed();
        }
        else if (rotateRight.action.IsPressed())
        {
            currentSpeed += acceleration * Time.deltaTime;
            ClampMaxSpeed();
        }
        else
        {
            float oldCurrentSpeed = currentSpeed;
            currentSpeed += decceleration * Time.deltaTime * -Mathf.Sign(currentSpeed);
            if(Mathf.Sign(currentSpeed) != Mathf.Sign(oldCurrentSpeed))
            {
                currentSpeed = 0;
            }
        }

  
        Vector3 newEuler = transform.localEulerAngles + (Vector3.up * currentSpeed * Time.deltaTime);
        transform.localEulerAngles = newEuler;
    }

    private void ClampMaxSpeed()
    {
        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
        else if (currentSpeed < -maxSpeed)
        {
            currentSpeed = -maxSpeed;
        }
    }

    private void OnDisable()
    {
        rotateLeft.action.Disable();
        rotateRight.action.Disable();
    }
}
