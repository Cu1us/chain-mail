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
    [SerializeField] float damage;

    [Header("References")]
    [SerializeField] Transform arrow;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] GameObject bloodParticle;

    [HideInInspector]public float bowChargePercentage;

    GameObject newParticle;
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.up, arrowSpeed * Time.deltaTime, LayerMask.GetMask("Enemy"));

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
            AddDamage(hit.collider.gameObject);


            newParticle = Instantiate(bloodParticle, transform.position, Quaternion.identity);
            newParticle.transform.SetParent(transform);
            newParticle.transform.localScale = Vector3.one;
            Invoke(nameof(RemoveParticle), 4);

            this.enabled = false;
        }
    }

    void AddKnockback(GameObject enemy)
    {
        enemy.GetComponent<Rigidbody2D>().AddForce(transform.up * knockbackForce, ForceMode2D.Impulse);
    }

    void AddDamage(GameObject enemy)
    {
        enemy.GetComponent<EnemyHealth>().TakeDamage(damage);
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

    void RemoveParticle()
    {
        Destroy(newParticle);
    }
}
