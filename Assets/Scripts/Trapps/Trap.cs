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
            other.GetComponent<EnemyMovement>().StateChange(EnemyMovement.EnemyState.STUCK);
            other.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            other.GetComponent<Transform>().position = new Vector2(other.GetComponent<Transform>().position.x , transform.position.y);
            other.GetComponent<CapsuleCollider2D>().enabled = false;
            other.GetComponent<Animator>().SetBool("Trapfall", true);
        }
    }
}
