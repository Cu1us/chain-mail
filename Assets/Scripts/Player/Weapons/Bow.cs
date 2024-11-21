using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    [Header("Settings")]
    [SerializeField] float maxBowCharge;
    [SerializeField] float lowestAllowedBowCharge;

    [Header("References")]
    [SerializeField] Arrow arrowPrefab;
    [SerializeField] PlayerInputData playerInputData;
    [SerializeField] Transform playerTransform;
    [SerializeField] SpriteRenderer spriteRenderer;

    Vector2 mousePos;
    Vector2 arrowDirection;

    float targetAngle;
    float chargeTimer;
    float bowCharge;

    bool Attackpress = false;

    void Update()
    {
        ChargeBow();
        FlipSprite();
    }

    public override void AttackPress()
    {
        Attackpress = true;
    }

    void ChargeBow()
    {
        if (Attackpress)
        {
            if (chargeTimer < maxBowCharge)
            {
                chargeTimer += Time.deltaTime;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.red;
            }

            if (chargeTimer > maxBowCharge * lowestAllowedBowCharge && chargeTimer < maxBowCharge)
            {
                GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    public override void AttackRelease()
    {
        Attackpress = false;

        if (chargeTimer > maxBowCharge * lowestAllowedBowCharge)
        {
            bowCharge = (chargeTimer / maxBowCharge);
            chargeTimer = 0;
            GetComponent<SpriteRenderer>().color = Color.gray;

            InstantiateArrow();
        }
        else
        {
            chargeTimer = 0;
        }
    }

    void InstantiateArrow()
    {
        targetAngle = Mathf.Atan2(playerInputData.aimDirection.y, playerInputData.aimDirection.x) * Mathf.Rad2Deg;
        Arrow newArrow = Instantiate(arrowPrefab, playerTransform.position, Quaternion.Euler(0, 0, targetAngle - 90));
        newArrow.bowChargePercentage = bowCharge;
    }

    void FlipSprite()
    {
        if (playerInputData.movementInput.x > 0.3f)
        {
            spriteRenderer.flipX = true;
        }
        else if (playerInputData.movementInput.x < -0.3f)
        {
            spriteRenderer.flipX = false;
        }
    }
}
