using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerMovementPlaceHolder : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerInput();
    }
    void PlayerInput()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (vertical != 0 || horizontal != 0)
        {
            MovePlayer(vertical, horizontal);
        }

        if (Input.GetKey(KeyCode.E))
        {
            RotatePlayer(-1);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            RotatePlayer(1);
        }
    }

    void MovePlayer(float vertical, float horizontal)
    {
        Vector2 direction = new Vector2(horizontal, vertical);

        direction = direction.normalized * speed * Time.deltaTime;

        rb.MovePosition((Vector2)rb.transform.position + direction);
    }

    void RotatePlayer(float clokcwise)
    {
        float currentRotation = rb.rotation;
        rb.MoveRotation(currentRotation + clokcwise * rotationSpeed * Time.deltaTime);
    }
}
