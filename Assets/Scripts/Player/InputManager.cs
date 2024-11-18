using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    static InputManager instance;

    [SerializeField] PlayerInputManager inputManager;
    [SerializeField] PlayerInput meleePlayer;
    [SerializeField] PlayerInput rangedPlayer;

    static InputDevice[] meleePlayerDevices;
    static InputDevice[] rangedPlayerDevices;



    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        InputSystem.onDeviceChange += OnDeviceChanged;
    }

    static void OnDeviceChanged(InputDevice device, InputDeviceChange change)
    {
        Debug.Log($"Device {change}: {device.name}");
        switch (change)
        {
            case InputDeviceChange.Added:
                DeviceAdded(device);
                break;
            case InputDeviceChange.Disconnected:
                // Device got unplugged.
                break;
            case InputDeviceChange.Reconnected:
                // Plugged back in.
                break;
            case InputDeviceChange.Removed:
                // Remove from Input System entirely; by default, Devices stay in the system once discovered.
                break;
            default:

                break;
        }
    }

    void Start()
    {
        if (!instance)
            instance = this;
        if (!inputManager)
            inputManager = FindFirstObjectByType<PlayerInputManager>();
        Debug.Log($"Melee player ID: {meleePlayer.playerIndex}, ranged player ID: {rangedPlayer.playerIndex}");
        ReassignDevices();
    }

    static void DeviceAdded(InputDevice device)
    {
        ReassignDevices();
    }

    static void ReassignDevices()
    {
        Debug.LogWarning("Reassigning devices!");
        List<InputDevice> meleeDevices = new();
        List<InputDevice> rangedDevices = new();
        bool meleeHasGamepad = false;
        bool rangedHasGamepad = false;
        bool rangedHasMouse = false;


        foreach (InputDevice device in InputSystem.devices)
        {
            string deviceClass = device.description.deviceClass;
            Debug.Log($"Iterating through device: {device.name} {device.description}");
            if (deviceClass == "Keyboard")
            {
                meleeDevices.Add(device);
                rangedDevices.Add(device);
            }
            else if (deviceClass == "Gamepad")
            {
                if (!rangedHasGamepad)
                {
                    rangedDevices.Add(device);
                    rangedHasGamepad = true;
                }
                else if (!meleeHasGamepad)
                {
                    meleeDevices.Add(device);
                    meleeHasGamepad = true;
                }
            }
            else if (deviceClass == "Mouse")
            {
                if (!rangedHasMouse)
                {
                    rangedDevices.Add(device);
                    rangedHasMouse = true;
                }
            }
        }
        meleePlayerDevices = meleeDevices.ToArray();
        rangedPlayerDevices = rangedDevices.ToArray();
        if (instance)
        {
            instance.AssignDeviceToMeleePlayer(meleePlayerDevices);
            instance.AssignDeviceToRangedPlayer(rangedPlayerDevices);
        }
    }

    void AssignDeviceToMeleePlayer(InputDevice[] devices)
    {
        meleePlayer.SwitchCurrentControlScheme("Melee", devices);
        foreach (InputDevice device in devices)
        {
            Debug.Log("Melee device assigned: " + device.description.deviceClass);
        }
    }
    void AssignDeviceToRangedPlayer(InputDevice[] devices)
    {
        rangedPlayer.SwitchCurrentControlScheme("Ranged", devices);
        foreach (InputDevice device in devices)
        {
            Debug.Log("Ranged device assigned: " + device.description.deviceClass);
        }
    }
}
