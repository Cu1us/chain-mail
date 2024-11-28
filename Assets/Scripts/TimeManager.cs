using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    public static float timeScale = 1;
    static double freezeUntil;
    public static bool IsFrozen { get { return freezeUntil > Time.unscaledTimeAsDouble; } }

    void Awake()
    {
        if (!instance) instance = this;
        timeScale = 1f;
    }

    public static void Freeze(float duration)
    {
        freezeUntil = System.Math.Max(freezeUntil, Time.unscaledTimeAsDouble) + duration;
        UpdateTimeScale();
    }
    public static void Unfreeze()
    {
        freezeUntil = default;
        UpdateTimeScale();
    }
    public static void ResetTimeScale()
    {
        freezeUntil = default;
        timeScale = 1;
        UpdateTimeScale();
    }

    void Update()
    {
        UpdateTimeScale();
    }

    static void UpdateTimeScale()
    {
        float newTimeScale = timeScale;
        if (Time.unscaledTimeAsDouble < freezeUntil)
        {
            timeScale = 0;
        }
        Time.timeScale = timeScale;
    }
}
