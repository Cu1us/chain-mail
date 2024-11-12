using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float bowCooldown;
    public GameObject arrow;
    public GameObject bow;
    Vector2 mousePos;
    Vector2 direction;

    float targetAngle;
    float bowTimer;

    void Update()
    {
        bowTimer += Time.deltaTime;
        PlayerInput();
    }

    void PlayerInput()
    {
        if (Input.GetMouseButton(0))
        {
            RotateBow();
            ShootArrow();
        }
    }

    void RotateBow()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        direction = mousePos - (Vector2)transform.position;

        targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    void ShootArrow()
    {
        if (bowTimer > bowCooldown)
        {
            GameObject newArrow = Instantiate(arrow, bow.transform.position, Quaternion.Euler(0, 0, targetAngle - 90));

            bowTimer = 0;
        }
    }
}

