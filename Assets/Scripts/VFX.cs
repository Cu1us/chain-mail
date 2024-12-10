using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    public static VFX Spawn(VFXType type, Vector2 position, Vector2 facingDir)
    {
        VFX effect = Instantiate(VFXManager.GetPrefab(type), position, Quaternion.identity);
        effect.facingDir = facingDir;
        return effect;
    }
    public static VFX Spawn(VFXType type, Vector2 position)
    {
        return Instantiate(VFXManager.GetPrefab(type), position, Quaternion.identity);
    }

    [SerializeField] ParticleSystem[] particleSystems;
    float lifetime;
    Vector2 facingDir = Vector2.zero;

    void Start()
    {
        float duration = 0.25f;
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            ParticleSystem.MainModule main = particleSystem.main;
            ParticleSystem.MinMaxCurve startRotation = main.startRotation;

            float rotation = Vector2.SignedAngle(Vector2.right, facingDir) * Mathf.Deg2Rad;
            if (facingDir != Vector2.zero) startRotation.constant = rotation + startRotation.constant;

            main.startRotation = startRotation;
            particleSystem.Play();
            duration = Mathf.Max(duration, main.duration);
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
