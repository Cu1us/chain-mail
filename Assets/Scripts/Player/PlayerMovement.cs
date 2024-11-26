using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PlayerInputData))]
public class PlayerMovement : MonoBehaviour, IKnockable
{
    [Header("Status")]
    [ReadOnlyInspector] public Vector2 velocity;

    [ReadOnlyInspector] public bool beingGrabbed;
    [ReadOnlyInspector] public float swingVelocity;
    [ReadOnlyInspector] public Vector2 swingForwardDirection;

    [Header("Settings")]
    [SerializeField] float movementSpeed;
    [SerializeField] float velocityDeceleration;
    [SerializeField] Collider2D[] canCollideWith;
    [SerializeField] Vector2 localPointOfCollision;

    [Header("References")]
    [ReadOnlyInspector][SerializeField] PlayerInputData Input;

    // Properties
    public Vector2 position { get { return transform.position; } set { SetPosition(value, transform.position); } }

    // Local variables
    //

    void SetPosition(Vector2 newPos, Vector2 oldPos)
    {
        if (newPos == oldPos) return;
        Vector2 collisionCheckPoint = newPos + localPointOfCollision;
        if (!IsPointInACollider(newPos))
        {
            transform.position = newPos;
        }
        else
        {
            Debug.Log("In collider!");
        }
    }

    bool IsPointInACollider(Vector2 position)
    {
        foreach (Collider2D collider in canCollideWith)
        {
            if (collider.OverlapPoint(position))
                return true;
        }
        return false;
    }

    void Reset()
    {
        Input = GetComponent<PlayerInputData>();
    }

    void Update()
    {
        if (!beingGrabbed && Input.chainRotationalInput == 0) position += Input.movementInput * movementSpeed * Time.deltaTime;

        position += velocity * Time.deltaTime;
        velocity = Vector2.MoveTowards(velocity, Vector2.zero, velocityDeceleration * Time.deltaTime);
    }

    public void MoveTowards(Vector2 target, float distance)
    {
        position = Vector2.MoveTowards(position, target, distance);
    }
    public void RotateAround(Vector2 pivot, float angle)
    {
        Vector2 direction = position - pivot;
        direction = Quaternion.Euler(0, 0, angle) * direction;
        position = pivot + direction;
    }
    public void Launch(Vector2 force)
    {
        velocity += force;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (swingVelocity > 1f && other.gameObject.CompareTag("Enemy"))
        {
            if (other.gameObject.TryGetComponent(out Rigidbody2D rigidbody) && other.gameObject.TryGetComponent(out Pathfinding pathfinding))
            {
                Debug.Log("Hit! " + swingForwardDirection + ", " + swingVelocity);
                pathfinding.CancelAgentUpdate();
                rigidbody.velocity += swingForwardDirection * swingVelocity * 2.25f;

                Physics2D.IgnoreCollision(other, GetComponent<Collider2D>());
                StartCoroutine(EnableCollisionAfterTime(0.5f, other, GetComponent<Collider2D>()));

                IEnumerator EnableCollisionAfterTime(float time, Collider2D col1, Collider2D col2)
                {
                    yield return new WaitForSeconds(time);
                    Physics2D.IgnoreCollision(col1, col2, false);
                }
            }
        }
    }
}

public interface IKnockable
{
    public void Launch(Vector2 force);
}