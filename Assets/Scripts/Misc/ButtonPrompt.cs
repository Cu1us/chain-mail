using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPrompt : MonoBehaviour
{
    public bool visible;
    [ReadOnlyInspector][SerializeField] SpriteRenderer spriteRenderer;
    [ReadOnlyInspector][SerializeField] Image image;
    public float OscillateSpeed;
    [Range(0f, 1f)] public float OscillateStrength;

    [Header("Icons")]
    [SerializeField] Sprite Keyboard;
    [SerializeField] Sprite PS4;
    [SerializeField] Sprite Xbox;

    Vector2 defaultScale;
    void Reset() { TryGetComponent(out spriteRenderer); TryGetComponent(out image); }
    void Awake() => defaultScale = transform.localScale;

    void Update()
    {
        if (spriteRenderer) spriteRenderer.enabled = visible;
        if (image) image.enabled = visible;
        if (visible)
        {
            if (OscillateSpeed != 0)
            {
                if (spriteRenderer) transform.localScale = defaultScale * (1 + (Mathf.Sin(Time.time * OscillateSpeed) * OscillateStrength));
            }
            SetSprite(PlayerInputData.inputType switch
            {
                PlayerInputData.InputType.Keyboard => Keyboard,
                PlayerInputData.InputType.Xbox => Xbox,
                PlayerInputData.InputType.PS4 => PS4,
                _ => PS4
            });
        }
    }

    void SetSprite(Sprite sprite)
    {
        if (spriteRenderer) spriteRenderer.sprite = sprite;
        if (image) image.sprite = sprite;
    }
}
