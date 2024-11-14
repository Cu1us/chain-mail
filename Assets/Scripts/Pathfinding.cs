using UnityEngine;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour
{
    [Header("References")]
    NavMeshAgent agent;
    [SerializeField] Transform player1;
    [SerializeField] Transform player2;
    SpriteRenderer spriteRenderer;

    [Header("Speeds")]

    float chargeSpeed;
    float walkSpeed;
    float normalSpeed;

    [Header("Stopping Distance")]
    float stoppingDistance;
    float chargeStoppingDistance;

    public enum EnemyState
    {
        ATTACK, CHARGE, IDLE, APPROACH, FLEE, SIDESTRIFE

    }


    [Header("Distance")]
    [SerializeField] float minDistanceToPlayer; //triggers the enemy to move out of range
    Transform targetTransform;


    Vector2 target;
    public bool targetInRange;

    EnemyState state;

    [Header("Timer")]
    float stateTimer;
    [SerializeField] float stumbleCooldownTime;
    float stumbleTimer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player1 = GameObject.Find("Player1").transform;
        player2 = GameObject.Find("Player2").transform;



    }

    void Update()
    {
        stumbleTimer += Time.deltaTime;
        stateTimer -= Time.deltaTime;
        if(stateTimer <= 0)
        {
            State();
        }
        ComparePlayerDistance();

        //target = targetTransform.position; in approach


        switch (state)
        {
            case EnemyState.IDLE:
                break;
            case EnemyState.APPROACH:
                break;
            case EnemyState.SIDESTRIFE:
                break;

             case EnemyState.ATTACK:
                break;    

            case EnemyState.CHARGE:
                break;

            case EnemyState.FLEE:
                break;

        }

        MinimumDistance();

        agent.SetDestination(target);
        Flip();
    }



    void State()
    {
        if (targetInRange)
        {
            int random = Random.Range(0, 101);

            if (random < 30)
            {
                state = EnemyState.CHARGE;
            }
            else if (random >= 30 && random < 60)
            {
                state = EnemyState.SIDESTRIFE;
            }
            else if (random >= 60)
            {
                state = EnemyState.FLEE;
            }
            else
            {
                state = EnemyState.ATTACK;
            }
        }
        else //if(target detected)
        {
            state = EnemyState.APPROACH;
        }
        //else
        // {
        //     state = EnemyState.IDLE;
        // }
    }


    void ComparePlayerDistance()
    {
        float distPlayer1 = Vector2.Distance(player1.position, transform.position);
        float distPlayer2 = Vector2.Distance(player2.position, transform.position);
        if (distPlayer1 < distPlayer2)
        {
            targetTransform = player1;
        }
        else
        {
            targetTransform = player2;
        }

    }

    void MinimumDistance()
    {
        float dist = Vector2.Distance(targetTransform.position, transform.position);

        if (dist < minDistanceToPlayer)
        {
            Vector2 pos = new Vector2(targetTransform.position.x - transform.position.x, targetTransform.position.y - transform.position.y);
            pos.Normalize();

            target = new Vector2(targetTransform.position.x - pos.x * agent.stoppingDistance * 2, targetTransform.position.y - pos.y * agent.stoppingDistance * 2);
        }
    }

    void Idle()
    {
        agent.SetDestination(transform.position);
    }

    void Approach()
    {
        target = targetTransform.position;
    }
    void SideStrife()
    {

    }

    void Charge()
    {
        target = targetTransform.position;
    }

    void Attack()
    {
        target = targetTransform.position;
    }

    void Flee()
    {
       // target = 
    }

    void Flip()
    {
        float x = targetTransform.position.x - transform.position.x;
        if (x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    public void Stumble()
    {
        if (stumbleCooldownTime < stumbleTimer)
        {
            agent.speed = 0f;
            spriteRenderer.color = Color.cyan;
            Invoke(nameof(Stand), 2);
        }
        //play animation
    }

    void Stand()
    {
        spriteRenderer.color = Color.white;
        agent.speed = 3.5f;
        stumbleTimer = 0;
    }


}
