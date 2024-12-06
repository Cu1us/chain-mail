using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] float selfDestructTimer;
    [HideInInspector] public Transform targetTransform;

    void Start()
    {
        Invoke(nameof(DestroyGameObject), selfDestructTimer);
    }

    void Update()
    {
        UpdatePosition();
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    void UpdatePosition()
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position;
        }
    }
}
