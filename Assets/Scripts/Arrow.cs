using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] float arrowSpeed;

    void Start()
    {
        Invoke(nameof(SelfDestruct), 3);
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
