using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SpriteBlink : MonoBehaviour
{
    [ReadOnlyInspector][SerializeField] new Renderer renderer;
    float blinkUntil;
    bool blinking = false;

    void Update()
    {
        if (!blinking && Time.time <= blinkUntil)
        {
            blinking = true;
            SetBlinkState(true);
        }
        else if (blinking && Time.time > blinkUntil)
        {
            blinking = false;
            SetBlinkState(false);
        }
    }

    public void Blink(float duration, bool additive = false)
    {
        if (additive)
            blinkUntil = Mathf.Max(blinkUntil, Time.time) + duration;
        else
            blinkUntil = Time.time + duration;
    }

    public void SetBlinkState(bool blinking)
    {
        renderer.material.SetInt("_Blink", blinking ? 1 : 0);
    }

    void Reset()
    {
        renderer = GetComponent<Renderer>();
    }
}
