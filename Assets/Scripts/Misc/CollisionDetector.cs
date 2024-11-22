using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollisionDetector : MonoBehaviour
{
    [ReadOnlyInspector] public Collider2D thisCollider;

    public Action<Collision2D> onCollisionEnter;
    public Action<Collision2D> onCollisionExit;
    public Action<Collider2D> onTriggerEnter;
    public Action<Collider2D> onTriggerExit;


    void Reset()
    {
        thisCollider = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        onCollisionEnter?.Invoke(other);
    }
    void OnCollisionExit2D(Collision2D other)
    {
        onCollisionExit?.Invoke(other);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter?.Invoke(other);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        onTriggerExit?.Invoke(other);
    }
}
