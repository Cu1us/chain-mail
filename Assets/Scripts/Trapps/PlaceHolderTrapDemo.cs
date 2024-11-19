using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlaceHolderTrapDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<Pathfinding>().Stumble(10);
            other.transform.position = transform.position;
            other.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
        }
        else if(other.CompareTag("Player"))
        {
            
        }
    }
}
