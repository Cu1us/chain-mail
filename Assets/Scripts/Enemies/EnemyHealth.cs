using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float enemyHealth;
    SpriteRenderer spriteRenderer;
    Pathfinding pathfinding;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        pathfinding = GetComponent<Pathfinding>();
    }


    void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        if (enemyHealth < 0)
        {
            Death();
        }
        else if (enemyHealth < 30)
        {
            if (Random.Range(0, 2) < 1)
            {
                pathfinding.StateChange(Pathfinding.EnemyState.FLEE);
            }
        }

    }

    void Death()
    {
        spriteRenderer.color = Color.red;
        pathfinding.enabled = false;
        Invoke(nameof(Destroy), 3f);
    }
}
