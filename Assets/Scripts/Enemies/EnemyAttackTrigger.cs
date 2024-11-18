using UnityEngine;

public class EnemyAttackTrigger : MonoBehaviour
{
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
         
    }

     void OnTriggerEnter2D(Collider2D other)
    {
        Attack();  

    }

    void Attack()
    {
        Debug.Log("Attacking");
    }
}
