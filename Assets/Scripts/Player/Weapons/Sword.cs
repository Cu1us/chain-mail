using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon
{
    [Header("Force Settings")]
    [SerializeField] float knockbackForce;
    [SerializeField] float knockbackRotatingForce;

    [Header("References")]
    [SerializeField] PolygonCollider2D swordCollider;
    [SerializeField] BoxCollider2D swordRotateCollider;

    [SerializeField] Transform player1;
    [SerializeField] Transform player2;
    [SerializeField] Transform swordPivot;

    [SerializeField] Chain_Demo chain;
    [SerializeField] Animator animator;

    List<Collider2D> enemiesInsideTrigger = new List<Collider2D>();

    bool ChainRotating;

    void Start()
    {
        Application.targetFrameRate = 160; // Should later be removed!
    }

    void Update()
    {
        ChainRotation();

        if (Input.GetKeyDown(KeyCode.D))
        {
            FlipColliderDirection(true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            FlipColliderDirection(false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }

    void ChainRotation()
    {
        if (chain.grabStatus == Chain_Demo.GrabStatus.B)
        {
            ChainRotating = true;
            swordCollider.enabled = false;
            swordRotateCollider.enabled = true;

            Vector2 ChainDirection = player1.transform.position - player2.transform.position;
            ChainDirection.Normalize();
            float angleDegrees = (Mathf.Atan2(ChainDirection.y, ChainDirection.x) * Mathf.Rad2Deg);
            swordPivot.transform.rotation = Quaternion.Euler(0, 0, angleDegrees);
        }
        else
        {
            ChainRotating = false;
            swordCollider.enabled = true;
            swordRotateCollider.enabled = false;
        }
    }

    void FlipColliderDirection(bool colliderRight) // TODO: The swordcollider is not in the right angle after the player have been rotated
    {
        if (colliderRight)
        {
            swordPivot.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            swordPivot.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !enemiesInsideTrigger.Contains(collision))
        {
            enemiesInsideTrigger.Add(collision);
        }
        if (ChainRotating)
        {
            Attack();
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
        animator.SetTrigger("PlayAttack");
        AddKnockback();
        AddDamage();
    }

    void AddKnockback()
    {
        for (int i = enemiesInsideTrigger.Count - 1; i >= 0; i--)
        {
            if (!ChainRotating)
            {
                Vector2 forceDirection = enemiesInsideTrigger[i].transform.position - transform.position;
                forceDirection.Normalize();
                enemiesInsideTrigger[i].GetComponent<Rigidbody2D>().AddForce(forceDirection * knockbackForce, ForceMode2D.Impulse);
            }
            if (ChainRotating)
            {
                Vector2 perpendicular = Vector2.Perpendicular(player1.position - player2.position);

                Vector2 enemyDirection = enemiesInsideTrigger[i].transform.position - transform.position;

                Vector2 forceDirection = perpendicular + enemyDirection; //TODO: Adjust force direction  //TODO: Adding knockback to multiple enemies cause increase in knockback

                forceDirection *= Mathf.Sign(chain.rotationalVelocity);

                forceDirection.Normalize();

                enemiesInsideTrigger[i].GetComponent<Rigidbody2D>().AddForce(forceDirection * knockbackForce * knockbackRotatingForce, ForceMode2D.Impulse);
            }
        }
    }

    void AddDamage()
    {
        //Destroy(enemiesInsideTrigger[i].gameObject);
    }
}
