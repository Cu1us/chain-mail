using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour // TODO: IKnockable interface
{
    public Vector2 position { get { return transform.position; } set { transform.position = value; } }

    public Mode mode = Mode.ACTIVE;

    public bool grabbing = false;
    public bool grabbed = false;
    public int rotationalInput;

    [Header("Settings")]
    [SerializeField] float movementSpeed;

    [Header("Controls")]
    [SerializeField] KeyCode up;
    [SerializeField] KeyCode left;
    [SerializeField] KeyCode down;
    [SerializeField] KeyCode right;
    [SerializeField] KeyCode grab;
    [SerializeField] KeyCode rotateCounterclockwise;
    [SerializeField] KeyCode rotateClockwise;



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

    void Update()
    {
        if (!grabbing && !grabbed)
            Move();
        transform.position = position;

        grabbing = Input.GetKey(grab);
        rotationalInput = Input.GetKey(rotateClockwise) ? 1 : Input.GetKey(rotateCounterclockwise) ? -1 : 0;
    }

    void Move()
    {
        Vector2 moveVector = new(
            Input.GetKey(right) ? 1 : Input.GetKey(left) ? -1 : 0,
            Input.GetKey(up) ? 1 : Input.GetKey(down) ? -1 : 0
        );
        position += moveVector * movementSpeed * Time.deltaTime;
    }

    public enum Mode
    {
        ACTIVE, // normal behavior
        GRABBED // inactive
    }
    public static implicit operator Vector2(Movement m) => m.position;
    public static implicit operator Vector3(Movement m) => m.position;
}
