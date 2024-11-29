using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    public static float timeScale = 1;
    public static float slowmoFactor = 1;
    static double freezeUntil;
    public static bool IsFrozen { get { return freezeUntil > Time.unscaledTimeAsDouble; } }
    static Guid slowmoCoroutineGUID;

    void Awake()
    {
        if (!instance) instance = this;
        ResetTimeScale();
    }

    public static void Freeze(float duration, bool additive = true)
    {
        if (additive)
            freezeUntil = Math.Max(freezeUntil, Time.unscaledTimeAsDouble) + duration;
        else
            freezeUntil = Time.unscaledTimeAsDouble + duration;
        UpdateTimeScale();
    }
    public static void Slowmo(float duration, float timeSpeed, Action onFinish = null)
    {
        slowmoCoroutineGUID = Guid.NewGuid();
        instance.StartCoroutine(instance.SlowmoTime(duration, timeSpeed, onFinish, slowmoCoroutineGUID));
    }
    IEnumerator SlowmoTime(float duration, float timeSpeed, Action onFinish, Guid guid)
    {
        float slowmoUntil = Time.unscaledTime + duration;
        slowmoFactor = timeSpeed;
        UpdateTimeScale();
        yield return new WaitUntil(() => !guid.Equals(slowmoCoroutineGUID) || Time.unscaledTime > slowmoUntil);
        if (guid.Equals(slowmoCoroutineGUID))
            slowmoFactor = 1;
        UpdateTimeScale();
        onFinish?.Invoke();
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
        instance.StopAllCoroutines();
        slowmoFactor = 1;
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
            newTimeScale = 0;
        }
        newTimeScale *= slowmoFactor;
        Time.timeScale = newTimeScale;
    }
}
