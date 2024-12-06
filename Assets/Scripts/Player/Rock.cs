using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : SwingableObject
{
    [SerializeField] TrailRenderer trailRenderer;

    protected override void Update()
    {
        if (trailRenderer) trailRenderer.emitting = beingGrabbed;

        base.Update();
    }
}
