using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    [SerializeField] Chain chainToFollow;
    [SerializeField][Min(0)] float chainFollowStrength;
    [SerializeField] float cameraSizeByChainVelocity;


    Camera cameraToMove;
    float cameraSize;

    void Start()
    {
        if (!chainToFollow)
        {
            chainToFollow = FindFirstObjectByType<Chain>();
            if (chainToFollow == null)
                Debug.LogWarning("The camera movement script is missing a Chain object reference - the old Chain_Demo will not work anymore");
        }
        cameraToMove = Camera.main;
        cameraSize = cameraToMove.orthographicSize;
    }

    void Update()
    {
        Vector3 cameraTarget = new(chainToFollow.Pivot.x, chainToFollow.Pivot.y, cameraToMove.transform.position.z);

        cameraTarget = Vector3.Lerp(cameraToMove.transform.position, cameraTarget, chainFollowStrength * Time.deltaTime);
        cameraToMove.transform.position = cameraTarget;

        cameraToMove.orthographicSize = cameraSize + chainToFollow.rotationalVelocity * cameraSizeByChainVelocity;

    }
}
