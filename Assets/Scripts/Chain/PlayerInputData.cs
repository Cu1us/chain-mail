using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputData : MonoBehaviour
{
    public enum InputType
    {
        Keyboard,
        PS4,
        Xbox
    }

    static List<(float, float, float)> RumbleData = new();
    static bool rumbling = false;
    public static void Rumble(float duration, float FreqA, float FreqB)
    {
        if (Gamepad.current == null) return;
        RumbleData.Add((Time.time + duration, FreqA, FreqB));
    }
    public static void StopRumble()
    {
        if (Gamepad.current == null) return;
        rumbling = false;
        RumbleData.Clear();
        Gamepad.current.SetMotorSpeeds(0, 0);
    }

    void Update()
    {
        if (Gamepad.current != null)
        {
            if (rumbling && RumbleData.Count == 0)
            {
                rumbling = false;
                Gamepad.current.SetMotorSpeeds(0, 0);
            }
            else if (RumbleData.Count > 0)
            {
                RumbleData.RemoveAll((a) => Time.time > a.Item1);
                if (RumbleData.Count > 0)
                {
                    //Debug.Log("Rumble start: " + RumbleData[0].Item2 + " to " + RumbleData[0].Item3);
                    Gamepad.current.SetMotorSpeeds(RumbleData[0].Item2, RumbleData[0].Item3);
                    rumbling = true;
                }
            }
        }
    }

    public bool inputDisabled { get; private set; } = false;
    public static InputType inputType { get; private set; } = InputType.Keyboard;

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

    public Action onDeviceChange;


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


    void Start()
    {
        InputSystem.onDeviceChange += OnDeviceChanged;
    }

    void OnDeviceChanged(InputDevice device, InputDeviceChange change)
    {
        if (Gamepad.all.Count == 0 || Gamepad.current == null) inputType = InputType.Keyboard;
        else if (Gamepad.current is XInputController) inputType = InputType.Xbox;
        else inputType = InputType.PS4;

        switch (change)
        {
            case InputDeviceChange.Added:
                onDeviceChange?.Invoke();
                break;
            case InputDeviceChange.Disconnected:
                onDeviceChange?.Invoke();
                break;
            case InputDeviceChange.Reconnected:
                onDeviceChange?.Invoke();
                break;
            case InputDeviceChange.Removed:
                // Remove from Input System entirely; by default, Devices stay in the system once discovered.
                break;
            default:

                break;
        }
    }
}
