using UnityEngine;

public class PlayerBow : MonoBehaviour
{
    [SerializeField] float bowCooldown;
    [SerializeField] float maxBowCharge;
    [SerializeField] GameObject bow;
    [SerializeField] GameObject arrow;

    Vector2 mousePos;
    Vector2 arrowDirection;

    float targetAngle;
    float chargeTimer;
    float bowCharge;

    void Update()
    {
        PlayerInput();
    }

    void PlayerInput()
    {
        if (Input.GetMouseButton(0))
        {
            BowCharge();
        }
        if (Input.GetMouseButtonUp(0))
        {
            ShootArrow();
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
            bow.GetComponent<SpriteRenderer>().color = Color.red;
        }

        if (chargeTimer > maxBowCharge / 3 && chargeTimer < maxBowCharge)
        {
            bow.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    void ShootArrow()
    {
        if (chargeTimer > maxBowCharge / 3)
        {
            bowCharge = (chargeTimer / maxBowCharge);
            chargeTimer = 0;
            bow.GetComponent<SpriteRenderer>().color = Color.gray;

            RotateBow();
            InstantiateArrow();
        }
        else
        {
            chargeTimer = 0;
        }
    }

    void RotateBow()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        arrowDirection = mousePos - (Vector2)bow.transform.position;

        targetAngle = Mathf.Atan2(arrowDirection.y, arrowDirection.x) * Mathf.Rad2Deg;
    }

    void InstantiateArrow()
    {
        GameObject newArrow = Instantiate(arrow, bow.transform.position, Quaternion.Euler(0, 0, targetAngle - 90));
        newArrow.GetComponent<Arrow>().arrowSpeed = bowCharge;
    }
}

