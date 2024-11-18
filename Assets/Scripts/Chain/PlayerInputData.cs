using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputData : MonoBehaviour
{
    public Vector2 movementInput;
    public Vector2 aimDirection;
    public float chainRotationalInput;
    public Action onAttack;

    [SerializeField] bool DebugRays;

    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMovement(InputValue value)
    {
        movementInput = value.Get<Vector2>();
        if (DebugRays) Debug.DrawRay(transform.position, movementInput, Color.green, 1f);
    }
    void OnAim(InputValue value)
    {
        aimDirection = value.Get<Vector2>();
        if (DebugRays) Debug.DrawRay(transform.position, aimDirection, Color.cyan, 1f);
    }
    void OnAimScreenCoords(InputValue value)
    {
        Vector2 aimTarget = value.Get<Vector2>();
        aimTarget = mainCamera.ScreenToWorldPoint(aimTarget);
        aimDirection = (aimTarget - (Vector2)transform.position).normalized;
        if (DebugRays) Debug.DrawRay(transform.position, aimDirection, Color.cyan, 1f);
    }
    void OnChainRotation(InputValue value)
    {
        chainRotationalInput = value.Get<float>();
        if (DebugRays) Debug.DrawRay(transform.position, movementInput, Color.gray, 1f);
    }
    void OnAttack(InputValue value)
    {
        onAttack?.Invoke();
    }
}
