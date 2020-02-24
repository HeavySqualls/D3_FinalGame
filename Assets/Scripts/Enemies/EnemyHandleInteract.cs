using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandleInteract : MonoBehaviour
{
    [Header("Red Flash:")]
    [Tooltip("The length of time the object will flash.")]
    public float flashDuration = 0.4f;
    private SpriteRenderer spriteRenderer;
    private float flashDelay = 0.1f;

    [Space]
    [Header("References:")]
    CrabwormLarvaController crabLarvaCon;


    // << ------ TODO: Figure out a way to make this more modular to be able to work with all future enemy types


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        crabLarvaCon = GetComponent<CrabwormLarvaController>();
    }

    public void TakeDamage(Vector2 _hitDirection, float _damage, float _knockBack, float _knockUp, float _stunTime)
    {
        crabLarvaCon.currentHP -= _damage;
        print(gameObject.name + " was damaged!");

        StartCoroutine(crabLarvaCon.UnitKnocked(_hitDirection, _knockBack, _knockUp, _stunTime));
        StartCoroutine(IFlashRed());

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
