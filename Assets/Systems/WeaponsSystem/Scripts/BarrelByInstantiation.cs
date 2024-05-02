using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelByInstantiation : Barrel
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootPoint;

    public override void Shot()
    {
        Instantiate(projectilePrefab, shootPoint.position,shootPoint.rotation);
    }
}
