using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticle : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(SelfDestruct), 2);
    }

    void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
