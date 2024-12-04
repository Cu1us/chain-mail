using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float enemyHealth;
    SpriteRenderer spriteRenderer;
    EnemyMovement enemyMovement;
    [SerializeField] Sprite dead;
    Collider2D coll2D;
    float damageStuckMultiplier = 2;
    

    void Start()
    {
        coll2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

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
            enemyHealth -= damage * damageStuckMultiplier;
        }
        else 
        {
            enemyHealth -= damage;
        }
        AudioManager.Play("hurthuman");
        if (enemyHealth < 0)
        {
            Death();
        }

    }

    void Death()
    {
        spriteRenderer.sprite = dead;
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
