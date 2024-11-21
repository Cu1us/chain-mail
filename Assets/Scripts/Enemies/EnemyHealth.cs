using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float enemyHealth;
    SpriteRenderer spriteRenderer;
    Pathfinding pathfinding;
    [SerializeField] Sprite dead;
    

    void Start()
    {
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
        AudioManager.Play("Hurt");
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
       // spriteRenderer.color = Color.red;
        pathfinding.enabled = false;
        Invoke(nameof(Destroy), 3f);
    }

    void DestroyTemp()
    {
        Destroy(this.gameObject);
    }
}
