using UnityEngine;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float enemyHealth;
    [SerializeField] float damageStuckMultiplier = 2;

    [Header("References")]
    [SerializeField] Sprite dead;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] EnemyMovement enemyMovement;
    [SerializeField] Collider2D coll2D;
    [SerializeField] Animator animator;
    [SerializeField] GameObject DamageText;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            DestroyEnemy();
        }
    }

    public void TakeDamage(float damage)
    {
        if (enemyMovement.state == EnemyMovement.EnemyState.STUCK)
        {
            damage *= damageStuckMultiplier;
        }

        enemyHealth -= damage;

        AudioManager.Play("hurthuman");
        GameObject damageText = Instantiate(DamageText, (Vector2)transform.position + new Vector2(0, -0.5f), Quaternion.identity);
        damageText.transform.SetParent(transform);
        damageText.GetComponentInChildren<TextMeshPro>().text = damage.ToString();

        if (enemyHealth < 0)
        {
            Death();
        }
    }

    void Death()
    {
        animator.SetBool("isDead", true);
        enemyMovement.isAttackState = false;
        enemyMovement.StateChange(EnemyMovement.EnemyState.STUCK);
        enemyMovement.enabled = false;
        coll2D.enabled=false;
        Invoke(nameof(DestroyEnemy), 3f);
    }

    void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
