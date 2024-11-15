using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Sword : MonoBehaviour, IWeapon
{
    [SerializeField] float knockbackForce;
    [SerializeField] float knockbackRotatingForce;

    [SerializeField] BoxCollider2D swordCollider;
    [SerializeField] PolygonCollider2D swordRotateCollider;

    [SerializeField] Transform player1;
    [SerializeField] Transform player2;
    [SerializeField] Transform swordPivot;

    [SerializeField] Chain_Demo chain;

    [SerializeField] Animator animator;

    List<Collider2D> enemiesInsideTrigger = new List<Collider2D>();

    bool ChainRotate;

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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !enemiesInsideTrigger.Contains(collision))
        {
            enemiesInsideTrigger.Add(collision);
        }
        if (ChainRotate)
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
        AddKnockbackAndDamage();
    }

    void AddKnockbackAndDamage()
    {
        for (int i = enemiesInsideTrigger.Count - 1; i >= 0; i--)
        {
            if (!ChainRotate)
            {
                Vector2 forceDirection = enemiesInsideTrigger[i].transform.position - transform.position;
                forceDirection.Normalize();
                enemiesInsideTrigger[i].GetComponent<Rigidbody2D>().AddForce(forceDirection * knockbackForce, ForceMode2D.Impulse);
            }
            if (ChainRotate)
            {
                Vector2 perpendicular = Vector2.Perpendicular(player1.position - player2.position);

                Vector2 forceDirection = enemiesInsideTrigger[i].transform.position - transform.position;

                Vector2 angle = perpendicular + forceDirection;

                angle *= Mathf.Sign(chain.rotationalVelocity);

                angle.Normalize();

                enemiesInsideTrigger[i].GetComponent<Rigidbody2D>().AddForce(angle * knockbackForce, ForceMode2D.Impulse);
            }

            //Destroy(enemiesInsideTrigger[i].gameObject);
        }
    }

    void FlipColliderDirection(bool colliderRight)
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

    void ChainRotation()
    {
        if (chain.grabStatus == Chain_Demo.GrabStatus.B)
        {
            swordCollider.enabled = true;
            swordRotateCollider.enabled = false;

            Vector2 ChainDirection = player1.transform.position - player2.transform.position;
            ChainDirection.Normalize();
            float angleDegrees = (Mathf.Atan2(ChainDirection.y, ChainDirection.x) * Mathf.Rad2Deg);
            swordPivot.transform.rotation = Quaternion.Euler(0, 0, angleDegrees);

            ChainRotate = true;
        }
        else
        {
            ChainRotate = false;
            swordCollider.enabled = false;
            swordRotateCollider.enabled = true;
        }
    }
}
