using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class Chain : MonoBehaviour
{
    // Exposed fields

    [Header("Status")]
    [ReadOnlyInspector] public GrabStatus grabStatus;
    [ReadOnlyInspector] public float rotationalVelocity;


    [Header("Settings")]
    [SerializeField] float maxDistance;
    [SerializeField] float maxRotationSpeed;
    [SerializeField] float rotationAcceleration;
    [SerializeField] float rotationDeceleration;
    [SerializeField] float swapPlacesForce;
    [SerializeField] float chainExtendRateWhenSwung;

    [Header("Advanced settings")]
    [SerializeField] float heldPivotOffset;
    [Tooltip("After the chain has reached this fraction of the max rotation speed, players cannot change the rotational direction until it has stopped")]
    [SerializeField][Range(0, 1)] float forcePreserveMomentumThreshold;
    [SerializeField][Min(0)] float pivotReadjustToCenterTime;
    [SerializeField] AnimationCurve pivotReadjustToCenterCurve;


    [Header("Players")]
    public PlayerMovement PlayerA;
    public PlayerMovement PlayerB;
    public PlayerInputData PlayerAInput;
    public PlayerInputData PlayerBInput;


    [Header("References")]
    [ReadOnlyInspector][SerializeField] LineRenderer lineRenderer;
    [ReadOnlyInspector][SerializeField] EdgeCollider2D edgeCollider;

    // Properties
    public Vector2 Pivot { get { return Vector2.Lerp(PlayerA.position, PlayerB.position, localPivot); } }
    public Vector2 Center { get { return (PlayerA.position + PlayerB.position) / 2; } }
    public PlayerMovement Grabber { get { return grabStatus switch { GrabStatus.A => PlayerA, GrabStatus.B => PlayerB, _ => null }; } }
    public PlayerMovement Grabee { get { return grabStatus switch { GrabStatus.A => PlayerB, GrabStatus.B => PlayerA, _ => null }; } }
    public PlayerInputData GrabberInput { get { return grabStatus switch { GrabStatus.A => PlayerAInput, GrabStatus.B => PlayerBInput, _ => null }; } }
    public PlayerInputData GrabeeInput { get { return grabStatus switch { GrabStatus.A => PlayerBInput, GrabStatus.B => PlayerAInput, _ => null }; } }

    // Local variables
    float localPivot;
    float currentChainLength;
    float lastChainHeldTime;


    void Reset()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        lineRenderer.positionCount = 2;
    }
    void Start()
    {
        PlayerAInput.onChainSwap += SwapPlaces;
        PlayerBInput.onChainSwap += SwapPlaces;
    }
    public void SwapPlaces()
    {
        if (PlayerA.velocity.sqrMagnitude > 1 || PlayerB.velocity.sqrMagnitude > 1) return;
        PlayerA.Launch((PlayerB.position - PlayerA.position).normalized * swapPlacesForce);
        PlayerB.Launch((PlayerA.position - PlayerB.position).normalized * swapPlacesForce);
    }

    void Update()
    {
        currentChainLength = Vector2.Distance(PlayerA.position, PlayerB.position);

        ApplyConstraint();

        UpdatePivot();

        AccelerateBasedOnInput();

        ExtendChainBySpeed();

        RotateChain();

        Render();

        PositionHitbox();
    }

    void ExtendChainBySpeed()
    {
        if (rotationalVelocity < 10f || grabStatus == GrabStatus.NONE || currentChainLength >= maxDistance - 0.05f) return;
        float pushDistance = (rotationalVelocity / 720) * chainExtendRateWhenSwung;
        Grabee.MoveTowards(Center, -pushDistance);
    }

    void ApplyConstraint()
    {
        if (currentChainLength > maxDistance)
        {
            float stretchedDistance = (currentChainLength - maxDistance) / 2;
            Vector2 center = Center;
            PlayerA.MoveTowards(center, stretchedDistance);
            PlayerB.MoveTowards(center, stretchedDistance);

            ConstrainVelocity(PlayerA);
            ConstrainVelocity(PlayerB);

            void ConstrainVelocity(PlayerMovement player)
            {
                Vector2 direction = (center - player.position).normalized;
                float dot = Vector2.Dot(player.velocity, direction);
                player.velocity -= direction * dot;
            }
        }
    }

    void AccelerateBasedOnInput()
    {
        if (grabStatus != GrabStatus.NONE && GrabberInput.chainRotationalInput != 0)
        {
            float targetRotVelocity = GrabberInput.chainRotationalInput * maxRotationSpeed;
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
        PlayerA.swingVelocity = rotationalVelocity / 114.591559026164f * currentChainLength * localPivot;
        PlayerB.swingVelocity = rotationalVelocity / 114.591559026164f * currentChainLength * (1 - localPivot);

        if (rotationalVelocity == 0) return;
        float rotation = rotationalVelocity * Time.deltaTime;
        if (grabStatus == GrabStatus.NONE)
        {
            PlayerA.RotateAround(Pivot, rotation);
            PlayerB.RotateAround(Pivot, rotation);
        }
        else
        {
            Grabee.RotateAround(Pivot, rotation);
            if (heldPivotOffset > 0)
                Grabber.RotateAround(Pivot, rotation);

            Grabee.swingForwardDirection = Vector2.Perpendicular((Grabee.position - Grabber.position).normalized) * Mathf.Sign(rotationalVelocity);
            Debug.DrawRay(Grabee.position, Grabee.swingForwardDirection, Color.red, 5f);
            Grabber.swingForwardDirection = Vector2.zero;
        }
    }
    void UpdatePivot()
    {
        bool playerATryingToRotate = PlayerAInput.chainRotationalInput != 0;
        bool playerBTryingToRotate = PlayerBInput.chainRotationalInput != 0;

        // Decide who is grabbing, and set pivot accordingly
        if (!playerATryingToRotate && !playerBTryingToRotate)
        {
            grabStatus = GrabStatus.NONE;

            // Animate pivot back to center
            float pivotAnimationProgress = Mathf.Clamp((Time.time - lastChainHeldTime) / pivotReadjustToCenterTime, 0, 1);
            pivotAnimationProgress = pivotReadjustToCenterCurve.Evaluate(pivotAnimationProgress);
            float pivotOffsetByLength = heldPivotOffset / currentChainLength;
            localPivot = pivotOffsetByLength + pivotAnimationProgress * (0.5f - pivotOffsetByLength);
        }
        else
        {
            lastChainHeldTime = Time.time;
            if (playerATryingToRotate && !playerBTryingToRotate)
            {
                grabStatus = GrabStatus.A;
                localPivot = heldPivotOffset;
            }
            else if (!playerATryingToRotate && playerBTryingToRotate)
            {
                grabStatus = GrabStatus.B;
                localPivot = 1 - heldPivotOffset;
                lastChainHeldTime = Time.time;
            }
        }

        PlayerA.beingGrabbed = grabStatus == GrabStatus.B;
        PlayerB.beingGrabbed = grabStatus == GrabStatus.A;
    }
    void PositionHitbox()
    {
        Vector2 pointA = Vector2.MoveTowards(PlayerA.position, Center, 0.5f);
        Vector2 pointB = Vector2.MoveTowards(PlayerA.position, Center, 0.5f);

        edgeCollider.SetPoints(new List<Vector2> { pointA, pointB });
    }
    void Render()
    {
        lineRenderer.SetPosition(0, PlayerA.position);
        lineRenderer.SetPosition(1, PlayerB.position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Pathfinding enemy))
        {
            enemy.Stumble();
        }
    }

    public enum GrabStatus
    {
        A,
        B,
        NONE
    }
}
