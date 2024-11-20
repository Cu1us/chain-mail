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

    Vector2 mousePos;
    Vector2 arrowDirection;

    float targetAngle;
    float chargeTimer;
    float bowCharge;

    bool Attackpress = false;

    void Update()
    {
        ChargeBow();
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

            RotateArrow();
            InstantiateArrow();
        }
        else
        {
            chargeTimer = 0;
        }
    }

    void RotateArrow()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        arrowDirection = mousePos - (Vector2)transform.position;

        targetAngle = Mathf.Atan2(arrowDirection.y, arrowDirection.x) * Mathf.Rad2Deg;
    }

    void InstantiateArrow()
    {
        Arrow newArrow = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0, 0, targetAngle - 90));
        newArrow.bowChargePercentage = bowCharge;
    }
}
