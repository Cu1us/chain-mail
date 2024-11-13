using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanielTest : MonoBehaviour
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
  
        other.transform.position =new Vector2 (transform.position.x+2f, transform.position.y+2f);


   }
}
