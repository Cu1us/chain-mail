using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Trap : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.transform.position = transform.position;
            other.GetComponent<Pathfinding>().CancelAgentUpdate();
            other.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            other.GetComponent<EnemyHealth>().TakeDamage(200);
        }

        /*else if(other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(20);
        }*/
    }
}
