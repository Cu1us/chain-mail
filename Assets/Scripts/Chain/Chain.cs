using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    #region Fields
    [Header("Status")]
    [ReadOnlyInspector] public AnchorStatus anchorStatus;
    [ReadOnlyInspector] public AnchorStatus fallingIntoHole = AnchorStatus.NONE;



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
    [SerializeField] float fallingIntoHoleDragSpeed;
    [SerializeField] bool useSwapAimbot;
    [SerializeField] float aimbotTargetDistance;

    [Header("Advanced settings")]
    [SerializeField] float heldPivotOffset;
    [Tooltip("After the chain has reached this fraction of the max rotation speed, players cannot change the rotational direction until it has stopped")]
    [SerializeField][Range(0, 1)] float forcePreserveMomentumThreshold;
    [SerializeField][Min(0)] float pivotReadjustToCenterTime;
    [SerializeField] AnimationCurve pivotReadjustToCenterCurve;
    [SerializeField] float knockbackWhenHittingWall;
    [SerializeField][Range(0, 1)] float speedMultiplierWhenSwitchDir;
    [SerializeField] float chainElongationFromSwap;
    [SerializeField] float chainElongationResetDuration;

    [Header("Connections")]
    public SwingableObject Player;
    public SwingableObject Rock;
    [SerializeField] PlayerInputData inputData;

    [Header("References")]
    [ReadOnlyInspector][SerializeField] LineRenderer lineRenderer;
    [ReadOnlyInspector][SerializeField] EdgeCollider2D edgeCollider;
    [SerializeField] Transform aimbotReticle;

    // Properties
    public Vector2 Pivot { get { return Vector2.Lerp(Player.position, Rock.position, localPivot); } }
    public Vector2 Center { get { return (Player.position + Rock.position) / 2; } }
    public SwingableObject Anchor { get { return anchorStatus switch { AnchorStatus.PLAYER => Player, AnchorStatus.ROCK => Rock, _ => null }; } }
    public SwingableObject Swingee { get { return anchorStatus switch { AnchorStatus.PLAYER => Rock, AnchorStatus.ROCK => Player, _ => null }; } }
    public float effectiveMaxDistance { get { return maxDistance + Mathf.Max(0, 1 - ((Time.time - lastSwapTime) / chainElongationResetDuration)) * chainElongationFromSwap; } }

    // Local variables
    float localPivot;
    public float currentChainLength { get; private set; }
    float lastChainHeldTime;
    float lastSwapTime;
    #endregion

    void Start()
    {
        BindEvents();
        fallingIntoHole = AnchorStatus.NONE;
    }

    #region Events
    void BindEvents()
    {
        inputData.onChainSwap += SwapPlaces;
        inputData.onSwitchAnchor += SwitchAnchor;
        inputData.onChainRotate += OnChainRotate;
        Player.onKnockedChain += OnKnockedWhileSwung;
        Rock.onKnockedChain += OnKnockedWhileSwung;
        Player.onSwingIntoWall += OnSwingIntoWall;
        Rock.onSwingIntoWall += OnSwingIntoWall;
        Player.onFallIntoHole += OnPlayerFallIntoHole;
        Rock.onFallIntoHole += OnRockFallIntoHole;
    }
    void OnPlayerFallIntoHole(bool fallingIn)
    {
        if (fallingIn)
            FallIntoHole(AnchorStatus.PLAYER, Player);
        else
            StopFallingIntoHole();
    }
    void OnRockFallIntoHole(bool fallingIn)
    {
        if (fallingIn)
            FallIntoHole(AnchorStatus.ROCK, Rock);
        else
            StopFallingIntoHole();
    }
    void FallIntoHole(AnchorStatus newFallingStatus, SwingableObject faller)
    {
        if (fallingIntoHole == AnchorStatus.NONE)
        {
            fallingIntoHole = newFallingStatus;
            faller.fallingIntoHole = true;
        }
        else if (fallingIntoHole != newFallingStatus)
        {
            faller.fallingIntoHole = true;
        }
    }
    void StopFallingIntoHole()
    {
        fallingIntoHole = AnchorStatus.NONE;
    }
    void OnChainRotate(int direction)
    {
        if (direction == 0 || rotationalVelocity == 0) return;
        if (Mathf.Sign(direction) == Mathf.Sign(rotationalVelocity))
        {
            rotationalVelocity = -rotationalVelocity * speedMultiplierWhenSwitchDir;
            //SwitchAnchor();
        }
    }
    void OnSwingIntoWall(Vector2 hitNormal)
    {
        rotationalVelocity = -rotationalVelocity;
        Vector2 launchVelocity = hitNormal * knockbackWhenHittingWall * currentChainLength;
        Anchor.Launch(launchVelocity);
        Swingee.Launch(launchVelocity);
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
        if (fallingIntoHole != AnchorStatus.NONE)
            DragIntoHole();

        UpdatePivot();

        AccelerateBasedOnInput();

        ExtendChainByInput();

        RotateChain();

        Render();

        PositionHitbox();

        Anchor.velocity *= 1 - (0.5f * Time.deltaTime);
    }

    // void ExtendChainBySpeed()
    // {
    //     if (Mathf.Abs(rotationalVelocity) < 10f || AnchorStatus == AnchorStatus.NONE || currentChainLength >= maxDistance - 0.05f) return;
    //     float pushDistance = Mathf.Abs(rotationalVelocity / 720) * chainExtendRateWhenSwung;
    //     Grabee.MoveTowards(Center, -pushDistance);
    // }

    void ApplyConstraint()
    {
        if (currentChainLength > effectiveMaxDistance)
        {
            float stretchedDistance = (currentChainLength - effectiveMaxDistance) / 2;
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
    void DragIntoHole()
    {
        if (fallingIntoHole == AnchorStatus.NONE) return;

        SwingableObject faller = fallingIntoHole == AnchorStatus.PLAYER ? Player : Rock;
        SwingableObject dragger = fallingIntoHole == AnchorStatus.PLAYER ? Rock : Player;

        dragger.MoveTowards(faller.position, fallingIntoHoleDragSpeed * Time.deltaTime);

        rotationalVelocity = Mathf.MoveTowards(rotationalVelocity, 0, rotationDeceleration * Time.deltaTime);
    }
    void UpdatePivot()
    {
        if (anchorStatus == AnchorStatus.NONE && inputData.chainRotationalInput != 0)
        {
            SetAnchor(AnchorStatus.PLAYER);
        }
        if (fallingIntoHole != AnchorStatus.NONE && fallingIntoHole != anchorStatus)
        {
            SetAnchor(fallingIntoHole);
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
                    acceleration *= 5;
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
        if (fallingIntoHole != AnchorStatus.NONE) return;
        if (inputData.chainExtendInput == 0 || anchorStatus == AnchorStatus.NONE || rotationalVelocity == 0 || Player.velocity.sqrMagnitude > 1f || Anchor.velocity.sqrMagnitude > 1f) return;
        float pushDistance = inputData.chainExtendInput * extendChainSpeed * Time.deltaTime;

        if (currentChainLength + pushDistance > effectiveMaxDistance) pushDistance = Mathf.Min(effectiveMaxDistance - currentChainLength, pushDistance);
        if (currentChainLength + pushDistance < minDistance && Swingee.velocity.sqrMagnitude < 10f) pushDistance = Mathf.Max(minDistance - currentChainLength, pushDistance);

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
        Vector2 pointA = Vector2.MoveTowards(Player.position, Center, 1f);
        Vector2 pointB = Vector2.MoveTowards(Rock.position, Center, 1f);

        edgeCollider.SetPoints(new List<Vector2> { pointA, pointB });
    }
    #endregion

    void SetAnchor(AnchorStatus anchor)
    {
        anchorStatus = anchor;
        Anchor.velocity = Vector2.zero;
    }
    void SwitchAnchor()
    {
        if (anchorStatus == AnchorStatus.PLAYER)
            SetAnchor(AnchorStatus.ROCK);
        else if (anchorStatus == AnchorStatus.ROCK)
            SetAnchor(AnchorStatus.PLAYER);
    }
    public void SwapPlaces()
    {
        SwingableObject toSwap = Rock;
        SwingableObject swapAnchor = Player;

        if (fallingIntoHole != AnchorStatus.NONE)
        {
            SwingableObject faller = fallingIntoHole == AnchorStatus.PLAYER ? Player : Rock;
            SwingableObject dragger = fallingIntoHole == AnchorStatus.PLAYER ? Rock : Player;

            if (dragger.fallingIntoHole && faller.fallingIntoHole) return; // If both are stuck, too bad

            faller.fallingIntoHole = false;
            fallingIntoHole = AnchorStatus.NONE;
            toSwap = faller;
            swapAnchor = dragger;
        }
        else if (Vector2.Dot(toSwap.velocity, swapAnchor.position - toSwap.position) > (swapAnchor.position - toSwap.position).magnitude / 2) return;

        AudioManager.Play("ChainSwap");

        rotationalVelocity = 0;
        toSwap.lastSwapTime = Time.time;


        Vector2 swapTargetPos = GetSwapTargetPosition();

        EnemyMovement aimbotTarget = GetAimbotTarget(swapTargetPos, swapAnchor);
        if (aimbotTarget != null)
        {
            swapTargetPos = (Vector2)aimbotTarget.transform.position;
        }

        toSwap.Launch((swapTargetPos - toSwap.position).normalized * swapPlacesForce * 2);
        lastSwapTime = Time.time;

        if (anchorStatus == AnchorStatus.ROCK)
        {
            SwitchAnchor();
        }
    }

    Vector2 GetSwapTargetPosition()
    {
        SwingableObject toSwap = Rock;
        SwingableObject swapAnchor = Player;

        return swapAnchor.position + (swapAnchor.position - toSwap.position).normalized * maxDistance;
    }
    EnemyMovement GetAimbotTarget(Vector2 swapTargetPosition, SwingableObject swapAnchor)
    {
        EnemyMovement closestEnemy = null;
        float closestDistance = aimbotTargetDistance * aimbotTargetDistance;
        foreach (EnemyMovement enemy in EnemyMovement.EnemyList)
        {
            float distanceToHitTarget = ((Vector2)enemy.transform.position - swapTargetPosition).sqrMagnitude;
            if (distanceToHitTarget < closestDistance)
            {
                if (Vector2.Distance(swapAnchor.position, (Vector2)enemy.transform.position) > maxDistance + chainElongationFromSwap)
                    continue;
                closestDistance = distanceToHitTarget;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    void FixedUpdate()
    {
        UpdateAimbotReticle();
    }

    void UpdateAimbotReticle()
    {
        if (rotationalVelocity == 0)
        {
            if (aimbotReticle.gameObject.activeSelf)
                aimbotReticle.gameObject.SetActive(false);
            return;
        }
        EnemyMovement aimbotTarget = GetAimbotTarget(GetSwapTargetPosition(), Player);
        if (aimbotTarget)
        {
            if (!aimbotReticle.gameObject.activeSelf)
                aimbotReticle.gameObject.SetActive(true);
            aimbotReticle.position = aimbotTarget.transform.position;
        }
        else
        {
            if (aimbotReticle.gameObject.activeSelf)
                aimbotReticle.gameObject.SetActive(false);
        }
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