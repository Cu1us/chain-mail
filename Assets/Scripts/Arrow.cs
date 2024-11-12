using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] float maxArrowSpeed;
    public float arrowSpeed;

    void Start()
    {
        Invoke(nameof(SelfDestruct), 3);
        arrowSpeed *= maxArrowSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.Translate(Vector3.up * arrowSpeed * Time.deltaTime);
    }

    void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
