using UnityEngine;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour
{
    [Header("References")]
    NavMeshAgent agent;
    Transform player1;
    Transform player2;
    Transform targetTransform;
    Transform targetTransform2;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;


    [Header("Speeds")]

    [SerializeField] float chargeMovementSpeed;
    [SerializeField] float walkMovementSpeed;
    [SerializeField] float normalMovementSpeed;

    [Header("Stopping Distance")]
    float stoppingDistanceOutsideAttackRange = 3;

    [SerializeField] float flankExtraDistance = 1;

    public enum EnemyState
    {
        ATTACK, CHARGE, IDLE, APPROACH, FLEE, SIDESTRIFE, FLANK, STUCK

    }
    EnemyState state;

    private enum StrifeDir
    {
        LEFT, RIGHT
    }
    StrifeDir strifeDir;

    [Header("Distance")]
    [SerializeField] float minDistanceToPlayer; //triggers the enemy to move out of range



    Vector2 target;
    [Header("Bool")]
    public bool playerDetected = true;
    public bool attackState;
    [SerializeField] bool isSpearMan = true;
    


    [Header("Timer")]
    float stateTimer = 1; //sett till 0 senare för att köra runtime
    [SerializeField] float stumbleCooldownTime;
    float stumbleTimer;





    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player1 = GameObject.Find("Player1").transform;
        player2 = GameObject.Find("Player2").transform;

    }

    void Update()
    {
        stumbleTimer += Time.deltaTime;
        stateTimer -= Time.deltaTime;   //använd senare. just nu sett state för att testa olika states.
        if (rb.velocity.sqrMagnitude < 0.1 && agent.updatePosition == false && state != EnemyState.STUCK)
        {
            AgentUpdate();
        }
       
        if (stateTimer <= 0)
        {
            RandomState();
        }
        ComparePlayerDistance();

        if (Input.GetKeyDown(KeyCode.Tab)) //debug funktion for switching state
        {
            RandomState();
        }

        switch (state)
        {
            case EnemyState.IDLE:
                Idle();
                break;
            case EnemyState.APPROACH:
                Approach();
                break;
            case EnemyState.SIDESTRIFE:
                SideStrife();
                break;

            case EnemyState.ATTACK:
                Attack();
                break;

            case EnemyState.CHARGE:
                break;

            case EnemyState.FLANK:
                Flank();
                break;

            case EnemyState.FLEE:
                break;

        }

        MinimumDistance();
        
         agent.SetDestination(target);
       
        Flip();
        
    }



    public void RandomState()
    {
        if (playerDetected)
        {
            int random = Random.Range(0, 12);

            if (random <= 2)
            {
                state = EnemyState.APPROACH;
            }
            else if (random > 2 && random <= 4)
            {
                state = EnemyState.SIDESTRIFE;
            }
            else if (random > 4 && random <= 6)
            {
                state = EnemyState.FLANK;
            }
            else if (random > 6 && random <= 8)
            {
                state = EnemyState.ATTACK;
            }
            else if (random > 8 && random <= 10 && isSpearMan)
            {
                state = EnemyState.CHARGE;
            }
            else
            {
                state = EnemyState.FLEE;
            }

        }
        else
        {
            state = EnemyState.IDLE;
        }

        StateChange(state);
    }

    public void StateChange(EnemyState _state)
    {
        stateTimer = 5;
        state = _state;
        Debug.Log(state);
        switch (state)
        {
            case EnemyState.ATTACK:
                attackState = true;
                agent.speed = chargeMovementSpeed;
                minDistanceToPlayer = 0;
                agent.stoppingDistance = 1.5f;
                break;


            case EnemyState.CHARGE:
                attackState = false;
                minDistanceToPlayer = 0;
                agent.stoppingDistance = 0;
                agent.speed = chargeMovementSpeed;
                Charge();
                break;


            case EnemyState.IDLE:
                attackState = true;
                minDistanceToPlayer = 0;
                agent.stoppingDistance = 0;
                agent.speed = walkMovementSpeed;
                break;


            case EnemyState.APPROACH:
                attackState = true;
                minDistanceToPlayer = 2;
                agent.stoppingDistance = stoppingDistanceOutsideAttackRange;
                agent.speed = normalMovementSpeed;
                break;


            case EnemyState.FLEE:
                attackState = false;
                agent.stoppingDistance = 0;
                agent.speed = normalMovementSpeed;
                minDistanceToPlayer = 0;
                Flee();
                break;


            case EnemyState.SIDESTRIFE:
                attackState = true;
                if (Random.Range(0, 2) == 0)
                {
                    strifeDir = StrifeDir.LEFT;
                }
                else
                {
                    strifeDir = StrifeDir.RIGHT;
                }
                agent.speed = walkMovementSpeed;
                agent.stoppingDistance = 0;
                minDistanceToPlayer = 0;
                break;


            case EnemyState.FLANK:
                attackState = true;
                minDistanceToPlayer = 2;
                agent.stoppingDistance = 0;
                agent.speed = normalMovementSpeed;

                break;
        }
        Debug.Log(state);
    }


    void ComparePlayerDistance()
    {
        float distPlayer1 = Vector2.Distance(player1.position, transform.position);
        float distPlayer2 = Vector2.Distance(player2.position, transform.position);
        if (distPlayer1 < distPlayer2)
        {
            targetTransform = player1;
            targetTransform2 = player2;
        }
        else
        {
            targetTransform = player2;
            targetTransform2 = player1;
        }
    }

    void MinimumDistance()
    {
        //Keep a minimum distance to the closest target.

        float dist = Vector2.Distance(targetTransform.position, transform.position);

        if (dist < minDistanceToPlayer)
        {
            Vector2 pos = new Vector2(targetTransform.position.x - transform.position.x, targetTransform.position.y - transform.position.y);
            pos.Normalize();
            target = new Vector2(targetTransform.position.x - pos.x * 10, targetTransform.position.y - pos.y * 10);
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
        Vector2 perpendicular = Vector2.Perpendicular(transform.position - targetTransform.position);
        if (strifeDir == StrifeDir.LEFT)
        {
            target = new Vector2(transform.position.x - perpendicular.x, transform.position.y - perpendicular.y);
        }
        else
        {
            target = new Vector2(transform.position.x + perpendicular.x, transform.position.y + perpendicular.y);
        }

    }

    void Charge()
    {
        Vector2 dir = targetTransform.position - transform.position;
        dir.Normalize();
        target = new Vector2(targetTransform.position.x + dir.x * 3, targetTransform.position.y + dir.y * 3);
    }

    void Flank()
    {
        Vector2 perpendicular = Vector2.Perpendicular(targetTransform.position - targetTransform2.position);

        float midPointX = (player1.position.x + player2.position.x) * 0.5f;
        float midPointY = (player1.position.y + player2.position.y) * 0.5f;

        Vector2 point1 = new Vector2(midPointX - perpendicular.x + flankExtraDistance, midPointY - perpendicular.y + flankExtraDistance);
        Vector2 point2 = new Vector2(midPointX + perpendicular.x + flankExtraDistance, midPointY + perpendicular.y + flankExtraDistance);

        float distPoint1 = Vector2.Distance(transform.position, point1);
        float distPoint2 = Vector2.Distance(transform.position, point2);

        if (distPoint1 < distPoint2)
        {
            target = point1;
        }
        else
        {
            target = point2;
        }

        if (Vector2.Distance(transform.position, target) < 0.5f)
        {
            StateChange(EnemyState.ATTACK);
        }
    }

    void Attack()
    {
        target = targetTransform.position;
    }

    void Flee()
    {
        Vector2 pos = targetTransform.position - transform.position;
        pos.Normalize();
        target = new Vector2(targetTransform.position.x - pos.x * 10, targetTransform.position.y - pos.y * 10);
        stateTimer = 1;
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

    public void Stumble(float timer = 2)
    {
        if (stumbleCooldownTime < stumbleTimer)
        {
            state = EnemyState.STUCK;
            attackState = false;
            agent.speed = 0f;
            spriteRenderer.color = Color.cyan;
            Invoke(nameof(Stand), timer);
        }
        //play animation
        stateTimer = timer;
    }

    void Stand()
    {
        spriteRenderer.color = Color.white;
        RandomState();
        stumbleTimer = 0;
    }

    public void CancelAgentUpdate()
    {
        agent.updatePosition = false;
        
    }

    void AgentUpdate()
    {
        agent.nextPosition = transform.position;
        agent.updatePosition = true;
    }


}
