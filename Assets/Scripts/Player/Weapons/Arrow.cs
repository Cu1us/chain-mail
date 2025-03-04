using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [HideInInspector] public float bowChargePercentage;

    GameObject newParticle;
    RaycastHit2D target;

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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.up, arrowSpeed * Time.deltaTime, LayerMask.GetMask("Player"));

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            target = hit;
            AudioManager.Play("ArrowHit");

            ChangePos();
            DisableEnemyMovement();
            AddKnockback();
            AddDamage(target);
            InstantiateParticle();

            Invoke(nameof(DisableTrail), 0.5f);

            this.enabled = false;
        }
    }

    void ChangePos()
    {
        transform.position = target.point;
        transform.SetParent(target.transform);
    }

    void DisableEnemyMovement()
    {
        if (target.collider.TryGetComponent<Pathfinding>(out Pathfinding pathfinding))
        {
            pathfinding.CancelAgentUpdate();
        }
    }

    void AddKnockback()
    {
        //target.collider.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * knockbackForce, ForceMode2D.Impulse);
    }

    void AddDamage(RaycastHit2D target)
    {
        if (target.transform.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth component))
        {
            target.collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }

    void InstantiateParticle()
    {
        newParticle = Instantiate(bloodParticle, transform.position, Quaternion.identity);
        newParticle.transform.SetParent(transform);
        newParticle.transform.localScale = Vector3.one;
    }

    void DisableTrail()
    {
        trailRenderer.enabled = false;
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
