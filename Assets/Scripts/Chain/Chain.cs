using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    [Header("Status")]
    [ReadOnlyInspector] public GrabStatus grabStatus;

    [Header("Settings")]
    [SerializeField] float maxDistance;
    [SerializeField] float rotationSpeed;

    [Header("Advanced settings")]
    [SerializeField] float heldPivotOffset;

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

    void Update()
    {
        CalculateChainLength();
        ApplyConstraint();
        UpdatePivot();
        RotateBasedOnInput();
        PositionHitbox();
        Render();
    }
    void CalculateChainLength()
    {
        currentChainLength = Vector2.Distance(PlayerA.position, PlayerB.position);
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

            CalculateChainLength();
        }
    }
    void UpdatePivot()
    {
        if (!PlayerAInput.isGrabbingChain && !PlayerBInput.isGrabbingChain)
        {
            SetGrabStatus(GrabStatus.NONE);
        }
    }
    void RotateBasedOnInput()
    {
        if (grabStatus == GrabStatus.NONE || GrabberInput.movementInput == Vector2.zero) return;
        Vector2 targetDir = GrabberInput.movementInput.normalized;
        Vector2 currentDir = (Grabee.position - Pivot).normalized;
        float angle = Vector2.SignedAngle(currentDir, targetDir);
        float rotation = rotationSpeed * Time.deltaTime * Mathf.Sign(angle);

        PlayerA.RotateAround(Pivot, rotation);
        PlayerB.RotateAround(Pivot, rotation);
    }
    void SetGrabStatus(GrabStatus newStatus)
    {
        grabStatus = newStatus;
        switch (newStatus)
        {
            case GrabStatus.NONE:
                localPivot = 0.5f;
                break;
            case GrabStatus.A:
                localPivot = heldPivotOffset;
                break;
            case GrabStatus.B:
                localPivot = 1 - heldPivotOffset;
                break;
        }
    }
    void PositionHitbox()
    {
        Vector2 pointA = Vector2.MoveTowards(PlayerA.position, Center, 0.5f);
        Vector2 pointB = Vector2.MoveTowards(PlayerB.position, Center, 0.5f);

        edgeCollider.SetPoints(new List<Vector2> { pointA, pointB });
    }
    void Render()
    {
        lineRenderer.SetPosition(0, PlayerA.position);
        lineRenderer.SetPosition(1, PlayerB.position);
    }

    void Start()
    {
        PlayerAInput.onChainGrab += OnPlayerAGrab;
        PlayerBInput.onChainGrab += OnPlayerBGrab;
    }
    void OnPlayerAGrab()
    {
        SetGrabStatus(GrabStatus.A);
    }
    void OnPlayerBGrab()
    {
        SetGrabStatus(GrabStatus.B);
    }
    void Reset()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        lineRenderer.positionCount = 2;
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
