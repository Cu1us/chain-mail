using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour, IKnockable
{
    public Vector2 position { get { return transform.position; } set { transform.position = value; } }
    public Vector2 velocity;

    public bool grabbing = false;
    public bool grabbed = false;
    public int rotationalInput;
    public Action onTrySwapPlaces;

    [Header("Settings")]
    [SerializeField] float movementSpeed;
    [SerializeField] float swapPlacesInputWindow = 0.2f;
    [SerializeField] float velocityDeceleration;



    [Header("Controls")]
    [SerializeField] KeyCode up;
    [SerializeField] KeyCode left;
    [SerializeField] KeyCode down;
    [SerializeField] KeyCode right;
    [SerializeField] KeyCode rotateClockwise;
    [SerializeField] KeyCode rotateCounterclockwise;


    float lastClockwiseRotationPress = float.NegativeInfinity;
    float lastCounterclockwiseRotationPress = float.NegativeInfinity;


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

    void Update()
    {
        position += velocity * Time.deltaTime;
        velocity = Vector2.MoveTowards(velocity, Vector2.zero, velocityDeceleration * Time.deltaTime);

        if (Input.GetKeyDown(rotateClockwise))
            lastClockwiseRotationPress = Time.time;
        if (Input.GetKeyDown(rotateCounterclockwise))
            lastCounterclockwiseRotationPress = Time.time;

        if (!grabbing && !grabbed)
            Move();
        transform.position = position;

        if (Input.GetKey(rotateClockwise) == Input.GetKey(rotateCounterclockwise))
            rotationalInput = 0;
        else
            rotationalInput = Input.GetKey(rotateClockwise) ? 1 : -1;

        grabbing = rotationalInput != 0;
    }

    void Move()
    {
        Vector2 moveVector = new(
            Input.GetKey(left) == Input.GetKey(right) ? 0 : Input.GetKey(right) ? 1 : -1,
            Input.GetKey(up) == Input.GetKey(down) ? 0 : Input.GetKey(up) ? 1 : -1
        );

        if (Time.time - lastClockwiseRotationPress < swapPlacesInputWindow && Time.time - lastCounterclockwiseRotationPress < swapPlacesInputWindow)
        {
            SwapPlaces();
        }

        position += moveVector * movementSpeed * Time.deltaTime;
    }
    void SwapPlaces()
    {
        lastClockwiseRotationPress = lastCounterclockwiseRotationPress = 0;
        onTrySwapPlaces?.Invoke();
        Debug.Log("Tried to swap!", gameObject);
    }

    public static implicit operator Vector2(Movement m) => m.position;
    public static implicit operator Vector3(Movement m) => m.position;
}

public interface IKnockable
{
    public void Launch(Vector2 force);
}