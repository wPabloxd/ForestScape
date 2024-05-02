using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class BarrelByRaycastEnemy : Barrel
{
    [Header("Weapon Parts")]
    [SerializeField] Transform shootPoint;
    [SerializeField] Transform shellEjectPoint;
    [SerializeField] GameObject shell;
    [SerializeField] GameObject tracerPrefab;
    VisualEffect muzzleFlash;

    [Header("Weapon Stats")]
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 1;
    [SerializeField] float cadence = 15f;
    [SerializeField] Vector2 dispersionAngles = new Vector2(5, 5);
    [SerializeField] Vector2 dispersionAnglesADSed = new Vector2(0.5f, 0.5f);
    [SerializeField] bool doesWallbang;
    [SerializeField] int burstDuration;
    [SerializeField] bool isShotgun;
    [SerializeField] int numberOfPellets;
    public bool aiming;
    int currentBurst;
    public Weapon weapon;

    [Header("Other")]
    [SerializeField] LayerMask layerMask = Physics.DefaultRaycastLayers;
    WeaponAnimations weaponAnimations;

    bool isContinuousShooting;
    bool isBurstMode;
    bool shootCoolDown;
    bool drawing;
    float nextShotTime;
    private void OnEnable()
    {
        currentBurst = burstDuration;
        StopShooting();
        StartCoroutine(DrawTime());
        if (muzzleFlash)
        {
            foreach (Transform child in muzzleFlash.transform)
            {
                child.gameObject.SetActive(false);
            }
            muzzleFlash.Stop();
        }
    }
    private void Awake()
    {
        weaponAnimations = GetComponentInParent<WeaponAnimations>();
        weapon = GetComponentInParent<Weapon>();
        muzzleFlash = GetComponentInChildren<VisualEffect>();
    }
    private void Update()
    {
        if (weapon.swapping)
        {
            weaponAnimations.Holstering();
        }
        else if (weapon.performingMeleeAttack)
        {
            StopShooting();
            weaponAnimations.MeleeAttack();
            weapon.performingMeleeAttack = false;
        }
        if (weapon.magAmmo <= 0 && !weaponAnimations.reloading)
        {
            weaponAnimations.Reloading(weapon.magAmmo == 0);
        }
        if (Time.time > nextShotTime)
        {
            shootCoolDown = false;
            if (isContinuousShooting)
            {
                Shot();
            }
            else if (isBurstMode)
            {
                Shot();
                currentBurst--;
                if (currentBurst == 0)
                {
                    currentBurst = burstDuration;
                    isBurstMode = false;
                }
            }
        }
        if (weapon.magAmmo == 0)
        {
            weaponAnimations.Empty();
        }
        if (GameManager.Instance.paused)
        {
            StopShooting();
        }
    }
    public override void Shot()
    {
        if (!shootCoolDown && !drawing && !weaponAnimations.reloading && !weapon.swapping)
        {
            if (weapon.magAmmo == 0)
            {
                weaponAnimations.EmptyShot();
                nextShotTime = Time.time;
                nextShotTime += 1f / cadence;
                shootCoolDown = true;
                return;
            }
            weaponAnimations.Shooting();
            weapon.magAmmo--;
            nextShotTime = Time.time;
            nextShotTime += 1f / cadence;
            shootCoolDown = true;
            if (muzzleFlash)
            {
                foreach (Transform child in muzzleFlash.transform)
                {
                    child.gameObject.SetActive(true);
                }
                muzzleFlash.Play();
            }
            if (isShotgun)
            {
                for (global::System.Int32 i = 0; i < numberOfPellets; i++)
                {
                    Vector3 dispersedForward = DispersedForward();
                    Vector3 finalPosition = transform.position + dispersedForward.normalized * range;
                    GameObject tracerGO = Instantiate(tracerPrefab);
                    if (doesWallbang)
                    {
                        RaycastHit[] hits = Physics.RaycastAll(shootPoint.position, dispersedForward, range, layerMask);
                        if (hits.Length > 0)
                        {
                            for (int j = 0; j < hits.Length; j++)
                            {
                                RaycastHit hit = hits[i];
                                hit.collider.GetComponent<Hurtbox>()?.NotifyHit(this, damage);
                                if (i == hits.Length - 1)
                                {
                                    finalPosition = hit.point;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Physics.Raycast(shootPoint.position, dispersedForward, out RaycastHit hit, range, layerMask))
                        {
                            finalPosition = hit.point;
                            hit.collider.GetComponent<Hurtbox>()?.NotifyHit(this, damage);
                        }
                    }
                    Tracer tracer = tracerGO.GetComponent<Tracer>();
                    tracer.Init(shootPoint.position, finalPosition);
                }
            }
            else
            {
                Vector3 dispersedForward = DispersedForward();
                Vector3 finalPosition = transform.position + dispersedForward.normalized * range;
                GameObject tracerGO = Instantiate(tracerPrefab);
                if (doesWallbang)
                {
                    RaycastHit[] hits = Physics.RaycastAll(shootPoint.position, dispersedForward, range, layerMask);
                    if (hits.Length > 0)
                    {
                        for (int i = 0; i < hits.Length; i++)
                        {
                            RaycastHit hit = hits[i];
                            hit.collider.GetComponent<Hurtbox>()?.NotifyHit(this, damage);
                            if (i == hits.Length - 1)
                            {
                                finalPosition = hit.point;
                            }
                        }
                    }
                }
                else
                {
                    if (Physics.Raycast(shootPoint.position, dispersedForward, out RaycastHit hit, range, layerMask))
                    {
                        finalPosition = hit.point;
                        hit.collider.GetComponent<Hurtbox>()?.NotifyHit(this, damage);
                    }
                }
                Tracer tracer = tracerGO.GetComponent<Tracer>();
                tracer.Init(shootPoint.position, finalPosition);
            }
            if (shell)
            {
                Instantiate(shell, shellEjectPoint.position, shellEjectPoint.rotation);
            }
        }
        if (isShotgun)
        {
            weaponAnimations.EndReload();
        }
    }
    public override void Reload()
    {
        weapon.magAmmo = weapon.magSize - 1;
    }
    public void ReloadOneBullet()
    {
        if (weapon.magAmmo != weapon.magSize)
        {
            weapon.magAmmo++;
        }
    }
    public bool Full()
    {
        if (weapon.magAmmo != weapon.magSize && weapon.reserveAmmo > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private Vector3 DispersedForward()
    {
        float horizontalDispersionAngle;
        float verticalDispersionAngle;
        if (aiming)
        {
            horizontalDispersionAngle = UnityEngine.Random.Range(-dispersionAnglesADSed.x, dispersionAnglesADSed.x);
            verticalDispersionAngle = UnityEngine.Random.Range(-dispersionAnglesADSed.y, dispersionAnglesADSed.y);
        }
        else
        {
            horizontalDispersionAngle = UnityEngine.Random.Range(-dispersionAngles.x, dispersionAngles.x);
            verticalDispersionAngle = UnityEngine.Random.Range(-dispersionAngles.y, dispersionAngles.y);
        }
        Quaternion horizontalRotation = Quaternion.AngleAxis(horizontalDispersionAngle, transform.up);
        Quaternion verticalRotation = Quaternion.AngleAxis(verticalDispersionAngle, transform.right);
        return horizontalRotation * verticalRotation * transform.forward;
    }
    public override void StartShooting()
    {
        isContinuousShooting = true;
    }
    public override void StopShooting()
    {
        isContinuousShooting = false;
    }
    public override void BurstShooting()
    {
        isBurstMode = true;
    }
    IEnumerator DrawTime()
    {
        drawing = true;
        yield return new WaitForSeconds(0.4f);
        drawing = false;
    }
}