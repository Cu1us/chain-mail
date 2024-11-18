using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChainMover : MonoBehaviour
{
    void OnMovement(InputValue value)
    {
        Vector2 movement = value.Get<Vector2>();
        transform.position += (Vector3)movement * Time.deltaTime;
    }
    void OnChainRotation(InputValue value)
    {
        Debug.Log(name + " Rotating chain! " + value);
    }
    void OnAttack(InputValue value)
    {
        Debug.Log(name + " Attacking! " + value);
    }
}
