using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class EnemyMovment : MonoBehaviour
{
    // Start is called before the first frame update


    [SerializeField] Transform playerTransform;
    SpriteRenderer spriteRenderer;
    float movmentSpeed = 5;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, movmentSpeed*Time.deltaTime);
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

    void OnTriggerEnter2D()
    {
        Stumble();
    }

}
