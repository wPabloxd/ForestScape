using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class BarrelByRaycast : Barrel
{
    [Header("Weapon Parts")]
    [SerializeField] Transform shootPoint;
    [SerializeField] Transform shellEjectPoint;
    [SerializeField] GameObject shell;
    [SerializeField] GameObject tracerPrefab;
    [SerializeField] Audible shotAudible;
    VisualEffect muzzleFlash;

    [Header("Weapon Stats")]
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 1;
    [SerializeField] float cadence = 15f;
    [SerializeField] Vector2 dispersionAngles = new Vector2(5, 5);
    [SerializeField] Vector2 dispersionAnglesADSed = new Vector2(0.5f, 0.5f);
    [SerializeField] bool doesWallbang;
    [SerializeField] int burstDuration;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] bool isShotgun;
    [SerializeField] AudioSource hitmarkerSource;
    [SerializeField] int numberOfPellets;
    public bool aiming;
    int currentBurst;
    public Weapon weapon;

    [Header("Other")]
    [SerializeField] LayerMask layerMask = Physics.DefaultRaycastLayers;
    WeaponAnimations weaponAnimations;
    [SerializeField] InputActionReference reloadInput;
    [SerializeField] GameObject crosshair;

    bool isContinuousShooting;
    bool isBurstMode;
    bool shootCoolDown;
    bool drawing;
    float nextShotTime;
    private void OnEnable()
    {
        UpdateAmmoUI();
        reloadInput.action.Enable();
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
        if (aiming)
        {
            crosshair.SetActive(false);
        }
        else
        {
            crosshair.SetActive(true);
        }
        if(weapon.swapping)
        {
            weaponAnimations.Holstering();
        }
        else if(weapon.performingMeleeAttack)
        {
            StopShooting();
            weaponAnimations.MeleeAttack();
            weapon.performingMeleeAttack = false;
        }
        if (reloadInput.action.WasPressedThisFrame() && weapon.reserveAmmo > 0 && weapon.magAmmo != weapon.magSize && !weaponAnimations.reloading && !weapon.swapping)
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
        if(GameManager.Instance.paused)
        {
            StopShooting();
        }
    }
    public void UpdateAmmoUI()
    {
        switch (weapon.weaponNumber)
        {
            case 0:
                weapon.reserveAmmo += GameManager.Instance.bufferAmmoUSP;
                GameManager.Instance.bufferAmmoUSP = 0;
                break;
            case 1:
                weapon.reserveAmmo += GameManager.Instance.bufferAmmoM4;
                GameManager.Instance.bufferAmmoM4 = 0;
                break;
            case 2:
                weapon.reserveAmmo += GameManager.Instance.bufferAmmoSG;
                GameManager.Instance.bufferAmmoSG = 0;
                break;
        }
        ammoText.text = weapon.magAmmo + "/" + weapon.reserveAmmo;
    }
    public override void Shot()
    {
        if (!shootCoolDown && !drawing && !weaponAnimations.reloading && !weapon.swapping)
        {
            if(weapon.magAmmo == 0)
            {
                weaponAnimations.EmptyShot();
                nextShotTime = Time.time;
                nextShotTime += 1f / cadence;
                shootCoolDown = true;
                return;
            }
            weaponAnimations.Shooting();
            shotAudible.enabled = true;
            weapon.magAmmo--;
            UpdateAmmoUI();
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
            if(isShotgun)
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
                                hit.collider.GetComponentInParent<Hurtbox>()?.NotifyHit(this, damage);
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
                            hit.collider.GetComponentInParent<Hurtbox>()?.NotifyHit(this, damage);
                            if(hit.transform.gameObject.tag == "Enemy")
                            {
                                hitmarkerSource.Play();
                            }
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
                            hit.collider.GetComponentInParent<Hurtbox>()?.NotifyHit(this, damage);
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
                        hit.collider.GetComponentInParent<Hurtbox>()?.NotifyHit(this, damage);
                        if (hit.transform.gameObject.tag == "Enemy")
                        {
                            hitmarkerSource.Play();
                        }
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
        if (weapon.reserveAmmo >= weapon.magSize)
        {
            if (weapon.magAmmo == 0)
            {
                weapon.reserveAmmo -= weapon.magSize - 1;
                weapon.magAmmo = weapon.magSize - 1;
            }
            else
            {
                weapon.reserveAmmo -= weapon.magSize - weapon.magAmmo;
                weapon.magAmmo = weapon.magSize;
            }
        }
        else
        {
            if (weapon.magAmmo + weapon.reserveAmmo < weapon.magSize)
            {
                weapon.magAmmo += weapon.reserveAmmo;
                weapon.reserveAmmo = 0;
            }
            else
            {
                if(weapon.magAmmo == 0)
                {
                    weapon.reserveAmmo -= weapon.magSize - weapon.magAmmo - 1;
                    weapon.magAmmo = weapon.magSize - 1;
                }
                else
                {
                    weapon.reserveAmmo -= weapon.magSize - weapon.magAmmo;
                    weapon.magAmmo = weapon.magSize;
                }
            }
        }
        UpdateAmmoUI();
    }
    public void ReloadOneBullet()
    {
        if(weapon.magAmmo != weapon.magSize && weapon.reserveAmmo > 0)
        {
            weapon.magAmmo++;
            weapon.reserveAmmo--;
        }
        UpdateAmmoUI();
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
    private void OnDisable()
    {
        reloadInput.action.Disable();
    }
}