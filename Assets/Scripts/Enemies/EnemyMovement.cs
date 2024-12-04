using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{

    public static readonly List<EnemyMovement> EnemyList = new();

    [Header("Players")]
    Transform player1;
    Transform player2;
    Transform targetTransform1;
    Transform targetTransform2;

    [Header("Referenses")]
    NavMeshAgent agent;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    [SerializeField] Chain chain;
    Transform grabber;
    [SerializeField] Animator animator;


    [Header("settings")]
    [SerializeField] float acceleration;
    float accell;
    [SerializeField] float knockbackDeceleration = 10;
    [SerializeField] float maxVelocity;
    float currentMaxVelocity;
    [SerializeField] float flankExtraDistance;
    [SerializeField] float keepDistanceDistance;
    [SerializeField] float attackDistance;



    Vector2 target;
    Vector2 direction;
    Vector2 distToTarget;

    [Header("Bools")]
    public bool isAttackState;
    [SerializeField] bool isSpearMan;

    float currentChainLength;
    [Header("Timers")]
    float stumbleTimer;
    float stumbleTimerCooldown = 2;
    float stateTimer;
    float stateChangeCooldown = 3;


    public enum EnemyState
    {
        STUCK, MOVECLOSETOATTACK, FLANK, GUARD, KEEPDISTANCE, INTERCEPT, CHARGE, IDLE

    }
    public EnemyState state;
    EnemyState nextState;

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
        nextState = state;
        grabber = chain.PlayerA.transform;
        currentMaxVelocity = maxVelocity;
        accell = acceleration;
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

        CompareVelocity();
        rb.velocity = Vector2.MoveTowards(rb.velocity, direction * currentMaxVelocity, accell * Time.deltaTime);

        agent.nextPosition = transform.position;
        Flip();
    }





    public void RandomState()
    {
        if (chain.rotationalVelocity != 0)
        {
            nextState =EnemyState.INTERCEPT;
        }
        if (nextState == state)
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
            nextState = state;
        }
        else
        {
            state = nextState;
        }
        StateChange(state);
    }

    public void StateChange(EnemyState _state)
    {
        stateTimer = 0;
        state = _state;
        currentMaxVelocity = maxVelocity;
        isAttackState = true;
        switch (state)
        {

            case EnemyState.STUCK:
                isAttackState = false;
                currentMaxVelocity = 0;
                break;
            case EnemyState.KEEPDISTANCE:
                break;
            case EnemyState.MOVECLOSETOATTACK:
                if (isSpearMan)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        state = EnemyState.CHARGE;
                    }
                    else
                    {
                        state = EnemyState.MOVECLOSETOATTACK;
                    }
                }
                else
                {
                    state = EnemyState.MOVECLOSETOATTACK;
                };
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
                nextState = EnemyState.MOVECLOSETOATTACK;
                break;
            case EnemyState.GUARD:
                break;
            case EnemyState.INTERCEPT:
                stateTimer = -2;
                break;
            case EnemyState.CHARGE:
                target = targetTransform1.position + (targetTransform1.position - transform.position).normalized * 2;
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
        Vector2 dist = (transform.position - grabber.position).normalized;
        Vector2 perpendicular = Vector2.Perpendicular(dist);
        target = (Vector2)grabber.position + dist * interceptDistance + dist * 1.5f + (perpendicular*chain.rotationalVelocity).normalized*3;
    }

    void Charge()
    {
    }

    public void Stumble()
    {
        if (stumbleTimer > stumbleTimerCooldown)
        {
            animator.Play("Stumble");
            Invoke(nameof(Stand), 2f);
            StateChange(EnemyState.STUCK);
        }

    }

    void Stand()
    {
        animator.Play("Walk");
        Vector2 dist = targetTransform1.position - transform.position;
        if (dist.sqrMagnitude > 16)
        {
            StateChange(EnemyState.FLANK);
        }
        else
        {
            StateChange(EnemyState.MOVECLOSETOATTACK);
        }

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

    void CompareVelocity()
    {
        animator.SetFloat("Velocity", rb.velocity.sqrMagnitude);
        if (rb.velocity.sqrMagnitude > maxVelocity*maxVelocity)
        {
            accell = knockbackDeceleration;
        }
        else
        {
            accell = acceleration;
        }
    }

    void StandStill()
    {
        currentMaxVelocity = 0;
    }
    void DoneAttacking()
    {
        currentMaxVelocity = maxVelocity;
    }


    void OnEnable()
    {
        if (!EnemyList.Contains(this))
        {
            EnemyList.Add(this);
        }
    }
    void OnDisable()
    {
        EnemyList.Remove(this);
    }
}
