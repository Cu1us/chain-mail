using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ButtonPrompt : MonoBehaviour
{
    public bool visible;
    [ReadOnlyInspector][SerializeField] SpriteRenderer spriteRenderer;
    public float OscillateSpeed;
    [Range(0f, 1f)] public float OscillateStrength;

    [Header("Icons")]
    [SerializeField] Sprite Keyboard;
    [SerializeField] Sprite PS4;
    [SerializeField] Sprite Xbox;

    Vector2 defaultScale;
    void Reset() => spriteRenderer = GetComponent<SpriteRenderer>();
    void Awake() => defaultScale = transform.localScale;

    void Update()
    {
        spriteRenderer.enabled = visible;
        if (visible)
        {
            if (OscillateSpeed != 0)
            {
                transform.localScale = defaultScale * (1 + (Mathf.Sin(Time.time * OscillateSpeed) * OscillateStrength));
            }
            spriteRenderer.sprite = PlayerInputData.inputType switch
            {
                PlayerInputData.InputType.Keyboard => Keyboard,
                PlayerInputData.InputType.Xbox => Xbox,
                PlayerInputData.InputType.PS4 => PS4,
                _ => PS4
            };
        }
    }
}
