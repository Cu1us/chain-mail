using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : SwingableObject
{
    [SerializeField] float movementSpeed;

    [SerializeField] PlayerInputData Input;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] Animator animator;


    void Reset()
    {
        Input = GetComponent<PlayerInputData>();
    }

    protected override void Update()
    {
        animator.SetBool("Walking", beingGrabbed);
        animator.SetFloat("SwingSpeed", Mathf.Abs(swingVelocity) / 30);
        animator.SetBool("Flying", beingGrabbed && Mathf.Abs(swingVelocity) > 65 || (beingSwapped && velocity.sqrMagnitude > 1f) || velocity.sqrMagnitude > 20f);
        animator.SetBool("FallingInHole", fallingIntoHole);


        if (trailRenderer)
            trailRenderer.emitting = beingGrabbed;

        base.Update();

        if (spriteRenderer)
        {
            spriteRenderer.flipX = facingRight;
        }
    }
}