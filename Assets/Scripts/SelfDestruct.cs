using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] float selfDestructTimer;
    void Start()
    {
        Invoke(nameof(DestroyGameObject), selfDestructTimer);
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
