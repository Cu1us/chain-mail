using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour, IWeapon
{
    [Header("Settings")]
    [SerializeField] float maxBowCharge;

    [Header("References")]
    [SerializeField] GameObject arrowPrefab;

    Vector2 mousePos;
    Vector2 arrowDirection;

    float targetAngle;
    float chargeTimer;
    float bowCharge;

    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            BowCharge();
        }
        if (Input.GetMouseButtonUp(0))
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (chargeTimer > maxBowCharge / 3)
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

    void BowCharge()
    {
        if (chargeTimer < maxBowCharge)
        {
            chargeTimer += Time.deltaTime;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }

        if (chargeTimer > maxBowCharge / 3 && chargeTimer < maxBowCharge)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
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
        GameObject newArrow = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0, 0, targetAngle - 90));
        newArrow.GetComponent<Arrow>().arrowSpeed = bowCharge;
    }
}
