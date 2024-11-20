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
    [SerializeField] float maxKnockbackForce;

    [Header("References")]
    [SerializeField] Transform arrow;
    [SerializeField] TrailRenderer trailRenderer;
    [HideInInspector]public float bowChargePercentage;

    float knockbackForce;
    float arrowSpeed;
    float arrowTimer;

    void Start()
    {
        arrowSpeed = bowChargePercentage * maxArrowSpeed;
        knockbackForce = maxKnockbackForce * bowChargePercentage;
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

            Invoke(nameof(DisableTrail), 0.5f);
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

    void DisableTrail()
    {
        trailRenderer.enabled = false;
    }
}
