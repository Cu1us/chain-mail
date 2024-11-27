using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Trap : MonoBehaviour
{
    [SerializeField] GameObject Enemy;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            Enemy.GetComponent<EnemyMovement>().StateChange(EnemyMovement.EnemyState.STUCK);
            Enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            Enemy.GetComponent<Transform>().position = (Vector2)other.transform.position + new Vector2(0, 1);
            Enemy.GetComponent<CapsuleCollider2D>().enabled = false;
            Enemy.GetComponent<Animator>().SetBool("Trapfall", true);
        }
    }
}
