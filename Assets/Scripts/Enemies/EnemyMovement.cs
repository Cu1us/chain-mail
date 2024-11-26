using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{

    NavMeshAgent agent;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Transform player1;
    Transform player2;
    Transform targetTransform1;
    Transform targetTransform2;
    Vector2 target;
    Vector2 direction;
    Vector2 distToTarget;
    [SerializeField] float acceleration;
    [SerializeField] float maxVelocity;
    [SerializeField] float flankExtraDistance;
    [SerializeField] float keepDistanceDistance;
    [SerializeField] float attackDistance;
    public bool isAttackState;
    [SerializeField] Chain chain;
    float currentChainLength;
    float stumbleTimer;
    float stumbleTimerCooldown;
    float stateTimer;
    float stateChangeCooldown = 5;


    public enum EnemyState
    {
        STUCK, MOVECLOSETOATTACK, FLANK, GUARD, KEEPDISTANCE, INTERCEPT, CHARGE, IDLE

    }
    public EnemyState state;

    enum FlankDir
    {
        LEFT, RIGHT
    }
    FlankDir flankDir;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.updatePosition = false;

        player1 = GameObject.Find("Player1").transform;
        player2 = GameObject.Find("Player2").transform;
        targetTransform1 = player1;
        targetTransform2 = player2;
    }


    void Update()
    {
        
        stumbleTimer += Time.deltaTime;
        stateTimer += Time.deltaTime;
        if (stateTimer > stateChangeCooldown)
        {
            RandomState();
        }
        ClosestEnemy();

        switch (state)
        {
            case EnemyState.STUCK:
                break;
            case EnemyState.KEEPDISTANCE:
                KeepDistance();
                break;
            case EnemyState.MOVECLOSETOATTACK:
                MoveCloseToAttack();
                break;
            case EnemyState.FLANK:
                Flank();
                break;
            case EnemyState.GUARD:
                break;
            case EnemyState.INTERCEPT:
                Intercept();
                break;
            case EnemyState.CHARGE:
                break;
        }




        agent.SetDestination(target);

        direction = agent.nextPosition - transform.position;
        distToTarget = (Vector2)transform.position - target;

        if (distToTarget.sqrMagnitude < 0.625)
        {
            direction = Vector2.zero;
        }
        else
        {
            direction.Normalize();
        }

        rb.velocity = Vector2.MoveTowards(rb.velocity, direction * maxVelocity, acceleration * Time.deltaTime);
        agent.nextPosition = transform.position;
        Flip();
    }

    public void RandomState()
    {
        if (true)
        {
            int random = Random.Range(0, 7);
            
            if (random <= 2)
            {
                state = EnemyState.KEEPDISTANCE;
            }
            else if (random > 2 && random <= 4)
            {
                state = EnemyState.MOVECLOSETOATTACK;
            }
            else if (random > 4 && random <= 6)
            {
                state = EnemyState.FLANK;
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
        stateTimer = 0;
        state = _state;
        maxVelocity = 5;
        isAttackState = true;
        switch (state)
        {

            case EnemyState.STUCK:
                isAttackState = false;
                break;
            case EnemyState.KEEPDISTANCE:
                break;
            case EnemyState.MOVECLOSETOATTACK:
                break;
            case EnemyState.FLANK:
                if (Random.Range(0, 2) == 0)
                {
                    flankDir = FlankDir.LEFT;
                }
                else
                {
                    flankDir = FlankDir.RIGHT;
                }
                break;
            case EnemyState.GUARD:
                break;
            case EnemyState.INTERCEPT:
                break;
            case EnemyState.CHARGE:
                break;
        }
    }

    void ClosestEnemy()
    {
        Vector2 dist1 = player1.position - transform.position;
        Vector2 dist2 = player2.position - transform.position;

        if (dist1.sqrMagnitude < dist2.sqrMagnitude)
        {
            targetTransform1 = player1;
            targetTransform2 = player2;
        }
        else
        {
            targetTransform1 = player2;
            targetTransform2 = player1;
        }

    }

    void Flank()
    {
        Vector2 midPoint = (player1.position + player2.position) / 2;
        Vector2 dir = (Vector2)transform.position - midPoint;
        dir.Normalize();
        Vector2 perpendicular = Vector2.Perpendicular(dir);
        perpendicular.Normalize();

        if (flankDir == FlankDir.LEFT)
        {
            target = midPoint + dir * flankExtraDistance - perpendicular * 4;

        }
        else
        {
            target = midPoint + dir * flankExtraDistance + perpendicular * 4;
        }
    }

    void Guard()
    {

    }

    void KeepDistance()
    {
        target = targetTransform1.position + (transform.position - targetTransform1.position).normalized * keepDistanceDistance;
    }

    void MoveCloseToAttack()
    {
        target = targetTransform1.position + (transform.position - targetTransform1.position).normalized * attackDistance;
    }

    void Intercept()
    {
        float interceptDistance = chain.currentChainLength;
        target =targetTransform1.position + (transform.position - targetTransform1.position).normalized * interceptDistance;
    }

    public void Stumble()
    {
        if (stumbleTimer < stumbleTimerCooldown)
        {
            maxVelocity = 0;
            state = EnemyState.STUCK;
            Invoke(nameof(Stand), 2f);
        }

    }

    void Stand()
    {
        StateChange(EnemyState.MOVECLOSETOATTACK);
    }

    void Flip()
    {
        if (state != EnemyState.STUCK)
        {
            float x = targetTransform1.position.x - transform.position.x;
            if (x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }




}
