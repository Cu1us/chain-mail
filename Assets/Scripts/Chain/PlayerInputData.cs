using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputData : MonoBehaviour
{
    public enum InputType
    {
        Keyboard,
        Gamepad
    }
    public bool inputDisabled { get; private set; } = false;
    public static InputType inputType { get { return Gamepad.current != null ? InputType.Gamepad : InputType.Keyboard; } }

    [Obsolete] public Vector2 aimDirection => Vector2.zero;
    [Obsolete] public bool isHoldingAttack { get => !inputDisabled && _isHoldingAttack; }
    [Obsolete] bool _isHoldingAttack;
    [Obsolete] public Action onAttackPress;
    [Obsolete] public Action onAttackRelease;

    public Vector2 movementInput { get => inputDisabled ? default : _movementInput; }
    public float chainRotationalInput { get => inputDisabled ? default : _chainRotationalInput; }
    public float chainExtendInput { get => inputDisabled ? default : _chainExtendInput; }

    Vector2 _movementInput;
    float _chainRotationalInput;
    float _chainExtendInput;

    public Action<int> onChainRotate;
    public Action onChainSwap;
    public Action onSwitchAnchor;


    void OnMovement(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }
    void OnRotateChain(InputValue value)
    {
        _chainRotationalInput = Mathf.Round(value.Get<float>());
        if (!inputDisabled) onChainRotate?.Invoke(Mathf.RoundToInt(value.Get<float>()));
    }
    void OnExtendChain(InputValue value)
    {
        _chainExtendInput = Mathf.Round(value.Get<float>());
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
