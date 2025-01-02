using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : SwingableObject
{
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] ParticleSystem smokeTrail;
    [SerializeField] Animator animator;

    Vector2 lastPosition;

    protected override void Update()
    {
        if (trailRenderer) trailRenderer.emitting = beingGrabbed;
        if (smokeTrail)
        {
            ParticleSystem.EmissionModule emission = smokeTrail.emission;
            emission.enabled = beingSwapped;
        }

        float moveSpeed = (lastPosition - (Vector2)transform.position).magnitude / Time.deltaTime;
        animator.SetBool("Moving", moveSpeed > 0.5f);
        animator.SetBool("Flying", beingGrabbed || (beingSwapped && velocity.sqrMagnitude > 1f) || velocity.sqrMagnitude > 20f);

        base.Update();
        lastPosition = transform.position;

        if (spriteRenderer)
        {
            //spriteRenderer.flipX = !facingRight;
        }
    }
}
