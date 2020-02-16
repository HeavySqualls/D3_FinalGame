using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerFeedback : MonoBehaviour
{
    [Header("RED FLASH:")]
    [Tooltip("The length of time the object will flash.")]
    public float flashDuration = 0.4f;
    private SpriteRenderer spriteRenderer;
    private float flashDelay = 0.1f;

    [Space]
    [Header("CAMERA SHAKE:")]
    [Tooltip("The length of time the object will shake.")]
    public float shakeDuration = 0.8f;
    [Tooltip("The strength of the shake.")]
    public float shakeStrength = 0.25f;
    [Tooltip("The number of shakes that will occur during the duration of the shake.")]
    public int vibrato = 9;
    [Tooltip("The randomness of the shake postitions.")]
    public float randomness = 2f;
    Camera cam;
    Tween shakeTween;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;
    }

    public IEnumerator IFlashRed()
    {
        shakeTween = cam.transform.DOShakePosition(0.8f, 0.25f, 9, 2f, false, true);

        for (float i = 0; i < flashDuration; i += flashDelay)
        {
            if (spriteRenderer.color == Color.white)
            {
                spriteRenderer.color = Color.red;
            }
            else if (spriteRenderer.color == Color.red)
            {
                spriteRenderer.color = Color.white;
            }

            yield return new WaitForSeconds(flashDelay);
        }

        spriteRenderer.color = Color.white;

        yield break;
    }
}
