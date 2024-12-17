using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    public static CameraMovement instance;
    [SerializeField] Chain chainToFollow;
    [SerializeField][Min(0)] float chainFollowStrength;
    [SerializeField] float cameraSizeByChainVelocity;


    Camera cameraToMove;
    float cameraSize;

    Vector3 cameraPosition;
    Vector3 cameraShakeOffset;

    float cameraShakeDuration;

    void Start()
    {
        if (!instance) instance = this;
        if (!chainToFollow)
        {
            chainToFollow = FindFirstObjectByType<Chain>();
            if (chainToFollow == null)
                Debug.LogWarning("The camera movement script is missing a Chain object reference, and there is no chain type object in the scene");
        }
        cameraToMove = Camera.main;
        cameraPosition = cameraToMove.transform.position;
        cameraSize = cameraToMove.orthographicSize;
    }

    void Update()
    {
        Vector3 cameraTarget = new(chainToFollow.Pivot.x, chainToFollow.Pivot.y, cameraToMove.transform.position.z);

        cameraPosition = Vector3.Lerp(cameraPosition, cameraTarget, chainFollowStrength * Time.deltaTime);


        if (cameraShakeDuration > 0)
        {
            cameraShakeDuration -= Time.deltaTime;
            float intensity = cameraShakeDuration * 2;
            Vector3 dirToShakeTo = (Random.onUnitSphere - cameraShakeOffset).normalized;
            cameraShakeOffset += dirToShakeTo * intensity;
            if (cameraShakeOffset.magnitude > intensity)
                cameraShakeOffset = cameraShakeOffset.normalized * intensity;
        }
        else
            cameraShakeOffset = Vector2.zero;
        cameraToMove.transform.position = cameraPosition + (Vector3)cameraShakeOffset;

        float targetSize = cameraSize + Mathf.Abs(chainToFollow.rotationalVelocity) * cameraSizeByChainVelocity;
        cameraToMove.orthographicSize = Mathf.Lerp(cameraToMove.orthographicSize, targetSize, Time.deltaTime);
    }

    public static void Shake(float duration, bool additive = false)
    {
        if (instance == null) return;
        if (additive)
            instance.cameraShakeDuration += duration;
        else
            instance.cameraShakeDuration = Mathf.Max(instance.cameraShakeDuration, duration);
    }
}
