using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Status")]
    public Vector2 position;

    public Mode mode = Mode.ACTIVE;

    public bool grabbing = false;
    public int rotationalInput;

    [Header("Settings")]
    [SerializeField] float movementSpeed;

    [Header("Controls")]
    [SerializeField] KeyCode up;
    [SerializeField] KeyCode left;
    [SerializeField] KeyCode down;
    [SerializeField] KeyCode right;
    [SerializeField] KeyCode grab;
    [SerializeField] KeyCode rotateClockwise;
    [SerializeField] KeyCode rotateCounterclockwise;



    public void MoveTowards(Vector2 target, float distance)
    {
        position = Vector2.MoveTowards(position, target, distance);
    }
    public void RotateAround(Vector2 pivot, float angle)
    {
        //transform.Rotate(Vector3.forward, pivot, angle);
    }

    void Update()
    {
        Vector2 moveVector = new(
            Input.GetKey(right) ? 1 : Input.GetKey(left) ? -1 : 0,
            Input.GetKey(up) ? 1 : Input.GetKey(down) ? -1 : 0
        );
        rotationalInput = Input.GetKey(rotateClockwise) ? 1 : Input.GetKey(rotateCounterclockwise) ? -1 : 0;
        position += moveVector * movementSpeed * Time.deltaTime;
        transform.position = position;

        grabbing = Input.GetKey(grab);
    }

    public enum Mode
    {
        ACTIVE, // normal behavior
        KINEMATIC // inactive
    }
    public static implicit operator Vector2(Movement m) => m.position;
    public static implicit operator Vector3(Movement m) => m.position;
}
