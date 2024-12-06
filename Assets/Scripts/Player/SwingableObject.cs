using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwingableObject : MonoBehaviour, IKnockable
{
    [Header("Status")]
    [ReadOnlyInspector] public Vector2 velocity;

    [ReadOnlyInspector] public bool beingGrabbed;
    [ReadOnlyInspector] public float lastSwapTime;
    [ReadOnlyInspector] public float swingVelocity;
    [ReadOnlyInspector] public Vector2 swingForwardDirection;

    [Header("Settings")]
    [SerializeField] protected float velocityDeceleration;
    [SerializeField] protected float maxSqrVelocity;

    [SerializeField] Collider2D[] canCollideWith;
    [SerializeField] Vector2 localPointOfCollision;

    // Events
    public Action<float> onKnockedChain;
    public Action onSwingIntoWall;

    // Properties
    public Vector2 position { get { return transform.position; } set { SetPosition(value, transform.position); } }
    public bool beingSwapped { get { return velocity.sqrMagnitude > 1f && Time.time - lastSwapTime < 0.75f; } }

    protected Vector2 translation;

    bool IsPointInACollider(Vector2 position)
    {
        foreach (Collider2D collider in canCollideWith)
        {
            if (collider.OverlapPoint(position))
                return true;
        }
        return false;
    }

    public void SetPosition(Vector2 newPos, Vector2 oldPos)
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
            RaycastHit2D hit = Physics2D.Raycast(oldPos, dirToWall, 2f, LayerMask.GetMask("Environment"));
            Debug.DrawRay(oldPos, dirToWall, Color.blue, 2f);
            if (hit.collider == null) return;
            float dot = Vector2.Dot(dirToWall, -hit.normal);
            newPos += hit.normal * dot * toWall.magnitude;
            transform.position = newPos;

            float velocityDot = Vector2.Dot(velocity.normalized, -hit.normal);
            velocity += hit.normal * dot * velocity.magnitude;

            if (beingGrabbed && Mathf.Abs(swingVelocity) > 10f)
            {
                float swingDot = Vector2.Dot(swingForwardDirection, -hit.normal);
                if (swingDot > 0.25f)
                {
                    onSwingIntoWall?.Invoke();
                }
            }
        }
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

    protected virtual void Update()
    {
        translation += velocity * Time.deltaTime;
        if (velocity.sqrMagnitude > maxSqrVelocity)
        {
            velocity = velocity.normalized * Mathf.Sqrt(maxSqrVelocity);
        }
        velocity = Vector2.MoveTowards(velocity, Vector2.zero, velocityDeceleration * Time.deltaTime);
        position += translation;
        translation = Vector2.zero;
    }
}

public interface IKnockable
{
    public void Launch(Vector2 force);
}