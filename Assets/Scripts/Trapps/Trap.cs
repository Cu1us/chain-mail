using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class Trap : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyMovement>().StateChange(EnemyMovement.EnemyState.DEAD);
            other.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            other.GetComponent<Transform>().DOMove((Vector2)transform.position + new Vector2(0, 0.2f), 0.2f);
            other.GetComponent<CapsuleCollider2D>().enabled = false;
            other.GetComponent<Animator>().SetBool("Trapfall", true);
        }
    }
}
