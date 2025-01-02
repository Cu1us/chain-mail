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
    [ReadOnlyInspector] public bool fallingIntoHole = false;

    [Header("Settings")]
    [SerializeField] protected float velocityDeceleration;
    [SerializeField] protected float maxSqrVelocity;
    [SerializeField] protected float maximumVelocityToFallInHole;

    [SerializeField] Collider2D[] canCollideWith;
    [SerializeField] Vector2 localPointOfCollision;
    [SerializeField] ButtonPrompt dragOutOfHoleButton;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected SpriteRenderer shadow;

    [SerializeField] protected TrailRenderer swapTrail;


    // Events
    public Action<float> onKnockedChain;
    public Action<Vector2> onSwingIntoWall;
    public Action<bool> onFallIntoHole;

    // Properties
    public Vector2 position { get { return transform.position; } set { if (!fallingIntoHole) SetPosition(value, transform.position); } }
    public bool beingSwapped { get { return velocity.sqrMagnitude > 1f && Time.time - lastSwapTime < 0.75f; } }

    protected bool facingRight = true;

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
        Vector2 collisionPointOld = oldPos + localPointOfCollision;
        if (!IsPointInACollider(collisionCheckPoint))
        {
            transform.position = newPos;
        }
        else
        {
            Vector2 toWall = collisionCheckPoint - collisionPointOld;
            Vector2 dirToWall = toWall.normalized;
            RaycastHit2D hit = Physics2D.Raycast(collisionPointOld, dirToWall, 2f, LayerMask.GetMask("Environment"));
            Debug.DrawRay(collisionPointOld, dirToWall, Color.blue, 2f);
            if (hit.collider == null) return;
            float dot = Vector2.Dot(dirToWall, -hit.normal);
            collisionCheckPoint += hit.normal * dot * toWall.magnitude;
            transform.position = collisionCheckPoint - localPointOfCollision;

            float velocityDot = Vector2.Dot(velocity.normalized, -hit.normal);
            velocity += hit.normal * dot * velocity.magnitude;

            if (beingGrabbed && Mathf.Abs(swingVelocity) > 10f)
            {
                float swingDot = Vector2.Dot(swingForwardDirection, -hit.normal);
                if (swingDot > 0.25f)
                {
                    onSwingIntoWall?.Invoke(hit.normal);
                    //VFX.Spawn(VFXType.GROUND_IMPACT, hit.point, hit.normal);
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
        /*if (fallingIntoHole && force.magnitude > maximumVelocityToFallInHole * 1.1f)
        {
            fallingIntoHole = false;
            onFallIntoHole?.Invoke(false);
        }*/
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
        if (swapTrail) swapTrail.emitting = beingSwapped;
        if (dragOutOfHoleButton)
        {
            dragOutOfHoleButton.visible = fallingIntoHole;
        }
        if (shadow)
        {
            shadow.enabled = !fallingIntoHole;
        }
        if (fallingIntoHole && Time.time - lastSwapTime < 0.5f)
        {
            velocity = Vector2.zero;
        }
        else
        {
            translation += velocity * Time.deltaTime;
            if (velocity.sqrMagnitude > maxSqrVelocity)
            {
                velocity = velocity.normalized * Mathf.Sqrt(maxSqrVelocity);
            }
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, velocityDeceleration * Time.deltaTime);
            position += translation;
            UpdateFacingDir();
            translation = Vector2.zero;
        }
    }

    void UpdateFacingDir()
    {
        if (Mathf.Abs(swingVelocity) < 10f)
        {
            if (translation.x > 0)
            {
                facingRight = true;
            }
            else if (translation.x < 0)
            {
                facingRight = false;
            }
        }
        else
        {
            if (swingForwardDirection.x > 0)
            {
                facingRight = true;
            }
            else if (swingForwardDirection.x < 0)
            {
                facingRight = false;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Trap") && !fallingIntoHole)
        {
            if (Mathf.Abs(((swingVelocity * swingForwardDirection) + velocity).sqrMagnitude) < maximumVelocityToFallInHole * maximumVelocityToFallInHole)
            {
                Vector2 collisionCheckPoint = position + localPointOfCollision;
                if (other.OverlapPoint(collisionCheckPoint))
                    onFallIntoHole?.Invoke(true);
            }
        }
    }
}

public interface IKnockable
{
    public void Launch(Vector2 force);
}