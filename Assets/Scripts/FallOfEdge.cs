using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallOfEdge : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int orderInLayer;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyMovement>().StateChange(EnemyMovement.EnemyState.FALLING);
            other.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
            other.GetComponent<EnemyHealth>().FallToDeath();
            other.GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }
}
