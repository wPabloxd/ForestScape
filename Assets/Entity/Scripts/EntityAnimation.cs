using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimation : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] float transitionVelocity = 10f;
    [SerializeField] bool isEnemy;
    public bool running;
    Animator animator;
    IEntityAnimable entityAnimable;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        entityAnimable = GetComponent<IEntityAnimable>();
    }
    void Update()
    {
        UpdateAnimation(entityAnimable.GetLastVelocity(), entityAnimable.GetVerticalVelocity(), entityAnimable.GetJumpSpeed(), entityAnimable.IsGrounded(), entityAnimable.IsRunning());
    }
    Vector3 smoothedAnimationVelocity = Vector3.zero;
    private void UpdateAnimation(Vector3 lastVelocity, float verticalVelocity, float jumpSpeed, bool isGrounded, bool isRunning)
    {
        running = isRunning;
        Vector3 velocityDistance = lastVelocity.normalized - smoothedAnimationVelocity;
        float transitionVelocityToApply = transitionVelocity * Time.deltaTime;

        transitionVelocityToApply = Mathf.Min(transitionVelocityToApply, velocityDistance.magnitude);
        smoothedAnimationVelocity += velocityDistance.normalized * transitionVelocityToApply;

        Vector3 localSmoothedAnimationVelocity = transform.InverseTransformDirection(smoothedAnimationVelocity);
        if(!isEnemy)
        {
            if (isRunning)
            {
                animator.SetFloat("Speed", 2);
            }
            else
            {
                animator.SetFloat("Speed", 1.25f);
            }
        }

        animator.SetFloat("sideMovement", localSmoothedAnimationVelocity.x);
        animator.SetFloat("forwardMovement", localSmoothedAnimationVelocity.z);

        float clampledVerticalVelocity = Mathf.Clamp(verticalVelocity, -jumpSpeed, jumpSpeed);
        float normalizedVerticalVelocity = Mathf.InverseLerp(jumpSpeed, -jumpSpeed, clampledVerticalVelocity);

        animator.SetFloat("normalizedVerticalVelocity", normalizedVerticalVelocity);
        animator.SetBool("IsGrounded", isGrounded);
    }
}