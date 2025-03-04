using UnityEngine;

public class DangerZone : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Animator animator;
    public float timer;
    public float triggerTime;
    Transform player1;
    Transform player2;
    [SerializeField] GameObject boom;
    Collider2D coll;

    void Start()
    {
        player1 = GameObject.Find("Player1").transform;
        player2 = GameObject.Find("Player2").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        coll.enabled = false;
        RandomTimer();

    }


    void Update()
    {
        timer += Time.deltaTime;
        if (timer > triggerTime)
        {
            transform.position = (player1.position + player2.position)*0.5f;
            RandomTimer();
            StartDangerZone();
            timer = 0;
        }

    }

    void RandomTimer()
    {
        triggerTime = Random.Range(10, 20);
    }

    void StartDangerZone()
    {
        animator.Play("DangerZone");
    }

    void Boom()
    {
        
        GameObject clone = Instantiate(boom, transform.position, Quaternion.identity);
        Destroy(clone, 0.5f);
    }

    void OnTriggerEnter2D(Collider2D other) //starts and stops collider in animator
    {
        Debug.Log("Boom");
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(50);

        }
    }

}
