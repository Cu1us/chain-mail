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


    void Start()
    {
        if (!instance)
            instance = this;
        if (!inputManager)
            inputManager = FindFirstObjectByType<PlayerInputManager>();
        ReassignDevices();
    }

    static void DeviceAdded(InputDevice device)
    {
        ReassignDevices();
    }

    static void ReassignDevices()
    {
        List<InputDevice> meleeDevices = new();
        List<InputDevice> rangedDevices = new();
        bool meleeHasGamepad = false;
        bool rangedHasGamepad = false;
        bool rangedHasMouse = false;


        foreach (InputDevice device in InputSystem.devices)
        {
            string deviceClass = device.description.deviceClass;
            if (deviceClass == "Keyboard")
            {
                meleeDevices.Add(device);
                rangedDevices.Add(device);
            }
            else if (IsGamepad(device))
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
    }
    void AssignDeviceToRangedPlayer(InputDevice[] devices)
    {
        rangedPlayer.SwitchCurrentControlScheme("Ranged", devices);
    }

    static bool IsGamepad(InputDevice device)
    {
        if (device.description.deviceClass.ToLower() == "gamepad")
            return true;
        if (device.name.ToLower().Contains("gamepad"))
            return true;
        return false;
    }
}
