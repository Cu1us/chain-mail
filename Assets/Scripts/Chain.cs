using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Chain : MonoBehaviour
{
    [Header("Status")]
    GrabStatus grabStatus;
    Vector2 pivot;
    Movement grabber
    {
        get
        {
            return grabStatus switch { GrabStatus.A => EntityA, GrabStatus.B => EntityB, _ => null };
        }
    }
    Movement grabbee
    {
        get
        {
            return grabStatus switch { GrabStatus.A => EntityA, GrabStatus.B => EntityB, _ => null };
        }
    }

    [Header("Attachments")]
    public Movement EntityA;
    public Movement EntityB;

    [Header("Settings")]
    [SerializeField] float maxDistance;
    [SerializeField] float rotationSpeed;


    [Header("References")]
    [SerializeField] LineRenderer lineRenderer;


    Vector2 center;
    float distance;

    void Reset()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        distance = Vector2.Distance(EntityA.position, EntityB.position);
        center = (EntityA.position + EntityB.position) / 2;

        ApplyConstraint();

        UpdatePivot();

        if (grabStatus != GrabStatus.NONE)
            RotateChain(grabber.rotationalInput);

        RenderLine();
    }

    void RotateChain(int direction)
    {
        float rotation = direction * rotationSpeed;

    }
    void UpdatePivot()
    {
        if (!EntityA.grabbing && !EntityB.grabbing)
        {
            grabStatus = GrabStatus.NONE;
            pivot = center;
        }
        else if (EntityA.grabbing && !EntityB.grabbing)
        {
            grabStatus = GrabStatus.A;
            pivot = EntityA;
        }
        else if (!EntityA.grabbing && EntityB.grabbing)
        {
            grabStatus = GrabStatus.B;
            pivot = EntityB;
        }
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
        }
    }

    public enum GrabStatus
    {
        NONE,
        A,
        B
    }
}
