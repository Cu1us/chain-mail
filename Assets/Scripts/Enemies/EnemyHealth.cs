using UnityEngine;
using TMPro;
using Unity.Mathematics;
using DG.Tweening;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float enemyHealth;
    [SerializeField] float damageStuckMultiplier;
    [SerializeField] float damageWallBounce;
    [SerializeField] float redTextDamage;
    [SerializeField] float damageTextLifeTime;

    [Header("References")]
    [SerializeField] Sprite dead;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rigidbody;

    [SerializeField] EnemyMovement enemyMovement;
    [SerializeField] Collider2D coll2D;
    [SerializeField] Animator animator;
    [SerializeField] GameObject DamageText;

    GameObject OldText;
    float OldTextTimer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            DestroyEnemy();
        }

        //TimeManager.Slowmo(1000, 0.2f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") && math.abs(rigidbody.velocity.magnitude) > 1)
        {
            TakeDamage(damageWallBounce);
        }
    }

    public void TakeDamage(float damage)
    {
        if (enemyMovement.state == EnemyMovement.EnemyState.STUCK)
        {
            damage *= damageStuckMultiplier;
        }

        enemyMovement.stumbleTimer = enemyMovement.stumbleTimerCooldown - 0.3f;


        damage = Mathf.Round(damage);
        enemyHealth -= damage;

        AudioManager.Play("hurthuman");
        CreateDamageText(damage);

        if (enemyHealth < 0)
        {
            Death();
        }
    }

    void CreateDamageText(float damage)
    {
        if (OldText == null)
        {
            GameObject damageText = Instantiate(DamageText, (Vector2)transform.position + new Vector2(0, -0.5f), Quaternion.identity);
            damageText.GetComponent<SelfDestruct>().targetTransform = transform;
            damageText.GetComponentInChildren<TextMeshPro>().text = damage.ToString();

            damageText.GetComponentInChildren<TextMeshPro>().color = getDamageColor(damage);

            OldText = damageText;
        }
        else
        {
            float newDamage = damage + Convert.ToInt32(OldText.GetComponentInChildren<TextMeshPro>().text);

            OldText.GetComponentInChildren<TextMeshPro>().text = newDamage.ToString();
            OldText.GetComponentInChildren<TextMeshPro>().color = getDamageColor(newDamage);

            OldText.GetComponent<Animator>().Play("DamageText", 0, 0f);

            OldText.gameObject.transform.DOScale(1.2f, 1);

            // Resets timers
            OldText.GetComponent<SelfDestruct>().timer = 0;
            OldTextTimer = 0;

        }
    }

    Color getDamageColor(float damage)
    {
        float gradientValue = Mathf.Clamp01(damage / redTextDamage);

        Color darkRed = new Color(0.5f, 0f, 0f);
        Color damageColor = Color.Lerp(Color.yellow, darkRed, gradientValue);

        return damageColor;
    }

    void ClearOldText()
    {
        OldTextTimer += Time.deltaTime;

        if (OldTextTimer > damageTextLifeTime)
        {
            OldText = null;
        }
    }

    void Death()
    {
        animator.SetBool("isDead", true);
        enemyMovement.isAttackState = false;
        enemyMovement.StateChange(EnemyMovement.EnemyState.STUCK);
        enemyMovement.enabled = false;
        coll2D.enabled = false;
        Invoke(nameof(DestroyEnemy), 3f);
    }

    void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
