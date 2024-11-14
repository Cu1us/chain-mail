using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class EnemyMovment : MonoBehaviour
{

    [SerializeField] Transform playerTransform;
    SpriteRenderer spriteRenderer;
    [SerializeField] float movementSpeed = 0.7f;
    

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.Find("Player1").transform;
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, movementSpeed*Time.deltaTime);
    }

    
    public void Stumble()
    {
        movementSpeed = 0;
        spriteRenderer.color = Color.cyan;
        Invoke("StandUp", 2f);
    }

    void StandUp()
    {
        movementSpeed = 0.7f;
        spriteRenderer.color = Color.white;
    }



}
