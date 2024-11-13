using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerSword : MonoBehaviour
{
    public Animator animator;
    public Transform parent;
    public GameObject colliderPivot;
    public GameObject sword;

    GameObject closestEnemie;
    float closestDistance = 500;

    List<Collider2D> enemiesInTrigger = new List<Collider2D>();
    List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !enemiesInTrigger.Contains(collision))
        {
            enemiesInTrigger.Add(collision);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemiesInTrigger.Remove(collision);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("SwordAnimation", true);

            for (int i = 0; i < enemiesInTrigger.Count; i++)
            {
                Destroy(enemiesInTrigger[i].gameObject);
                enemiesInTrigger.Clear();
            }
        }
        else
        {
            animator.SetBool("SwordAnimation", false);
        }

        RotateToEnemy();
        ClosestEnemy();
    }

    void RotateToEnemy()
    {
        if (closestDistance <= 3f && closestEnemie != null)
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
                Vector2 distance = enemy.transform.position - transform.position;
                float enemyDistance = Vector2.SqrMagnitude(distance);

                if (enemyDistance < closestDistance)
                {
                    closestEnemie = enemy;
                    closestDistance = enemyDistance;
                }
            }
        }
    }
}
