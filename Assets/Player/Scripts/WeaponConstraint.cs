using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponConstraint : MonoBehaviour
{
    [SerializeField] Transform targetToPlace;
    [SerializeField] float distanceMultiplier;
    void Update()
    {
        float verticalRotation;
        if(targetToPlace.eulerAngles.x < 180)
        {
            verticalRotation = 70 + targetToPlace.eulerAngles.x;
        }
        else
        {
            verticalRotation = -290 + targetToPlace.eulerAngles.x;
        }
        float invertedDistance = verticalRotation * distanceMultiplier;
        transform.position = targetToPlace.position + targetToPlace.forward * invertedDistance;
    }
}
