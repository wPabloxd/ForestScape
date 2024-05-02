using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SGAnimation : WeaponAnimations
{
    Animator animator;
    BarrelByRaycast barrel;
    BarrelByRaycastEnemy barrelEnemy;
    AudioSource audioSource;
    [SerializeField] GameObject HUDImage;
    [SerializeField] AudioClip[] reloadAudioClips;
    bool reloadCancelled;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (isPlayer)
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
        if (!reloading)
        {
            reloadCancelled = false;
        }
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
        if (isPlayer)
        {
            barrel.weapon.reloading = true;
        }
        else
        {
            barrelEnemy.weapon.reloading = true;
        }
        audioSource.clip = reloadAudioClips[0];
        audioSource.Play();
        animator.SetBool("Reload", true);
    }
    public void Reloaded()
    {
        if (isPlayer)
        {
            barrel.ReloadOneBullet();
            if (barrel.Full() || reloadCancelled)
            {
                StartCoroutine(ReloadDelayShot());
                animator.SetBool("Reload", false);
                barrel.weapon.reloading = false;
                reloadCancelled = false;
            }
            else
            {
                Reloading(true);
            }
        }
        else
        {
            barrelEnemy.ReloadOneBullet();
            if (barrelEnemy.Full() || reloadCancelled)
            {
                StartCoroutine(ReloadDelayShot());
                animator.SetBool("Reload", false);
                barrelEnemy.weapon.reloading = false;
                reloadCancelled = false;
            }
            else
            {
                Reloading(true);
            }
        }
    }
    IEnumerator ReloadDelayShot()
    {
        yield return new WaitForSeconds(0.5f);
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
        if(isPlayer)
        {
            HUDImage.SetActive(false);
        }
        if (reloading)
        {
            CancelReload();
        }
    }
    public override void EndReload()
    {
        if(reloading)
        {
            reloadCancelled = true;
        }
    }
    public override void CancelReload()
    {
        audioSource.Stop();
        reloading = false;
        if (isPlayer)
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