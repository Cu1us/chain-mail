using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputData : MonoBehaviour
{
    [ReadOnlyInspector] public Vector2 movementInput;
    [ReadOnlyInspector] public Vector2 aimDirection;
    [ReadOnlyInspector] public float chainRotationalInput;
    public Action onAttackPress;
    public Action onAttackRelease;
    public Action onChainSwap;



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
        Debug.Log("Rotating: value: " + chainRotationalInput);
        if (DebugRays) Debug.DrawRay(transform.position, movementInput, Color.gray, 1f);
    }
    void OnAttack(InputValue value)
    {
        bool held = Mathf.RoundToInt(value.Get<float>()) != 0;
        if (held)
        {
            onAttackPress?.Invoke();
        }
        else
        {
            onAttackRelease?.Invoke();
        }
    }
}
