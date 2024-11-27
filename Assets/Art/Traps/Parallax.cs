
using UnityEngine;
 
public class Parallax : MonoBehaviour {
    private Transform cam;
    private Vector3 prevCamPos;
    public float simulatedDistance;
 
    void Awake(){
        cam = Camera.main.transform;
    }
 
    void Start(){
        prevCamPos = cam.position;
    }
 
    void Update(){
        if(simulatedDistance == 0) return; // stops dividing by zero
        Vector3 camDelta = cam.position - prevCamPos;
        camDelta.z = 0;
        prevCamPos = cam.position;
        transform.position += -camDelta / simulatedDistance;        
    }
}