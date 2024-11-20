using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputData))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Status")]
    [ReadOnlyInspector] public Vector2 velocity;

    [ReadOnlyInspector] public bool beingGrabbed;
    [ReadOnlyInspector] public float swingVelocity;
    [Header("Settings")]
    [SerializeField] float movementSpeed;

    [Header("References")]
    [ReadOnlyInspector][SerializeField] PlayerInputData Input;

    // Properties
    public Vector2 position { get { return transform.position; } set { transform.position = value; } }

    // Local variables
    //

    void Reset()
    {
        Input = GetComponent<PlayerInputData>();
    }

    void Update()
    {
        if (!beingGrabbed && Input.chainRotationalInput == 0) position += Input.movementInput * movementSpeed * Time.deltaTime;
    }

    public void MoveTowards(Vector2 target, float distance)
    {
        position = Vector2.MoveTowards(position, target, distance);
    }
    public void RotateAround(Vector2 pivot, float angle)
    {
        Vector2 direction = position - pivot;
        direction = Quaternion.Euler(0, 0, angle) * direction;
        position = pivot + direction;
    }
    public void Launch(Vector2 force)
    {
        velocity += force;
    }
}
