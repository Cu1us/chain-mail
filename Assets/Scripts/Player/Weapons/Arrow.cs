using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float arrowLifetime;
    [SerializeField] float maxArrowSpeed;
    [SerializeField] float knockbackForce;
    [SerializeField] Transform arrow;

    [Header("References")]
    public float arrowSpeed;

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
            hit.collider.GetComponent<Pathfinding>().CancelAgentUpdate();
            AddKnockback(hit.collider.gameObject);
            this.enabled = false;
        }
    }

    void AddKnockback(GameObject enemy)
    {
        Vector2 forceDirection = enemy.transform.position - transform.position;
        forceDirection.Normalize();
        enemy.GetComponent<Rigidbody2D>().AddForce(forceDirection * knockbackForce, ForceMode2D.Impulse);
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
