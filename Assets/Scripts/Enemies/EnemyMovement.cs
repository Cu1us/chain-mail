using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{

    public static readonly List<EnemyMovement> EnemyList = new();

    [Header("Players")]
    Transform player1;
    Transform playerRock;
    public Transform targetTransform1;
    Transform targetTransform2;
    GameObject chainAndPlayers;

    [Header("Referenses")]
    NavMeshAgent agent;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Chain chain;
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
    float currentChainLength;


    Vector2 target;
    Vector2 direction;
    Vector2 distToTarget;

    [Header("Bools")]
    public bool isAttackState;
    [Header("EnemyType")]
    [SerializeField] bool isArcher;
    [SerializeField] bool isSentinel;
    [SerializeField] bool isSwordman;


    [Header("Timers")]
    public float stumbleTimer = 2;
    public float stumbleTimerCooldown = 2;
    float stateTimer;
    float stateChangeCooldown = 3;
    [SerializeField] float stumbleTime = 3;


    public enum EnemyState
    {
        STUCK, MOVECLOSETOATTACK, FLANK, KEEPDISTANCE, INTERCEPT, ARCHER, IDLE

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
        playerRock = GameObject.Find("Rock").transform;
        chainAndPlayers = GameObject.Find("Chain and Players");
        chain = chainAndPlayers.GetComponent<Chain>();

        targetTransform1 = player1;
        targetTransform2 = playerRock;
        nextState = state;
        grabber = chain.Anchor.transform;
        currentMaxVelocity = maxVelocity;
        accell = acceleration;
        agent.speed = currentMaxVelocity;
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
            case EnemyState.INTERCEPT:
                Intercept();
                break;
            case EnemyState.ARCHER:
                ArcherMoveTo();
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
        int random = Random.Range(0, 7);

        if (isSwordman || isSentinel)
        {
            if (chain.rotationalVelocity != 0)
            {
                Debug.Log("Intercept");
                nextState = EnemyState.INTERCEPT;
                StateChange(EnemyState.INTERCEPT);
                return;

            }
            if (nextState == state)
            {

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



        else if (isArcher)
        {
            state = EnemyState.ARCHER;
            StateChange(state);
        }
    }

    public void StateChange(EnemyState _state)
    {
        stateTimer = 0;
        state = _state;
        currentMaxVelocity = maxVelocity;
        isAttackState = true;
        if (isArcher && _state != EnemyState.STUCK)
        {
            state = EnemyState.ARCHER;
        }
        switch (state)
        {
            case EnemyState.STUCK:
                isAttackState = false;
                currentMaxVelocity = 0;
                break;
            case EnemyState.KEEPDISTANCE:
            stateTimer = stateChangeCooldown -2;
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
                nextState = EnemyState.MOVECLOSETOATTACK;
                break;
            case EnemyState.INTERCEPT:
                stateTimer = -2;
                break;
            case EnemyState.ARCHER:
                break;

        }
        agent.speed = currentMaxVelocity;
    }

    void ClosestEnemy()
    {
        Vector2 dist1 = player1.position - transform.position;
        Vector2 dist2 = playerRock.position - transform.position;

        if (dist1.sqrMagnitude < dist2.sqrMagnitude)
        {
            targetTransform1 = player1;
            targetTransform2 = playerRock;
        }
        else
        {
            targetTransform1 = playerRock;
            targetTransform2 = player1;
        }

    }

    void ArcherMoveTo()
    {
        float distFromCenter = 10;
        Vector2 perpendicular = Vector2.Perpendicular(targetTransform1.position).normalized;

        Vector2 dist1 = targetTransform1.position - transform.position;
        if (dist1.sqrMagnitude < 64)
        {
            isAttackState = false;
        }
        else
        {
            isAttackState = true;
        }
        Vector2 point1 = (Vector2)targetTransform1.position + perpendicular * distFromCenter;
        Vector2 point2 = (Vector2)targetTransform1.position - perpendicular * distFromCenter;

        float dot1 = Vector2.Dot(transform.position - targetTransform1.position, point1);
        float dot2 = Vector2.Dot(transform.position - targetTransform1.position, point2);

        if (dot1 > dot2)
        {
            target = point1;
        }
        else
        {
            target = point2;
        }
    }

    void Flank()
    {
        Vector2 midPoint = (player1.position + playerRock.position) / 2;
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
        target = (Vector2)grabber.position + dist * interceptDistance + dist * 1.8f + perpendicular.normalized * 2;
    }

    public void Stumble()
    {
        if (stumbleTimer > stumbleTimerCooldown)
        {
            animator.Play("Stumble");
            Invoke(nameof(Stand), stumbleTime);
            stateTimer += stumbleTime;
            StateChange(EnemyState.STUCK);
        }
    }

    void Stand()
    {
        stumbleTimer = 0;
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
        if (state != EnemyState.STUCK && rb.velocity.sqrMagnitude < currentMaxVelocity*currentMaxVelocity )
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
        if (rb.velocity.sqrMagnitude > maxVelocity * maxVelocity)
        {
            accell = knockbackDeceleration;
        }
        else
        {
            accell = acceleration;
        }
    }

    void StoppMoving()
    {
        currentMaxVelocity = 0;
    }
    void ContinueMoving()
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
