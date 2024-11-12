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
        Application.targetFrameRate = 120;
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
    }

    void MovePlayer(float vertical, float horizontal)
    {
        Vector2 direction = new Vector2(horizontal, vertical);

        direction = direction.normalized;

        rb.MovePosition((Vector2)rb.transform.position + direction * speed * Time.deltaTime);
    }
}
