using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PlayerInputData))]
public class PlayerMovement : MonoBehaviour, IKnockable
{
    [Header("Status")]
    [ReadOnlyInspector] public Vector2 velocity;

    [ReadOnlyInspector] public bool beingGrabbed;
    [ReadOnlyInspector] public float swingVelocity;
    [ReadOnlyInspector] public Vector2 swingForwardDirection;

    [Header("Settings")]
    [SerializeField] float movementSpeed;
    [SerializeField] float velocityDeceleration;
    [SerializeField] float maxSqrVelocity;

    [SerializeField] Collider2D[] canCollideWith;
    [SerializeField] Vector2 localPointOfCollision;

    [Header("References")]
    [ReadOnlyInspector][SerializeField] PlayerInputData Input;

    // Events
    public Action<float> onKnockedChain;

    // Properties
    public Vector2 position { get { return transform.position; } set { SetPosition(value, transform.position); } }

    // Local variables
    //

    void SetPosition(Vector2 newPos, Vector2 oldPos)
    {
        if (newPos == oldPos) return;
        Vector2 collisionCheckPoint = newPos + localPointOfCollision;
        if (!IsPointInACollider(newPos))
        {
            transform.position = newPos;
        }
        else
        {
            Vector2 toWall = newPos - oldPos;
            Vector2 dirToWall = toWall.normalized;
            RaycastHit2D hit = Physics2D.Raycast(oldPos, dirToWall, 2f, LayerMask.NameToLayer("Environment"));
            float dot = Vector2.Dot(dirToWall, -hit.normal);
            newPos += hit.normal * dot * toWall.magnitude;
            transform.position = newPos;

            float velocityDot = Vector2.Dot(velocity.normalized, -hit.normal);
            velocity += hit.normal * dot * velocity.magnitude;
        }
    }

    bool IsPointInACollider(Vector2 position)
    {
        foreach (Collider2D collider in canCollideWith)
        {
            if (collider.OverlapPoint(position))
                return true;
        }
        return false;
    }

    void Reset()
    {
        Input = GetComponent<PlayerInputData>();
    }

    void Update()
    {
        Vector2 translation = Vector2.zero;
        if (!beingGrabbed && Input.chainRotationalInput == 0) translation += Input.movementInput * movementSpeed * Time.deltaTime;


        translation += velocity * Time.deltaTime;
        if (velocity.sqrMagnitude > maxSqrVelocity)
        {
            velocity = velocity.normalized * Mathf.Sqrt(maxSqrVelocity);
        }
        velocity = Vector2.MoveTowards(velocity, Vector2.zero, velocityDeceleration * Time.deltaTime);

        position += translation;
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
        if (beingGrabbed)
        {
            float dot = Vector2.Dot(force.normalized, swingForwardDirection.normalized);
            force -= dot * force.normalized;
            onKnockedChain?.Invoke(dot);
        }
        velocity += force;
    }
}

public interface IKnockable
{
    public void Launch(Vector2 force);
}