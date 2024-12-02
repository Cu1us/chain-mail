using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputData : MonoBehaviour
{
    public bool inputDisabled { get; private set; } = false;

    public Vector2 movementInput { get => inputDisabled ? default : _movementInput; }
    public Vector2 aimDirection { get => inputDisabled ? default : _aimDirection; }
    public float chainRotationalInput { get => inputDisabled ? default : _chainRotationalInput; }
    public float chainExtendInput { get => inputDisabled ? default : _chainExtendInput; }
    public bool isHoldingAttack { get => inputDisabled ? default : _isHoldingAttack; }

    Vector2 _movementInput;
    Vector2 _aimDirection;
    float _chainRotationalInput;
    float _chainExtendInput;
    bool _isHoldingAttack;

    public Action onAttackPress;
    public Action onAttackRelease;
    public Action<int> onChainRotate;
    public Action onChainSwap;

    [SerializeField] float swapPlacesButtonWindow;

    float lastSwap1Press = float.NegativeInfinity;
    float lastSwap2Press = float.NegativeInfinity;
    float swapPlaceTimer;
    float chainRotateTimer;
    float chainHeldTimer;
    [SerializeField] float chainHeldMaxTime;
    [SerializeField] float chainRotateCooldown;


    [SerializeField] bool DebugRays;

    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        swapPlaceTimer += Time.deltaTime;
        chainRotateTimer += Time.deltaTime;
        if(_chainRotationalInput != 0)
        {
            chainHeldTimer += Time.deltaTime;
        }
        if(chainHeldTimer > chainHeldMaxTime)
        {
            _chainRotationalInput = 0;
        }
        if(_chainRotationalInput == 0)
        {
            chainHeldTimer = 0;
        }
    }

    void OnMovement(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
        if (DebugRays) Debug.DrawRay(transform.position, movementInput, Color.green, 1f);
    }
    void OnAim(InputValue value)
    {
        _aimDirection = value.Get<Vector2>();
        if (DebugRays) Debug.DrawRay(transform.position, aimDirection, Color.cyan, 1f);
    }
    void OnAimScreenCoords(InputValue value)
    {
        Vector2 aimTarget = value.Get<Vector2>();
        aimTarget = mainCamera.ScreenToWorldPoint(aimTarget);
        _aimDirection = (aimTarget - (Vector2)transform.position).normalized;
        if (DebugRays) Debug.DrawRay(transform.position, aimDirection, Color.cyan, 1f);
    }
    void OnChainRotation(InputValue value)
    {
        if (chainRotateTimer < chainRotateCooldown)
        {
            return;
        }

        chainRotateTimer = 0;

        _chainRotationalInput = value.Get<float>();
        if (DebugRays) Debug.DrawRay(transform.position, movementInput, Color.gray, 1f);
        if (!inputDisabled) onChainRotate?.Invoke(Mathf.RoundToInt(chainRotationalInput));
    }
    void OnExtendChain(InputValue value)
    {
        _chainExtendInput = value.Get<float>();
    }
    void OnAttack(InputValue value)
    {
        _isHoldingAttack = Mathf.RoundToInt(value.Get<float>()) != 0;
        if (isHoldingAttack)
        {
            if (!inputDisabled) onAttackPress?.Invoke();
        }
        else
        {
            if (!inputDisabled) onAttackRelease?.Invoke();
        }
    }

    void OnChainSwap1()
    {
        if (swapPlaceTimer < 1)
        {
            return;
        }
        lastSwap1Press = Time.time;
        if (Time.time - lastSwap2Press < swapPlacesButtonWindow)
        {
            swapPlaceTimer = 0;
            OnSwapPlaces();
        }
    }
    void OnChainSwap2()
    {
        if (swapPlaceTimer < 1)
        {
            return;
        }
        lastSwap2Press = Time.time;
        if (Time.time - lastSwap1Press < swapPlacesButtonWindow)
        {
            swapPlaceTimer = 0;
            OnSwapPlaces();
        }
    }
    void OnSwapPlaces()
    {
        lastSwap1Press = float.NegativeInfinity;
        lastSwap2Press = float.NegativeInfinity;
        if (!inputDisabled) onChainSwap?.Invoke();
    }


    public void DisableInput()
    {
        inputDisabled = true;
    }
    public void EnableInput()
    {
        inputDisabled = false;
    }
}
