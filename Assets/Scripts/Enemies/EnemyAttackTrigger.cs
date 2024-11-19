using UnityEngine;

public class EnemyAttackTrigger : MonoBehaviour
{

    public bool isPlayerInRange = false;
    int playersInRange = 0;


    void OnTriggerEnter2D(Collider2D other)
    {
        playersInRange++;
        isPlayerInRange = true;

    }

    void OnTriggerExit2D()
    {
        playersInRange--;
        if (playersInRange == 0)
        {
            isPlayerInRange = true;
        }
    }

}
