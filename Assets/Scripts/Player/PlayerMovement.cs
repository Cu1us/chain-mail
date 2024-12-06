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
        if (!beingGrabbed && Input.chainRotationalInput == 0 && !beingSwapped) translation += Input.movementInput * movementSpeed * Time.deltaTime;

        if (trailRenderer) trailRenderer.emitting = beingGrabbed;

        base.Update();
    }
}