using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovment : MonoBehaviour
{
    // Start is called before the first frame update


    [SerializeField] Transform playerTransform;
    float movmentSpeed = 5;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, movmentSpeed*Time.deltaTime);
    }

    IEnumerator Stumble()
    {
        movmentSpeed = 0;
        yield return new WaitForSeconds(3f);
        movmentSpeed = 5;
        
    }

    void OnTriggerEnter2D()
    {
        StartCoroutine(Stumble());
    }

}
