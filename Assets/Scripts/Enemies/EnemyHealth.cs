using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float enemyHealth;
    SpriteRenderer spriteRenderer;
    Pathfinding pathfinding;
    [SerializeField] Sprite dead;
    Collider2D collider2D;
    

    void Start()
    {
        collider2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        pathfinding = GetComponent<Pathfinding>();

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            DestroyTemp();
        }
    }


    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        AudioManager.Play("hurthuman");
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
        spriteRenderer.sprite = dead;
        pathfinding.StateChange(Pathfinding.EnemyState.STUCK);
       // spriteRenderer.color = Color.red;
        pathfinding.enabled = false;
        collider2D.enabled=false;
        Invoke(nameof(DestroyTemp), 3f);
    }

    void DestroyTemp()
    {
        Destroy(gameObject);
    }
}
