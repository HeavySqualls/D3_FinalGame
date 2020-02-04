using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeedback : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float flashDelay = 0.1f;
    private float flashDuration = 0.4f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public IEnumerator IFlashRed()
    {
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
