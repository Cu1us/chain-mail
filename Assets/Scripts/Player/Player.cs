using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputData))]
public class Player : MonoBehaviour
{
    [SerializeField] IWeapon weapon;
    PlayerInputData playerInputData;

    private void Start()
    {
        playerInputData = GetComponent<PlayerInputData>();
        playerInputData.onAttackPress += AttackPress;
        playerInputData.onAttackRelease += AttackRelease;
    }

    void AttackPress()
    {
        weapon.AttackPress();
    }

    void AttackRelease()
    {
        weapon.AttackRelease();
    }
}
