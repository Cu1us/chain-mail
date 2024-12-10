using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] float selfDestructTimer;
    [HideInInspector] public Transform targetTransform;
    [HideInInspector] public float timer;

    void Update()
    {
        UpdatePosition();
        DestroyGameObject();
    }

    void DestroyGameObject()
    {
        timer += Time.deltaTime;

        if(timer > selfDestructTimer)
        {
            Destroy(gameObject);
        }
    }

    void UpdatePosition()
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position;
        }
    }
}
