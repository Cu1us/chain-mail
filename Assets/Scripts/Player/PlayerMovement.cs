using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : SwingableObject
{
    [SerializeField] float movementSpeed;

    [SerializeField] PlayerInputData Input;
    [SerializeField] TrailRenderer trailRenderer;


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
        }

        if (trailRenderer)
            trailRenderer.emitting = beingGrabbed;

        base.Update();
    }
}