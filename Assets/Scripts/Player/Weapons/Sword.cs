using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    List<Collider2D> enemiesInsideTrigger = new List<Collider2D>();
    List<GameObject> enemies = new List<GameObject>();

    void Update()
    {
        Attack(); // Should be called from another script
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

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = enemiesInsideTrigger.Count - 1; i >= 0; i--)
            {
                Destroy(enemiesInsideTrigger[i].gameObject);
            }

            enemiesInsideTrigger.Clear();
        }
    }
}
