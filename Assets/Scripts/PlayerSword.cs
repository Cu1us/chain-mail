using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerSword : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] Transform parent;
    [SerializeField] GameObject colliderPivot;
    [SerializeField] GameObject sword;

    GameObject closestEnemie;
    float closestDistance = Mathf.Infinity;

    List<Collider2D> enemiesInsideTrigger = new List<Collider2D>();
    List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    void Update()
    {
        HandleSword();
        RotateToEnemy();
        ClosestEnemy();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !enemiesInsideTrigger.Contains(collision))
        {
            enemiesInsideTrigger.Add(collision);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemiesInsideTrigger.Remove(collision);
        }
    }

    void HandleSword()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("SwordAnimation", true);

            for (int i = enemiesInsideTrigger.Count - 1; i >= 0; i--)
            {
                Destroy(enemiesInsideTrigger[i].gameObject);
            }

            enemiesInsideTrigger.Clear();
        }
        else
        {
            animator.SetBool("SwordAnimation", false);
        }
    }

    void RotateToEnemy()
    {
        if (closestDistance <= 5f && closestEnemie != null)
        {
            Vector2 direction = (Vector2)closestEnemie.transform.position - (Vector2)colliderPivot.transform.position;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            colliderPivot.transform.rotation = Quaternion.Euler(0, 0, targetAngle - 90);
        }
    }

    void ClosestEnemy()
    {
        closestEnemie = null;
        closestDistance = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                float enemyDistance = Vector2.Distance(enemy.transform.position, transform.position);

                if (enemyDistance < closestDistance)
                {
                    closestEnemie = enemy;
                    closestDistance = enemyDistance;
                }
            }
        }
    }
}
