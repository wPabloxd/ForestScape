using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Barrel[] barrels;
    public int weaponNumber;
    public int magAmmo;
    public int magSize;
    public int reserveAmmo;
    public bool swapping;
    public bool performingMeleeAttack;
    public enum WeaponKind
    {
        pistol,
        rifle,
        melee,
    }
    public enum ShotMode
    {
        semiauto,
        fullauto,
        burstmode,
    }
    public ShotMode shotMode;
    public WeaponKind weaponKind;
    public bool reloading;
    private void Awake()
    {
        barrels = GetComponentsInChildren<Barrel>();
    }
    public void Shot()
    {
        foreach (Barrel b in barrels)
        {
            b.Shot();
        }
    }
    public void StartShooting()
    {
        foreach (Barrel b in barrels)
        {
            b.StartShooting();
        }
    }
    public void StopShooting()
    {
        foreach (Barrel b in barrels)
        {
            b.StopShooting();
        }
    }
    public void BurstShooting()
    {
        foreach (Barrel b in barrels)
        {
            b.BurstShooting();
        }
    }
}