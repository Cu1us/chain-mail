using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    [SerializeField] Chain_Demo chainToFollow;
    [SerializeField][Min(0)] float chainFollowStrength;
    [SerializeField] float cameraSizeByChainVelocity;


    Camera cameraToMove;
    float cameraSize;

    void Start()
    {
        if (!chainToFollow)
            chainToFollow = FindFirstObjectByType<Chain_Demo>();
        cameraToMove = Camera.main;
        cameraSize = cameraToMove.orthographicSize;
    }

    void Update()
    {
        Vector3 cameraTarget = new(chainToFollow.worldPivot.x, chainToFollow.worldPivot.y, cameraToMove.transform.position.z);

        cameraTarget = Vector3.Lerp(cameraToMove.transform.position, cameraTarget, chainFollowStrength * Time.deltaTime);
        cameraToMove.transform.position = cameraTarget;

        cameraToMove.orthographicSize = cameraSize + chainToFollow.rotationalVelocity * cameraSizeByChainVelocity;

    }
}
