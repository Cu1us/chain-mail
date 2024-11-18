using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float arrowLifetime;
    [SerializeField] float maxArrowSpeed;
    [SerializeField] Transform arrow;

    [Header("References")]
    public float arrowSpeed;

    bool move = true;
    float arrowTimer;

    void Start()
    {
        arrowSpeed *= maxArrowSpeed;
    }
    void Update()
    {
        Move();
        CheckCollision();
        SelfDestruct();
    }

    void Move()
    {
        transform.Translate(Vector3.up * arrowSpeed * Time.deltaTime);
    }

    void CheckCollision()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.up, arrowSpeed * Time.deltaTime);

        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            transform.position = hit.point;
            transform.SetParent(hit.transform);
            this.enabled = false;
        }
    }

    void SelfDestruct()
    {
        arrowTimer += Time.deltaTime;
        if (arrowTimer > arrowLifetime)
        {
            Destroy(gameObject);
        }
    }
}
