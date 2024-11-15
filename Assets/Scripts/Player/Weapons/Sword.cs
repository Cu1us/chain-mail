using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon
{
    List<Collider2D> enemiesInsideTrigger = new List<Collider2D>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            FlipCollider(true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            FlipCollider(false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
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

    public void Attack()
    {
        for (int i = enemiesInsideTrigger.Count - 1; i >= 0; i--)
        {
            Destroy(enemiesInsideTrigger[i].gameObject);
        }

        enemiesInsideTrigger.Clear();
    }

    void FlipCollider(bool colliderRight)
    {
        if (colliderRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
    }
}
