using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandOstrich : Enemy_Turret_Base
{
    private bool isPreBurp = false;
    public float damageDelay;
    private bool isDamageDelay = false;

    ParticleSystem partSyst;

    protected override void Start()
    {
        base.Start();
        currentState = State.Idle;

        partSyst = GetComponentInChildren<ParticleSystem>();
    }

    protected override void Update()
    {
        base.Update();

        switch (this.currentState)
        {
            case State.Idle:
                this.Idle();
                break;
            case State.Prone:
                this.Prone();
                break;
            case State.Attacking:
                this.Attacking();
                break;
            case State.Hurt:
                this.Hurt();
                break;
            case State.Dead:
                this.Dead();
                break;
        }

        Animations();
    }


    // << --------------------------------------- ANIMATIONS -------------------------------- >> //

    private void Animations()
    {
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isPreBurp", isPreBurp);
        animator.SetBool("isDead", isDead);
    }


    // << --------------------------------------- STATES -------------------------------- >> //

    private void Idle()
    {
        isIdle = true;
        isPreBurp = false;
    }

    private void Prone()
    {
        isIdle = false;
        isPreBurp = true;
        FaceTarget();
        DetectPlayerCollisions();
    }

    private void Attacking()
    {
        FaceTarget();

        if (!isCooldown)
        {
            StartCoroutine(AttackCoolDown());
            isCooldown = true;
        }
    }

    private void Hurt()
    {

    }

    private void Dead()
    {
        
    }


    // << --------------------------------------- COMBAT -------------------------------- >> //

    public void ShootFlames() // called from inside the animator (for now)
    {
        partSyst.Play();
    }

    void OnParticleCollision(GameObject other)
    {
        RecieveDamage pRecieveDamage = other.GetComponent<RecieveDamage>();

        if (pRecieveDamage != null && !isDamageDelay)
        {
            isDamageDelay = true;
            pRecieveDamage.GetHit(direction, damageOutput, knockBack, knockUp, stunTime);
            StartCoroutine(DamageDelay());
        }
    }

    IEnumerator DamageDelay()
    {
        yield return new WaitForSeconds(damageDelay);

        isDamageDelay = false;
    }

    private void FaceTarget()
    {
        Vector2 targetDirection;

        if (target != null)
        {
            targetDirection = (transform.position - target.transform.position);
            targetDirection.x = Mathf.Clamp(targetDirection.x, -1f, 1f);

            if (targetDirection.x > 0 && direction == Vector2.right || targetDirection.x < 0 && direction == Vector2.left)
            {
                FlipSprite();
            }
        }
    }

    protected override void AfterThisUnitWasAttacked()
    {
        base.AfterThisUnitWasAttacked();

        currentState = State.Hurt;
        animator.SetTrigger("isHit");
        StartCoroutine(AttackCoolDown());
    }


    // << ------------------------------------- RAYCAST CHECKS -------------------------------- >> //

    void DetectPlayerCollisions()
    {
        RaycastHit2D huntingInfo = Physics2D.Raycast(eyeRange.position, direction, eyeRangeDistance, playerLayerMask);
        Debug.DrawRay(eyeRange.position, direction * eyeRangeDistance, Color.yellow);

        // If we have detected the player and are not currently attacking them
        if (huntingInfo.collider != null)
        {
            currentState = State.Attacking;
            animator.SetTrigger("isFlameBurp");
        }
    }


    // << ------------------------------------- COLLISION CHECKS -------------------------------- >> //

    private void OnTriggerEnter2D(Collider2D other)
    {
        RecieveDamage pRecieveDamage = other.GetComponent<RecieveDamage>();

        if (pRecieveDamage != null)
        {
            currentState = State.Prone;
            target = pRecieveDamage.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        RecieveDamage pRecieveDamage = other.GetComponent<RecieveDamage>();

        if (pRecieveDamage != null)
        {
            currentState = State.Idle;
            target = null;
        }
    }
}
