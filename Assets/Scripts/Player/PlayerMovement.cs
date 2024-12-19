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
        Vector2 movement = movementSpeed * Time.deltaTime * Input.movementInput;
        if (!beingSwapped && !beingGrabbed && Input.chainRotationalInput == 0)
        {
            translation += movement;
            animator.SetBool("Walking", Input.movementInput != default);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
        animator.SetBool("Flying", beingGrabbed || (beingSwapped && velocity.sqrMagnitude > 1f) || velocity.sqrMagnitude > 20f);
        animator.SetBool("FallingInHole", fallingIntoHole);


        if (trailRenderer)
            trailRenderer.emitting = beingGrabbed;

        base.Update();
    }
}