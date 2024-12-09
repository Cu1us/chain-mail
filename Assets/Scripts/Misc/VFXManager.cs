using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager instance;

    [SerializeField] VFX CircleImpact;
    [SerializeField] VFX DirectionalImpact;
    [SerializeField] VFX GroundImpact;


    void Awake()
    {
        if (!instance)
            instance = this;
    }

    public static VFX GetPrefab(VFXType type)
    {
        VFX vfx = type switch
        {
            VFXType.CIRCLE_IMPACT => instance.CircleImpact,
            VFXType.DIRECTIONAL_IMPACT => instance.DirectionalImpact,
            VFXType.GROUND_IMPACT => instance.GroundImpact,
            _ => null
        };
        if (vfx == null)
            throw new NullReferenceException("VFXType does not have an associated prefab in the VFXManager object's inspector!");
        return vfx;
    }
}

public enum VFXType
{
    CIRCLE_IMPACT,
    DIRECTIONAL_IMPACT,
    GROUND_IMPACT
}