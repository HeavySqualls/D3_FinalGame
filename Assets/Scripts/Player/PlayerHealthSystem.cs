﻿using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerHealthSystem : MonoBehaviour
{
    [Header("Health Variables:")]
    [SerializeField] float playerHealthStart;
    float playerHealth;

    [Space]
    [Header("Red Flash:")]
    [Tooltip("The length of time the object will flash.")]
    public float flashDuration = 5f;
    [SerializeField] float currentFlashDelay;
    [SerializeField] float phase1FlashDelay = 1f;
    [SerializeField] float phase2FlashDelay = 0.5f;
    [SerializeField] float phase3FlashDelay = 0.35f;
    private IEnumerator flashCoroutine;

    [Space]
    [Header("Injury Phases:")]
    [SerializeField] int currentPhase;
    int hurtPhase0 = 0;
    int hurtPhase1 = 1;
    int hurtPhase2 = 2;
    int hurtPhase3 = 3;

    [Space]
    [Header("Respawn:")]
    public SpawnZone spawnZone;
    public float respawnTime;
    [SerializeField] bool isBeingChased = false;
    public bool isDead = false;

    [Space]
    [Header("References:")]
    PlayerController pCon;
    Animator animator;
    Camera cam;
    Tween shakeTween;
    SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        SpawnManager.onResetLevelObjects += RespawnPlayer;
    }

    private void OnDisable()
    {
        SpawnManager.onResetLevelObjects -= RespawnPlayer;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;

        pCon = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();

        playerHealth = playerHealthStart;
        currentPhase = hurtPhase0;

        flashCoroutine = IFlashRed();
    }


    // ---- HANDLE DAMAGE ---- //

    public void TakeDamage(Vector2 _hitDirection, float _damage, float _knockBack, float _knockUp, float _stunTime)
    {
        if (!isDead)
        {
            shakeTween = cam.transform.DOShakePosition(0.8f, 0.25f, 9, 2f, false, true);
            StopCoroutine("IFlashRed");

            if (currentPhase == hurtPhase0)
            {
                print("Phase 1");
                currentPhase = hurtPhase1;
                currentFlashDelay = phase1FlashDelay;
            }
            else if (currentPhase == hurtPhase1)
            {
                print("Phase 2");
                currentPhase = hurtPhase2;
                currentFlashDelay = phase2FlashDelay;
            }
            else if (currentPhase == hurtPhase2)
            {
                print("Phase 3");
                currentPhase = hurtPhase3;
                currentFlashDelay = phase3FlashDelay;
            }
            else if (currentPhase == hurtPhase3)
            {
                print("PLAYER IS DEAD!");
                KillPlayer();
                return;
            }

            StartCoroutine("IFlashRed");

            playerHealth -= _damage;

            animator.SetTrigger("isHurt");

            if (!pCon.isHit)
            {
                StartCoroutine(pCon.PlayerKnocked(_hitDirection, _knockBack, _knockUp, _stunTime));
            }
        }    
    }

    public void KillPlayer()
    {
        print("player killed");
        isDead = true;
        pCon.animator.SetBool("isDead", isDead);
        pCon.DisablePlayerController();

        StopCoroutine("IFlashRed");
        spriteRenderer.color = Color.white;
        //StartCoroutine(RespawnCountdown());
        SpawnManager.PlayerWasKilled();
    }

    public void RespawnPlayer()
    { 
        isDead = false;
        pCon.animator.SetBool("isDead", isDead);

        if (spawnZone != null)
        {
            print("spawn player");
            spawnZone.RespawnObject(gameObject);
        }
        else

            Debug.LogError("No respawn location assigned!");

        pCon.EnablePlayerController();
        playerHealth = playerHealthStart;
        currentPhase = hurtPhase0;
    }

    //private IEnumerator RespawnCountdown()
    //{
    //    yield return new WaitForSeconds(respawnTime);

    //    isDead = false;
    //    pCon.animator.SetBool("isDead", isDead);
    //    pCon.DisablePlayerController();

    //    if (spawnZone != null)
    //    {
    //        print("spawn player");
    //        spawnZone.RespawnObject(gameObject);
    //    }
    //    else
    //    {
    //        Debug.LogError("No respawn location assigned!");
    //    }

    //    pCon.EnablePlayerController();
    //    playerHealth = playerHealthStart;
    //    currentPhase = hurtPhase0;
    //}

    public IEnumerator IFlashRed()
    {
        print("Flash");

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
            print("Phase 3 to 2");
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
            print("Phase 2 to 1");
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

        spriteRenderer.color = Color.white;

        yield break;
    }
}
