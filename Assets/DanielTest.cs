using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanielTest : MonoBehaviour
{
     public bool playerInAttackRange;
     int playersInattackRange = 0;
  
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   void OnTriggerEnter2D()
   {
  
       // other.transform.position =new Vector2 (transform.position.x+2f, transform.position.y+2f);
         
          playersInattackRange ++;
          playerInAttackRange = true; 
          Debug.Log(playersInattackRange);
          GetComponentInParent<Pathfinding>().targetInRange = true;
   }

   void OnTriggerExit2D()
   {
    playersInattackRange --;
    if (playersInattackRange == 0)
    {
      playerInAttackRange = false;
      GetComponentInParent<Pathfinding>().targetInRange = false;
    }
     
     Debug.Log(playersInattackRange);
   }
}
