using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] InputActionReference aim;
    [SerializeField] Transform fpsCameraPivot;
    [SerializeField] float mouseSpeedX = 360f;
    [SerializeField] float mouseSpeedY = 360f;
    [SerializeField] float maxVerticalAngle = 90;
    bool isDead;
    private void Awake()
    {
        LoadSensitivity();
    }
    private void OnEnable()
    {
        aim.action.Enable();
    }
    private void Update()
    {
        if (!isDead)
        {
            if (GameManager.Instance.sensitivityChanged)
            {
                GameManager.Instance.sensitivityChanged = false;
                LoadSensitivity();
            }
            Vector2 aimDelta = aim.action.ReadValue<Vector2>();
            transform.Rotate(Vector3.up, aimDelta.x * mouseSpeedX * Time.deltaTime);

            float currentAngle = Vector3.SignedAngle(transform.forward, fpsCameraPivot.forward, transform.right);
            float rotationToApply = aimDelta.y * mouseSpeedY * Time.deltaTime;
            if (currentAngle + rotationToApply > maxVerticalAngle)
            {
                fpsCameraPivot.Rotate(Vector3.right, maxVerticalAngle - currentAngle);
            }
            else if (currentAngle + rotationToApply < -maxVerticalAngle)
            {
                fpsCameraPivot.Rotate(Vector3.right, -(maxVerticalAngle + currentAngle));
            }
            else
            {
                fpsCameraPivot.Rotate(Vector3.right, rotationToApply);
            }
        }
    }
    public void Death()
    {
        isDead = true;
    }
    public void LoadSensitivity()
    {
        mouseSpeedX = GameManager.Instance.sensitivity;
        mouseSpeedY = -GameManager.Instance.sensitivity;
    }
    private void OnDisable()
    {
        aim.action.Disable();
    }
}