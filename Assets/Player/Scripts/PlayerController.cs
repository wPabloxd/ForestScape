using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using DG.Tweening.Core.Easing;

public class PlayerController : MonoBehaviour, IEntityAnimable, IVisible
{
    public enum MovementMode
    {
        RelativeToCharacter,
        RelativeToCamera,
    }

    public enum OrientationMode
    {
        OrientateToCameraForward,
        OrientateToMovementForward,
        OrientateToTarget,
    }

    [Header("Movement Settings")]
    [SerializeField] float planeSpeed = 5f;
    [SerializeField] MovementMode movementMode = MovementMode.RelativeToCamera;
    [SerializeField] float gravity = -9.8f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] bool paused;
    [SerializeField] LayerMask groundMask;
    bool running;

    [Header("Orientation Settings")]
    [SerializeField] float angularSpeed = 360f;
    [SerializeField] Transform orientationTarget;
    [SerializeField] OrientationMode orientationMode = OrientationMode.OrientateToMovementForward;

    [Header("Movement Inputs")]
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference jump;
    [SerializeField] InputActionReference run;

    [Header("Weapon Inputs")]
    [SerializeField] InputActionReference swapWeapon;
    [SerializeField] InputActionReference fire;
    [SerializeField] InputActionReference ADS;
    [SerializeField] InputActionReference meleeAttack;
    [SerializeField] InputActionReference[] selectWeaponInputs;
    Animator animator;
    CharacterController characterController;
    EntityWeapons entityWeapons;

    [Header("Allegiance")]
    [SerializeField] string allegiance = "Player";

    bool alreadyAttacking;
    int weaponBeforeBat;
    bool isMelee;

    Vector3 velocityToApply = Vector3.zero;
    float verticalVelocity = 0f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        entityWeapons = GetComponent<EntityWeapons>();
    }
    private void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();
        meleeAttack.action.Enable();
        fire.action.Enable();
        swapWeapon.action.Enable();
        run.action.Enable();
        ADS.action.Enable();
        foreach (InputActionReference iar in selectWeaponInputs)
        {
            iar.action.Enable();
        }
    }
    void Update()
    {
        //Debug.Log(verticalVelocity);
        paused = GameManager.Instance.paused;
        if (!paused)
        {

            velocityToApply = Vector3.zero;
            UpdateMovementOnPlane();
            UpdateVerticalMovement();

            characterController.Move(velocityToApply * Time.deltaTime);

            UpdateOrientation();
            UpdateWeapons();
            SwapWeapons();
            MeleeAttack();
        }
    }
    private void MeleeAttack()
    {
        if (meleeAttack.action.WasPressedThisFrame())
        {  
            StartCoroutine(MeleeDuration());
       
        }
    }
    IEnumerator MeleeDuration()
    {
        entityWeapons.GetCurrentWeapon().performingMeleeAttack = true;
        isMelee = true;
        yield return new WaitForSeconds(2f/3f);
        isMelee = false;
    }
    public void ResetWeapon()
    {
        alreadyAttacking = false;
        entityWeapons.SetCurrentWeapon(weaponBeforeBat);
    }
    private void UpdateWeapons()
    {
        if (entityWeapons.HasCurrentWeapon())
        {
            switch (entityWeapons.GetCurrentWeapon().weaponKind)
            {
                case Weapon.WeaponKind.pistol:
                    animator.SetInteger("WeaponKind", 0);
                    break;
                case Weapon.WeaponKind.rifle:
                    animator.SetInteger("WeaponKind", 1);
                    break;
                case Weapon.WeaponKind.melee:
                    animator.SetInteger("WeaponKind", 2);
                    break;
            }
            if (!isMelee)
            {
                switch (entityWeapons.GetCurrentWeapon().shotMode)
                {
                    case Weapon.ShotMode.semiauto:
                        if (fire.action.WasPerformedThisFrame())
                        {
                            entityWeapons.Shot();
                        }
                        break;
                    case Weapon.ShotMode.fullauto:
                        if (fire.action.WasPerformedThisFrame())
                        {
                            entityWeapons.StartShooting();
                        }
                        if (fire.action.WasReleasedThisFrame())
                        {
                            entityWeapons.StopShooting();
                        }
                        break;
                    case Weapon.ShotMode.burstmode:
                        if (fire.action.WasPerformedThisFrame())
                        {
                            entityWeapons.BurstShooting();
                        }
                        break;
                }
            }
        }
    }
    void SwapWeapons()
    {
        Vector2 swapWeaponValue = swapWeapon.action.ReadValue<Vector2>();
        if (!alreadyAttacking)
        {
            if (swapWeaponValue.y < 0)
            {
                entityWeapons.SelectNextWeapon();
            }
            else if (swapWeaponValue.y > 0)
            {
                entityWeapons.SelectPreviousWeapon();
            }
            for (int i = 0; i < selectWeaponInputs.Length; i++)
            {
                if (selectWeaponInputs[i].action.WasPressedThisFrame())
                {
                    entityWeapons.SetCurrentWeapon(i);
                }
            }
        }
    }
    Vector3 lastVelocity = Vector3.zero;
    private void UpdateMovementOnPlane()
    {
        if(run.action.IsPressed() && !ADS.action.IsPressed() && !entityWeapons.GetCurrentWeapon().reloading)
        {
            running = true;
            planeSpeed = 5f;
        }
        else
        {
            running = false;
            planeSpeed = 2.5f;
        }
        Vector2 rawMoveValue = move.action.ReadValue<Vector2>();
        Vector3 xzMoveValue = (Vector3.right * rawMoveValue.x) + (Vector3.forward * rawMoveValue.y);

        switch (movementMode)
        {
            case MovementMode.RelativeToCharacter: UpdateMovementRelativeToCharacter(xzMoveValue); break;
            case MovementMode.RelativeToCamera: UpdateMovementRelativeToCamera(xzMoveValue); break;
        }

        void UpdateMovementRelativeToCharacter(Vector3 xzMoveValue)
        {
            Vector3 velocity = xzMoveValue * planeSpeed;
            velocityToApply += velocity;
        }
        void UpdateMovementRelativeToCamera(Vector3 xzMoveValue)
        {
            Transform cameraTransform = Camera.main.transform;
            Vector3 xzMoveValueFromCamera = cameraTransform.TransformDirection(xzMoveValue);
            float originalMagnitude = xzMoveValueFromCamera.magnitude;
            xzMoveValueFromCamera = Vector3.ProjectOnPlane(xzMoveValueFromCamera, Vector3.up).normalized * originalMagnitude;

            Vector3 velocity = xzMoveValueFromCamera * planeSpeed;

            velocityToApply += velocity;
        }
    }
    private void UpdateVerticalMovement()
    {
        //if (characterController.isGrounded)
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), Vector3.down, 0.12f, groundMask))
        {
            verticalVelocity = 0f;
            if(jump.action.WasPerformedThisFrame() )
            {
                verticalVelocity = jumpSpeed;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        velocityToApply += verticalVelocity * Vector3.up;
    }
    private void UpdateOrientation()
    {
        Vector3 desiredDirection = Vector3.zero;
        switch (orientationMode)
        {
            case OrientationMode.OrientateToCameraForward:
                desiredDirection = Camera.main.transform.forward;
                break;
            case OrientationMode.OrientateToMovementForward:
                if (velocityToApply.sqrMagnitude > 0f)
                {
                    desiredDirection = lastVelocity;
                }
                break;
            case OrientationMode.OrientateToTarget:
                desiredDirection = orientationTarget.transform.position - transform.position;
                desiredDirection.y = 0;
                break;
        }

        float angularDistance = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);
        float angleToApply = angularSpeed * Time.deltaTime;

        angleToApply = Mathf.Min(angleToApply, Mathf.Abs(angularDistance));
        angleToApply *= Mathf.Sign(angularDistance);

        Quaternion rotationToApply = Quaternion.AngleAxis(angleToApply, Vector3.up);
        transform.rotation = rotationToApply * transform.rotation;
    }
    private void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
        meleeAttack.action.Disable();
        fire.action.Disable();
        swapWeapon.action.Disable();
        run.action.Disable();
        ADS.action.Disable();
        foreach (InputActionReference iar in selectWeaponInputs)
        {
            iar.action.Disable();
        }
    }
    #region IEntityAnimable implementation
    public Vector3 GetLastVelocity()
    {
        return velocityToApply;
    }
    public float GetVerticalVelocity()
    {
        return verticalVelocity;
    }
    public float GetJumpSpeed()
    {
        return jumpSpeed;
    }
    public bool IsGrounded()
    {
        return Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), Vector3.down, 0.12f, groundMask);
    }
    #endregion
    #region
    public Transform GetTransform()
    {
        return transform;
    }
    public string GetAllegiance()
    {
        return allegiance;
    }
    public bool IsRunning()
    {
        return running;
    }
    public bool IsAiming()
    {
        return ADS.action.IsPressed();
    }
    #endregion
}