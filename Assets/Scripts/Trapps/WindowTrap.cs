using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowTrap : MonoBehaviour
{

    [SerializeField] Sprite brokenWindowSprite;
    bool isWindowBroken;
    SpriteRenderer spriteRenderer;
    [SerializeField] Transform fallPos;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!isWindowBroken)
        {
            if(other.CompareTag("Enemy"))
            {
                float velocity = other.GetComponent<Rigidbody2D>().velocity.sqrMagnitude;
                if (velocity > 100)
                {
                    isWindowBroken = true;
                    spriteRenderer.sprite = brokenWindowSprite;
                }
            }
        }
        if(other.CompareTag("Enemy") && isWindowBroken && other.GetComponent<Rigidbody2D>().velocity.sqrMagnitude > 100)
        {
          

            other.GetComponent<EnemyMovement>().StateChange(EnemyMovement.EnemyState.STUCK);
            other.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            other.GetComponent<Transform>().position = fallPos.transform.position;
            other.GetComponentInChildren<BoxCollider2D>().enabled = false;
            other.GetComponent<Animator>().SetBool("Trapfall", true);
        }
        else if(other.CompareTag("Player"))
        {

        }
    }
}
