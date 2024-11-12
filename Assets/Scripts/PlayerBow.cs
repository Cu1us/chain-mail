using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float bowCooldown;
    [SerializeField] float maxBowCharge;
    public GameObject arrow;
    public GameObject bow;

    Vector2 mousePos;
    Vector2 direction;

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
        if (Input.GetMouseButtonUp(0))
        {
            ShootArrow();
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

        direction = mousePos - (Vector2)bow.transform.position;

        targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    void InstantiateArrow()
    {
        GameObject newArrow = Instantiate(arrow, bow.transform.position, Quaternion.Euler(0, 0, targetAngle - 90));
        newArrow.GetComponent<Arrow>().arrowSpeed = bowCharge;
    }
}

