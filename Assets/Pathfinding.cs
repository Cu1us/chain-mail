using UnityEngine;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour
{   
    NavMeshAgent agent;
    [SerializeField] Transform player1;
    [SerializeField] Transform player2;

    Transform targetTransform;
    Vector2 target;
   // Rigidbody2D rb;
    public bool targetInRange;
    float minDistanceToPlayer = 2;

    void Start()
    {
       // rb = GetComponent<Rigidbody2D>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;        

    }

    void Update()
    {
        ComparePlayerDistance();

        target = targetTransform.position;

        //MinimumDistance();
        agent.SetDestination(target);
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
        //Debug.Log(Vector2.Distance(targetTransform.position, transform.position));
        if(Vector2.Distance(targetTransform.position, transform.position) < minDistanceToPlayer)
        {
            Vector2 pos =new Vector2(targetTransform.position.x-transform.position.x, targetTransform.position.y-transform.position.y);
           pos.Normalize();
           target=new Vector2(targetTransform.position.x -pos.x*2, targetTransform.position.y-pos.y*2);
           Debug.Log(target);
        }
    }

}
