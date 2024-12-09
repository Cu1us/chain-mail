using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    public static VFX Spawn(VFXType type, Vector2 position, Quaternion rotation)
    {
        VFX effect = Instantiate(VFXManager.GetPrefab(type), position, rotation);
        return effect;
    }
    public static VFX Spawn(VFXType type, Vector2 position)
    {
        return Spawn(type, position, Quaternion.identity);
    }
    public static VFX Spawn(VFXType type, Vector2 position, Vector2 facingDir)
    {
        return Spawn(type, position, Quaternion.LookRotation(facingDir.normalized));
    }

    [SerializeField] ParticleSystem[] particleSystems;
    float lifetime;

    void Start()
    {
        float duration = 0.25f;
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.Play();
            duration = Mathf.Max(duration, particleSystem.main.duration);
        }
        lifetime = duration;
    }

    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
