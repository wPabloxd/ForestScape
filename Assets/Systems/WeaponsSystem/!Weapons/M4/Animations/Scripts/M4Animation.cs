using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M4Animation : WeaponAnimations
{
    Animator animator;
    BarrelByRaycast barrel;
    BarrelByRaycastEnemy barrelEnemy;
    AudioSource audioSource;
    [SerializeField] AudioClip[] reloadAudioClips;
    [SerializeField] GameObject magazine;
    [SerializeField] GameObject HUDImage;
    [SerializeField] GameObject magazineEmpty;
    [SerializeField] Transform magazineSpot;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if(isPlayer )
        {
            barrel = GetComponentInChildren<BarrelByRaycast>();
        }
        else
        {
            barrelEnemy = GetComponentInChildren<BarrelByRaycastEnemy>();
        }
        entityAnimable = GetComponentInParent<IEntityAnimable>();
        animator = GetComponent<Animator>();
        animator.keepAnimatorStateOnDisable = true;
    }
    private void OnEnable()
    {
        if (isPlayer)
        {
            HUDImage.SetActive(true);
        }
        animator.Play("Unholster", 0, 0.25f);
        animator.ResetTrigger("Holster");
    }
    void Update()
    {
        Moving(entityAnimable.GetLastVelocity(), entityAnimable.IsRunning(), entityAnimable.IsAiming());
    }
    public override void Moving(Vector3 lastVelocity, bool isRunning, bool isAiming)
    {
        if (isPlayer)
        {
            barrel.aiming = isAiming;
        }
        if (Mathf.Abs(lastVelocity.x) > 0.5f || Mathf.Abs(lastVelocity.z) > 0.5f)
        {
            if (isRunning)
            {
                animator.SetInteger("MovementState", 2);
            }
            else
            {
                animator.SetInteger("MovementState", 1);
            }
        }
        else
        {
            animator.SetInteger("MovementState", 0);
        }
        if (isAiming)
        {
            animator.SetBool("Aiming", true);
        }
        else
        {
            animator.SetBool("Aiming", false);
        }
    }
    public override void Shooting()
    {
        animator.SetTrigger("Shoot");
        StartCoroutine(ShootingAnimationTrigger());
    }
    IEnumerator ShootingAnimationTrigger()
    {
        yield return new WaitForEndOfFrame();
        animator.ResetTrigger("Shoot");
    }
    public override void Reloading(bool empty)
    {
        reloading = true;
        if(isPlayer)
        {
            barrel.weapon.reloading = true;
        }
        else
        {
            barrelEnemy.weapon.reloading = true;
        }
        if (empty)
        {
            Instantiate(magazineEmpty, magazineSpot.position, magazineSpot.rotation);
            audioSource.clip = reloadAudioClips[0];
            audioSource.Play();
        }
        else
        {
            Instantiate(magazine, magazineSpot.position, magazineSpot.rotation);
            audioSource.clip = reloadAudioClips[1];
            audioSource.Play();
        }
        animator.SetBool("Reload", true);
    }
    public void Reloaded()
    {
        animator.SetBool("Reload", false);
        StartCoroutine(ReloadDelayShot());
        if(isPlayer )
        {
            barrel.weapon.reloading = false;
            barrel.Reload();
        }
        else
        {
            barrelEnemy.weapon.reloading = false;
            barrelEnemy.Reload();
        }
    }
    IEnumerator ReloadDelayShot()
    {
        yield return new WaitForSeconds(0.3f);
        reloading = false;
    }
    public override void EmptyShot()
    {
        if (!reloading)
        {
            audioSource.clip = reloadAudioClips[2];
            audioSource.Play();
        }
    }
    public override void Holstering()
    {
        animator.SetTrigger("Holster");
    }
    public override void MeleeAttack()
    {
        animator.SetTrigger("Melee");
        CancelReload();
        StartCoroutine(MeleeTrigger());
    }
    IEnumerator MeleeTrigger()
    {
        yield return new WaitForEndOfFrame();
        animator.ResetTrigger("Melee");
    }
    private void OnDisable()
    {
        if (isPlayer)
        {
            HUDImage.SetActive(false);
        }
        if (reloading)
        {
            CancelReload();
        }
    }
    public override void CancelReload()
    {
        audioSource.Stop();
        reloading = false;
        if(isPlayer)
        {
            barrel.weapon.reloading = false;
        }
        else
        {
            barrelEnemy.weapon.reloading = false;
        }
        animator.SetBool("Reload", false);
    }
}