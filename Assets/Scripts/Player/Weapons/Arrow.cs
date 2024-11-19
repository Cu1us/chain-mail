using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Arrow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float arrowLifetime;
    [SerializeField] float maxArrowSpeed;
    [SerializeField] float knockbackForce;
    [SerializeField] Transform arrow;

    [Header("References")]
    [HideInInspector]public float bowChargePercentage;

    float arrowSpeed;
    float arrowTimer;

    void Start()
    {
        arrowSpeed = bowChargePercentage * maxArrowSpeed;
        knockbackForce *= bowChargePercentage;
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

            if (hit.collider.TryGetComponent<Pathfinding>(out Pathfinding pathfinding))
            {
                pathfinding.CancelAgentUpdate();
            }

            AddKnockback(hit.collider.gameObject);
            this.enabled = false;
        }
    }

    void AddKnockback(GameObject enemy)
    {
        enemy.GetComponent<Rigidbody2D>().AddForce(transform.up * knockbackForce, ForceMode2D.Impulse);
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
