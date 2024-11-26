using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Trap : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("EnemyFeet"))
        {
            other.GetComponentInParent<Transform>().transform.position = transform.position;
            other.GetComponentInParent<Pathfinding>().CancelAgentUpdate();
            other.GetComponentInParent<Rigidbody2D>().velocity = new Vector2(0,0);
            other.GetComponentInParent<EnemyHealth>().TakeDamage(200);
        }

        /*else if(other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(20);
        }*/
    }
}
