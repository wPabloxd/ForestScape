using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityAnimable
{
    public Vector3 GetLastVelocity();
    public float GetVerticalVelocity();
    public float GetJumpSpeed();
    public bool IsGrounded();
    public bool IsRunning();
    public bool IsAiming();
}
