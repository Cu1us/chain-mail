using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    #region Fields
    [Header("Status")]
    [ReadOnlyInspector] public AnchorStatus anchorStatus;


    [ReadOnlyInspector] public float rotationalVelocity;


    [Header("Settings")]
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;
    [SerializeField] float maxRotationSpeed;
    [SerializeField] float rotationSpeedCap;
    [SerializeField] float rotationAcceleration;
    [SerializeField] float rotationDeceleration;
    [SerializeField] float swapPlacesForce;
    [SerializeField] float extendChainSpeed;
    [SerializeField] bool useSwapAimbot;
    [SerializeField] float aimbotTargetDistance;

    [Header("Advanced settings")]
    [SerializeField] float heldPivotOffset;
    [Tooltip("After the chain has reached this fraction of the max rotation speed, players cannot change the rotational direction until it has stopped")]
    [SerializeField][Range(0, 1)] float forcePreserveMomentumThreshold;
    [SerializeField][Min(0)] float pivotReadjustToCenterTime;
    [SerializeField] AnimationCurve pivotReadjustToCenterCurve;

    [Header("Connections")]
    public SwingableObject Player;
    public SwingableObject Rock;
    [SerializeField] PlayerInputData inputData;

    [Header("References")]
    [ReadOnlyInspector][SerializeField] LineRenderer lineRenderer;
    [ReadOnlyInspector][SerializeField] EdgeCollider2D edgeCollider;

    // Properties
    public Vector2 Pivot { get { return Vector2.Lerp(Player.position, Rock.position, localPivot); } }
    public Vector2 Center { get { return (Player.position + Rock.position) / 2; } }
    public SwingableObject Anchor { get { return anchorStatus switch { AnchorStatus.PLAYER => Player, AnchorStatus.ROCK => Rock, _ => null }; } }
    public SwingableObject Swingee { get { return anchorStatus switch { AnchorStatus.PLAYER => Rock, AnchorStatus.ROCK => Player, _ => null }; } }

    // Local variables
    float localPivot;
    public float currentChainLength { get; private set; }
    float lastChainHeldTime;
    #endregion

    void Start()
    {
        BindEvents();
    }

    #region Events
    void BindEvents()
    {
        inputData.onChainSwap += SwapPlaces;
        inputData.onSwitchAnchor += SwitchAnchor;
        Player.onKnockedChain += OnKnockedWhileSwung;
        Rock.onKnockedChain += OnKnockedWhileSwung;
        Player.onSwingIntoWall += OnSwingIntoWall;
        Rock.onSwingIntoWall += OnSwingIntoWall;
    }
    void OnSwingIntoWall()
    {
        rotationalVelocity = -rotationalVelocity;
    }
    void OnKnockedWhileSwung(float amount)
    {
        if (anchorStatus == AnchorStatus.NONE) return;
        float chainLength = currentChainLength * (anchorStatus == AnchorStatus.ROCK ? 1 : (1 - heldPivotOffset));
        rotationalVelocity += amount * 114.591559026164f / chainLength * Mathf.Sign(rotationalVelocity);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyMovement enemy))
        {
            enemy.Stumble();
        }
    }
    #endregion

    #region Main Loop
    void Update()
    {
        currentChainLength = Vector2.Distance(Player.position, Rock.position);

        ApplyConstraint();

        UpdatePivot();

        AccelerateBasedOnInput();

        ExtendChainByInput();

        RotateChain();

        Render();

        PositionHitbox();
    }

    // void ExtendChainBySpeed()
    // {
    //     if (Mathf.Abs(rotationalVelocity) < 10f || AnchorStatus == AnchorStatus.NONE || currentChainLength >= maxDistance - 0.05f) return;
    //     float pushDistance = Mathf.Abs(rotationalVelocity / 720) * chainExtendRateWhenSwung;
    //     Grabee.MoveTowards(Center, -pushDistance);
    // }

    void ApplyConstraint()
    {
        if (currentChainLength > maxDistance)
        {
            float stretchedDistance = (currentChainLength - maxDistance) / 2;
            Vector2 center = Center;
            Player.MoveTowards(center, stretchedDistance);
            Rock.MoveTowards(center, stretchedDistance);

            ConstrainVelocity(Player);
            ConstrainVelocity(Rock);

            void ConstrainVelocity(SwingableObject player)
            {
                Vector2 direction = (player.position - center).normalized;
                float dot = Vector2.Dot(player.velocity, direction);
                player.velocity -= direction * dot;
            }
        }
    }
    void UpdatePivot()
    {
        if (anchorStatus == AnchorStatus.NONE && inputData.chainRotationalInput != 0)
        {
            SetAnchor(AnchorStatus.PLAYER);
        }

        if (anchorStatus != AnchorStatus.NONE)
        {
            lastChainHeldTime = Time.time;
        }
        switch (anchorStatus)
        {
            case AnchorStatus.PLAYER:
                localPivot = heldPivotOffset;
                break;
            case AnchorStatus.ROCK:
                localPivot = 1;
                break;
            case AnchorStatus.NONE:
                // Animate pivot back to center
                float pivotAnimationProgress = Mathf.Clamp((Time.time - lastChainHeldTime) / pivotReadjustToCenterTime, 0, 1);
                pivotAnimationProgress = pivotReadjustToCenterCurve.Evaluate(pivotAnimationProgress);
                float pivotOffsetByLength = heldPivotOffset / currentChainLength;
                localPivot = pivotOffsetByLength + pivotAnimationProgress * (0.5f - pivotOffsetByLength);
                break;
        }
        Player.beingGrabbed = anchorStatus == AnchorStatus.ROCK && rotationalVelocity != 0;
        Rock.beingGrabbed = anchorStatus == AnchorStatus.PLAYER && rotationalVelocity != 0;
    }
    void AccelerateBasedOnInput()
    {
        if (anchorStatus != AnchorStatus.NONE && inputData.chainRotationalInput != 0)
        {
            float targetRotVelocity = -inputData.chainRotationalInput * maxRotationSpeed;
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
        if (Mathf.Abs(rotationalVelocity) > rotationSpeedCap)
        {
            rotationalVelocity = rotationSpeedCap * Mathf.Sign(rotationalVelocity);
        }
    }
    void ExtendChainByInput()
    {
        if (inputData.chainExtendInput == 0 || anchorStatus == AnchorStatus.NONE || rotationalVelocity == 0 || Player.velocity.sqrMagnitude > 1f || Anchor.velocity.sqrMagnitude > 1f) return;
        float pushDistance = inputData.chainExtendInput * extendChainSpeed * Time.deltaTime;

        if (currentChainLength + pushDistance > maxDistance) pushDistance = maxDistance - currentChainLength;
        if (currentChainLength + pushDistance < minDistance) pushDistance = minDistance - currentChainLength;

        Swingee.MoveTowards(Center, -pushDistance);
    }
    void RotateChain()
    {
        Player.swingVelocity = rotationalVelocity / 114.591559026164f * currentChainLength * localPivot;
        Rock.swingVelocity = rotationalVelocity / 114.591559026164f * currentChainLength * (1 - localPivot);

        if (rotationalVelocity == 0) return;
        float rotation = rotationalVelocity * Time.deltaTime / currentChainLength;
        if (anchorStatus == AnchorStatus.NONE)
        {
            Player.RotateAround(Pivot, rotation);
            Rock.RotateAround(Pivot, rotation);
        }
        else
        {
            Swingee.RotateAround(Pivot, rotation);
            if (heldPivotOffset > 0)
                Anchor.RotateAround(Pivot, rotation);

            Swingee.swingForwardDirection = Vector2.Perpendicular((Swingee.position - Anchor.position).normalized) * Mathf.Sign(rotationalVelocity);
            Debug.DrawRay(Swingee.position, Swingee.swingForwardDirection, Color.red, 5f);
            Anchor.swingForwardDirection = Vector2.zero;
        }
    }
    void Render()
    {
        lineRenderer.SetPosition(0, Player.position);
        lineRenderer.SetPosition(1, Rock.position);
    }
    void PositionHitbox()
    {
        Vector2 pointA = Vector2.MoveTowards(Player.position, Center, 0.5f);
        Vector2 pointB = Vector2.MoveTowards(Rock.position, Center, 0.5f);

        edgeCollider.SetPoints(new List<Vector2> { pointA, pointB });
    }
    #endregion

    void SetAnchor(AnchorStatus anchor)
    {
        anchorStatus = anchor;
    }
    void SwitchAnchor()
    {
        Debug.Log("Swapping anchor!");
        if (anchorStatus == AnchorStatus.PLAYER)
            SetAnchor(AnchorStatus.ROCK);
        else if (anchorStatus == AnchorStatus.ROCK)
            SetAnchor(AnchorStatus.PLAYER);
    }
    public void SwapPlaces()
    {
        if (Player.velocity.sqrMagnitude > 20 || Rock.velocity.sqrMagnitude > 20) return;
        /*if (Mathf.Abs(rotationalVelocity) == 0)
        {
            Player.Launch((Rock.position - Player.position).normalized * swapPlacesForce);
            Rock.Launch((Player.position - Rock.position).normalized * swapPlacesForce);
            return;
        }*/

        SwingableObject toSwap = Swingee;
        SwingableObject swapAnchor = Anchor;

        rotationalVelocity = 0;
        toSwap.lastSwapTime = Time.time;

        Vector2 swapToPos = swapAnchor.position + (swapAnchor.position - toSwap.position).normalized * maxDistance;
        Debug.DrawLine(toSwap.position, swapToPos, Color.gray, 2f);

        if (useSwapAimbot && EnemyMovement.EnemyList.Count > 0)
        {
            Vector2 closestPos = Vector2.zero;
            float closestDistance = aimbotTargetDistance * aimbotTargetDistance;
            foreach (EnemyMovement enemy in EnemyMovement.EnemyList)
            {
                float distance = ((Vector2)enemy.transform.position - swapToPos).sqrMagnitude;
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPos = enemy.transform.position;
                }
            }
            if (closestPos != Vector2.zero) swapToPos = closestPos;
        }
        Debug.DrawLine(toSwap.position, swapToPos, Color.green, 2f);

        toSwap.Launch((swapToPos - toSwap.position).normalized * swapPlacesForce * 2);
    }

    void Reset()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        lineRenderer.positionCount = 2;
    }





    public enum AnchorStatus
    {
        PLAYER,
        ROCK,
        NONE
    }
}