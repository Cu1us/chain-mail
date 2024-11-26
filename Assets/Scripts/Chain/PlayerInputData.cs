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
    [ReadOnlyInspector][Obsolete] public float chainRotationalInput;
    [ReadOnlyInspector] public bool isHoldingAttack;
    [ReadOnlyInspector] public bool isGrabbingChain;

    public Action onAttackPress;
    public Action onAttackRelease;
    public Action onChainSwap;
    public Action onChainGrab;


    [SerializeField] float swapPlacesButtonWindow;

    float lastSwap1Press = float.NegativeInfinity;
    float lastSwap2Press = float.NegativeInfinity;


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
    void OnGrabChain(InputValue value)
    {
        isGrabbingChain = Mathf.RoundToInt(value.Get<float>()) != 0;
        onChainGrab?.Invoke();
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

    [Obsolete]
    void OnChainRotation(InputValue value)
    {
        chainRotationalInput = value.Get<float>();
        Debug.Log("Rotating: value: " + chainRotationalInput);
        if (DebugRays) Debug.DrawRay(transform.position, movementInput, Color.gray, 1f);
    }
    void OnAttack(InputValue value)
    {
        isHoldingAttack = Mathf.RoundToInt(value.Get<float>()) != 0;
        if (isHoldingAttack)
        {
            onAttackPress?.Invoke();
        }
        else
        {
            onAttackRelease?.Invoke();
        }
    }

    void OnChainSwap1()
    {
        lastSwap1Press = Time.time;
        if (Time.time - lastSwap2Press < swapPlacesButtonWindow)
        {
            SwapPlaces();
        }
    }
    void OnChainSwap2()
    {
        lastSwap2Press = Time.time;
        if (Time.time - lastSwap1Press < swapPlacesButtonWindow)
        {
            SwapPlaces();
        }
    }
    void SwapPlaces()
    {
        lastSwap1Press = float.NegativeInfinity;
        lastSwap2Press = float.NegativeInfinity;
        onChainSwap?.Invoke();
    }
}
