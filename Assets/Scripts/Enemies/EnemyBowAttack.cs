using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyBowAttack : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float bowCharge;
    [SerializeField] float shootInterval;

    [Header("References")]
    [SerializeField] Arrow arrowPrefab;
    [SerializeField] EnemyMovement enemyMovement;

    float shootTimer;
    float randomInterval;

    void Start()
    {
        randomInterval = shootInterval;    
    }

    void Update()
    {
        ShootArrow();
    }

    void ShootArrow()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer > randomInterval && enemyMovement.isAttackState)
        {
            InstantiateArrow();
            shootTimer = 0;
            randomInterval = Random.Range(shootInterval - 1, shootInterval + 1);
        }
    }

    void InstantiateArrow()
    {
        Vector2 targetDirection = transform.position - enemyMovement.targetTransform1.transform.position;
        targetDirection.Normalize();

        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        Arrow newArrow = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0, 0, targetAngle + 90));
        newArrow.bowChargePercentage = bowCharge;
    }
}
