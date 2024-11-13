using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class Chain : MonoBehaviour
{
    [Header("Status")]
    GrabStatus grabStatus;
    Vector2 worldPivot;
    float localPivot { set { SetPivotByDistance(value); } }
    Movement grabber
    {
        get
        {
            return grabStatus switch { GrabStatus.A => EntityA, GrabStatus.B => EntityB, _ => null };
        }
    }
    Movement grabee
    {
        get
        {
            return grabStatus switch { GrabStatus.A => EntityB, GrabStatus.B => EntityA, _ => null };
        }
    }

    [Header("Attachments")]
    public Movement EntityA;
    public Movement EntityB;

    [Header("Settings")]
    [SerializeField] float maxDistance;
    [SerializeField] float maxRotationSpeed;
    [SerializeField] float rotationAcceleration;
    [SerializeField] float rotationDeceleration;
    [SerializeField] float swapPlacesForce;

    [Header("Advanced settings")]
    [SerializeField] float heldPivotOffset;
    [Tooltip("After the chain has reached this fraction of the max rotation speed, players cannot change the rotational direction until it has stopped")]
    [SerializeField][Range(0, 1)] float forcePreserveMomentumThreshold;
    [SerializeField][Min(0)] float pivotReadjustToCenterTime;


    [Header("References")]
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] EdgeCollider2D chainCollider;



    Vector2 center;
    float distance;
    float rotationalVelocity;
    float lastChainHeldTime;

    void Reset()
    {
        lineRenderer = GetComponent<LineRenderer>();
        chainCollider = GetComponent<EdgeCollider2D>();
        lineRenderer.positionCount = 2;
    }

    void Start()
    {
        EntityA.onTrySwapPlaces += SwapPlaces;
        EntityB.onTrySwapPlaces += SwapPlaces;

    }

    void SwapPlaces()
    {
        EntityA.Launch((EntityB.position - EntityA.position).normalized * swapPlacesForce);
        EntityB.Launch((EntityA.position - EntityB.position).normalized * swapPlacesForce);
    }

    void Update()
    {
        distance = Vector2.Distance(EntityA.position, EntityB.position);
        center = (EntityA.position + EntityB.position) / 2;

        ApplyConstraint();

        UpdatePivot();

        AccelerateBasedOnInput();

        RotateChain();

        RenderLine();

        PositionHitbox();
    }

    void PositionHitbox()
    {
        chainCollider.SetPoints(new List<Vector2> { EntityA, EntityB });
    }

    private void AccelerateBasedOnInput()
    {
        if (grabStatus != GrabStatus.NONE && grabber.rotationalInput != 0)
        {
            float targetRotVelocity = grabber.rotationalInput * maxRotationSpeed;
            float acceleration = rotationAcceleration * Time.deltaTime;
            if (Mathf.Sign(rotationalVelocity) != Mathf.Sign(targetRotVelocity))
            {
                if (Mathf.Abs(rotationalVelocity) / maxRotationSpeed > forcePreserveMomentumThreshold)
                    targetRotVelocity *= -1;
                else
                    acceleration *= 2;
            }
            rotationalVelocity = Mathf.MoveTowards(rotationalVelocity, targetRotVelocity, acceleration);
        }
        else
        {
            rotationalVelocity = Mathf.MoveTowards(rotationalVelocity, 0, rotationDeceleration * Time.deltaTime);
        }
    }

    void RotateChain()
    {
        if (rotationalVelocity == 0) return;
        float rotation = rotationalVelocity * Time.deltaTime;
        if (grabStatus == GrabStatus.NONE)
        {
            EntityA.RotateAround(worldPivot, rotation);
            EntityB.RotateAround(worldPivot, rotation);
        }
        else
        {
            grabee.RotateAround(worldPivot, rotation);
            if (heldPivotOffset != 0 && worldPivot != center)
                grabber.RotateAround(worldPivot, rotation);
        }
    }
    void UpdatePivot()
    {
        if (!EntityA.grabbing && !EntityB.grabbing)
        {
            grabStatus = GrabStatus.NONE;
            float pivotAnimationProgress = Mathf.Clamp((Time.time - lastChainHeldTime) / pivotReadjustToCenterTime, 0, 1);
            SetPivotByDistance(pivotAnimationProgress * 0.5f);
        }
        else if (EntityA.grabbing && !EntityB.grabbing)
        {
            grabStatus = GrabStatus.A;
            worldPivot = Vector2.MoveTowards(EntityA, center, heldPivotOffset);
            lastChainHeldTime = Time.time;
        }
        else if (!EntityA.grabbing && EntityB.grabbing)
        {
            grabStatus = GrabStatus.B;
            worldPivot = Vector2.MoveTowards(EntityB, center, heldPivotOffset);
            lastChainHeldTime = Time.time;
        }

        EntityA.grabbed = grabStatus == GrabStatus.B;
        EntityB.grabbed = grabStatus == GrabStatus.A;
    }

    // 0 = at entityA, 1 = at entityB, 0.5 = at center
    void SetPivotByDistance(float distance)
    {
        worldPivot = Vector2.Lerp(EntityA, EntityB, distance);
    }

    void RenderLine()
    {
        lineRenderer.SetPosition(0, EntityA);
        lineRenderer.SetPosition(1, EntityB);
    }
    void ApplyConstraint()
    {
        if (distance > maxDistance)
        {
            float stretchedDistance = (distance - maxDistance) / 2;
            EntityA.MoveTowards(center, stretchedDistance);
            EntityB.MoveTowards(center, stretchedDistance);

            ConstrainVelocity(EntityA);
            ConstrainVelocity(EntityB);

            void ConstrainVelocity(Movement entity)
            {
                Vector2 direction = (center - entity).normalized;
                float dot = Vector2.Dot(entity.velocity, direction);
                entity.velocity -= direction * dot;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyMovment enemy))
        {
            enemy.Stumble();
        }
    }

    public enum GrabStatus
    {
        NONE,
        A,
        B
    }
}
