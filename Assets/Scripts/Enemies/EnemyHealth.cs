using UnityEngine;
using TMPro;
using Unity.Mathematics;
using DG.Tweening;
using System;
using UnityEditorInternal;

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
    [SerializeField] bool isMale;
    [SerializeField] bool isSentinel;

    GameObject OldText;
    float OldTextTimer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            TakeDamage(1000);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") && math.abs(rigidbody.velocity.magnitude) > 5)
        {
            TakeDamage(damageWallBounce);
            VFX.Spawn(VFXType.CIRCLE_IMPACT, transform.position, 5);
        }
    }

    public void TakeDamage(float damage, bool isSwinging = false)
    {
        if (enemyMovement.state == EnemyMovement.EnemyState.STUCK)
        {
            damage *= damageStuckMultiplier;
        }
        

        enemyMovement.stumbleTimer = enemyMovement.stumbleTimerCooldown - 0.3f;


        if (isSentinel && enemyMovement.state != EnemyMovement.EnemyState.STUCK)
        {
            damage *= 0.5f;
        }
        damage = Mathf.Round(damage);
        enemyHealth -= damage;
        if (isMale)
        {
            AudioManager.Play("hurthuman");
        }
        else
        {
            AudioManager.Play("HurtHumanFemale");
        }
        AudioManager.Play("SwingHitEnemies");
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

            OldText.GetComponent<Animator>().Play("OldDamageText", 0, 0f);
            OldText.GetComponentInChildren<TextMeshPro>().color = getDamageColor(newDamage);
            OldText.gameObject.transform.DOScale(1.4f, 0.1f);

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
        if (isMale)
        {
            AudioManager.Play("MaleDeath");
        }
        else
        {
            AudioManager.Play("FemaleDeath");
        }

        animator.SetBool("isDead", true);
        enemyMovement.StateChange(EnemyMovement.EnemyState.STUCK);
        enemyMovement.isAttackState = false;
        coll2D.enabled = false;
        Invoke(nameof(DestroyEnemy), 2f);
    }

    void DestroyEnemy()
    {
        if (EnemyMovement.EnemyList.Count == 1)
        {
            GameObject nextLevel = GameObject.Find("NextLevel");
            nextLevel.GetComponent<DoorNextLevel>().OpenNextLevel();
        }
        Destroy(gameObject);
    }
}
