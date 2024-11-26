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
    float sqrMagnitude;


    public enum EnemyState
    {
        STUCK, MOVECLOSETOATTACK, FLANK, GUARD, KEEPDISTANCE, INTERCEPT, CHARGE

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

        StateChange(EnemyState.KEEPDISTANCE);
    }


    void Update()
    {

        ClosestEnemy();

        switch (state)
        {
            case EnemyState.STUCK:
                break;
            case EnemyState.KEEPDISTANCE:
            KeepDistance();
                break;
            case EnemyState.MOVECLOSETOATTACK:
                break;
            case EnemyState.FLANK:
                Flank();
                break;
            case EnemyState.GUARD:
                break;
            case EnemyState.INTERCEPT:
                break;
            case EnemyState.CHARGE:
                break;
        }
        //target = player1.position + (transform.position - player1.position).normalized * 2;
        agent.SetDestination(target);

        direction = agent.nextPosition - transform.position;
        distToTarget.x = transform.position.x - target.x;
        distToTarget.y = transform.position.y - target.y;

        // if (distToTarget.sqrMagnitude < 0.1)
        // {
        //     direction = Vector2.zero;
        // }
        // else
        // {
            
        // }
direction.Normalize();
        rb.velocity = Vector2.MoveTowards(rb.velocity, direction * maxVelocity, acceleration * Time.deltaTime);
        //Debug.Log(rb.velocity.magnitude);
        agent.nextPosition = transform.position;
    }

    public void StateChange(EnemyState _state)
    {
        state = _state;

        switch (state)
        {
            case EnemyState.STUCK:
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
        Vector2 midPoint = (player1.position+player2.position)/2;
        Vector2 dir =new Vector2( transform.position.x - player1.position.x, transform.position.y-player1.position.y);
        dir.Normalize();
        Vector2 perpendicular = Vector2.Perpendicular(dir);
        perpendicular.Normalize();
        
        if (flankDir == FlankDir.LEFT)
        {
            target = midPoint +dir*flankExtraDistance - perpendicular*4;
 
        }
        else
        {
            target = midPoint + dir*flankExtraDistance + perpendicular*4;
        }
    }

    void Guard()
    {

    }

    void KeepDistance()
    {
        target = (transform.position - targetTransform1.position).normalized*keepDistanceDistance;
    }

    void MoveCloseToAttack()
    {

    }

    void Intercept()
    {

    }



}
