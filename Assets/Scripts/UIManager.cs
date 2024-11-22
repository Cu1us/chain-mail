using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject ControllerUI;

    bool ControllerUIEnable = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            DisableControllsUI();
        }
    }

    void DisableControllsUI()
    {
        ControllerUIEnable = !ControllerUIEnable;
        ControllerUI.SetActive(ControllerUIEnable);
    }
}
