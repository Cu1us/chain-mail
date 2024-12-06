using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputData : MonoBehaviour
{
    public bool inputDisabled { get; private set; } = false;

    [Obsolete] public Vector2 aimDirection => Vector2.zero;
    [Obsolete] public Action<int> onChainRotate;
    [Obsolete] public bool isHoldingAttack { get => !inputDisabled && _isHoldingAttack; }
    [Obsolete] bool _isHoldingAttack;

    public Vector2 movementInput { get => inputDisabled ? default : _movementInput; }
    public float chainRotationalInput { get => inputDisabled ? default : _chainRotationalInput; }
    public float chainExtendInput { get => inputDisabled ? default : _chainExtendInput; }

    Vector2 _movementInput;
    float _chainRotationalInput;
    float _chainExtendInput;

    public Action onAttackPress;
    public Action onAttackRelease;
    public Action onChainSwap;
    public Action onSwitchAnchor;


    void OnMovement(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }
    void OnRotateChain(InputValue value)
    {
        _chainRotationalInput = value.Get<float>();
    }
    void OnExtendChain(InputValue value)
    {
        _chainExtendInput = value.Get<float>();
    }
    void OnSwitchAnchor()
    {
        if (!inputDisabled) onSwitchAnchor?.Invoke();
    }
    void OnSwapPlaces()
    {
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
