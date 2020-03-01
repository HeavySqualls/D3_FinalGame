using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerHealthSystem : MonoBehaviour
{
    [Header("RED FLASH:")]
    [Tooltip("The length of time the object will flash.")]
    public float flashDuration = 5f;
    [SerializeField] float currentFlashDelay;
    [SerializeField] float phase1FlashDelay = 1f;
    [SerializeField] float phase2FlashDelay = 0.5f;
    [SerializeField] float phase3FlashDelay = 0.35f;

    int currentPhase;
    int hurtPhase0 = 0;
    int hurtPhase1 = 1;
    int hurtPhase2 = 2;
    int hurtPhase3 = 3;

    [SerializeField] float playerHealthStart;
    float playerHealth;

    PlayerController pCon;
    PlayerFeedback pFeedBack;
    Animator animator;
    Camera cam;
    Tween shakeTween;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;

        pCon = GetComponent<PlayerController>();
        pFeedBack = GetComponent<PlayerFeedback>();
        animator = GetComponent<Animator>();

        playerHealth = playerHealthStart;
        currentPhase = hurtPhase0;
    }

    // ---- HANDLE DAMAGE ---- //

    public void TakeDamage(Vector2 _hitDirection, float _damage, float _knockBack, float _knockUp, float _stunTime)
    {
        shakeTween = cam.transform.DOShakePosition(0.8f, 0.25f, 9, 2f, false, true);

        if (currentPhase == hurtPhase0)
        {
            currentPhase = hurtPhase1;
            currentFlashDelay = phase1FlashDelay;
        }
        else if (currentPhase == hurtPhase1)
        {
            currentPhase = hurtPhase2;
            currentFlashDelay = phase2FlashDelay;

            StopCoroutine(IFlashRed());
            StartCoroutine(IFlashRed());
        }
        else if (currentPhase == hurtPhase2)
        {
            currentPhase = hurtPhase3;
            currentFlashDelay = phase3FlashDelay;

            StopCoroutine(IFlashRed());
            StartCoroutine(IFlashRed());
        }

        playerHealth -= _damage;

        animator.SetTrigger("isHurt");

        if (!pCon.isHit)
        {
            StartCoroutine(pCon.PlayerKnocked(_hitDirection, _knockBack, _knockUp, _stunTime));
        }
    }

    public IEnumerator IFlashRed()
    {
        for (float i = 0; i < flashDuration; i += currentFlashDelay)
        {
            if (spriteRenderer.color == Color.white)
            {
                spriteRenderer.color = Color.red;
            }
            else if (spriteRenderer.color == Color.red)
            {
                spriteRenderer.color = Color.white;
            }

            yield return new WaitForSeconds(currentFlashDelay);
        }

        if (currentPhase == hurtPhase3)
        {
            currentPhase = hurtPhase2;
            currentFlashDelay = phase2FlashDelay;

            for (float i = 0; i < flashDuration; i += currentFlashDelay)
            {
                if (spriteRenderer.color == Color.white)
                {
                    spriteRenderer.color = Color.red;
                }
                else if (spriteRenderer.color == Color.red)
                {
                    spriteRenderer.color = Color.white;
                }

                yield return new WaitForSeconds(currentFlashDelay);
            }
        }

        if (currentPhase == hurtPhase2)
        {
            currentPhase = hurtPhase1;
            currentFlashDelay = phase1FlashDelay;

            for (float i = 0; i < flashDuration; i += currentFlashDelay)
            {
                if (spriteRenderer.color == Color.white)
                {
                    spriteRenderer.color = Color.red;
                }
                else if (spriteRenderer.color == Color.red)
                {
                    spriteRenderer.color = Color.white;
                }

                yield return new WaitForSeconds(currentFlashDelay);
            }
        }

        currentPhase = 0;
        currentFlashDelay = 0;
        spriteRenderer.color = Color.white;

        yield break;
    }
}
