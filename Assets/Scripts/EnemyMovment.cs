using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class EnemyMovment : MonoBehaviour
{

    [SerializeField] Transform playerTransform;
    SpriteRenderer spriteRenderer;
    float movmentSpeed = 5;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, movmentSpeed*Time.deltaTime);
    }

    void OnTriggerEnter2D()
    {
        Stumble();
    }
    
    public void Stumble()
    {
        movmentSpeed = 0;
        spriteRenderer.color = Color.cyan;
        Invoke("StandUp", 2f);
    }

    void StandUp()
    {
        movmentSpeed = 5;
        spriteRenderer.color = Color.white;
    }



}
