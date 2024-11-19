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


    [Header("Settings")]



    [Header("Players")]
    public PlayerMovement PlayerA;
    public PlayerMovement PlayerB;

    [Header("References")]
    [ReadOnlyInspector][SerializeField] LineRenderer lineRenderer;
    [ReadOnlyInspector][SerializeField] EdgeCollider2D edgeCollider;

    // Properties
    public Vector2 Pivot { get { return Vector2.Lerp(PlayerA.position, PlayerB.position, localPivot); } }
    public PlayerMovement Grabber { get { return grabStatus switch { GrabStatus.A => PlayerB, GrabStatus.B => PlayerA, _ => null }; } }
    public PlayerMovement Grabee { get { return grabStatus switch { GrabStatus.A => PlayerB, GrabStatus.B => PlayerA, _ => null }; } }

    // Local variables
    float localPivot;



    void Reset()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
    }



    public enum GrabStatus
    {
        A,
        B,
        NONE
    }
}
