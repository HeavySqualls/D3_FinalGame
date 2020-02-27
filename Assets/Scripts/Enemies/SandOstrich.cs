using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandOstrich : Enemy_Turret_Base
{
    private bool isPreBurp = false;
    public float damageDelay = 1f;
    public bool isDamageDelay = false;
    public bool isStartInHole = false;
    public bool isInHole = false;
    public float holeRadius;
    public float idleRadius;

    [SerializeField] RecieveDamage pRecieveDamage;
    [SerializeField] ParticleSystem partSyst;
    CircleCollider2D circleCollider;

    protected override void Start()
    {
        base.Start();
        currentState = State.Idle;

        circleCollider = GetComponent<CircleCollider2D>();
        partSyst = GetComponentInChildren<ParticleSystem>();

        if (isStartInHole)
        {
            animator.SetTrigger("isGoingInHole");
            isInHole = true;
        }
        else
        {
            isInHole = false;
        }
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

        if (isTracking && !isDead)
        {
            TrackTargetDistance();
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
        DetectPlayerForFireAttack();
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
        partSyst.Stop();
        currentState = State.Hurt;
        animator.SetTrigger("isHit");
        StartCoroutine(AttackCoolDown());
    }

    public void DealDamage()
    {
        print(gameObject.name + " has hit " + pRecieveDamage.name);

        pRecieveDamage.GetHit(direction, damageOutput, knockBack, knockUp, stunTime);
        StartCoroutine(DamageDelay());
        isDamageDelay = true;
    }

    public Transform hitBoxPos;

    public void CastForPlayer() // Called every frame when in Attacking state 
    {
        if (!objectHit)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(hitBoxPos.position, 0.5f, direction, 0.5f, playerLayerMask);
            foreach (RaycastHit2D hit in hits)
            {
                RecieveDamage hitObj = hit.collider.gameObject.GetComponent<RecieveDamage>();

                if (hitObj != null)
                {
                    print("hit: " + hitObj.name);

                    hitObj.GetComponent<RecieveDamage>().GetHit(direction, damageOutput, knockBack, knockUp, stunTime);
                    objectHit = true; // prevents enemy attacking the player every frame - gets reset in AfterAttackCooldown()
                    AfterThisUnitWasAttacked();
                }
                else
                {
                    Debug.Log("No Recieve Damage component on player!");
                }
            }
        }
    }

    protected override void KillUnit()
    {
        base.KillUnit();

        print("Die");

        isIdle = false;
        isPreBurp = false;
        isDead = true;

        currentState = State.Dead;
        Destroy(gameObject, 5f);
    }

    // << ------------------------------------- RAYCAST CHECKS -------------------------------- >> //

    void DetectPlayerForFireAttack()
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

    bool isTracking = false;
    [SerializeField] float distanceToTarget;
    [SerializeField] float burpDistance;
    [SerializeField] float flameDistance;

    private void TrackTargetDistance()
    {
        distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToTarget <= flameDistance)
        {
            currentState = State.Attacking;
        }
        else if (distanceToTarget <= burpDistance)
        {
            currentState = State.Prone;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        pRecieveDamage = other.GetComponent<RecieveDamage>();

        if (pRecieveDamage != null)
        {
            if (isInHole)
            {
                target = pRecieveDamage.gameObject;
                isInHole = false;
                animator.SetTrigger("isExitingHole");
                isTracking = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        pRecieveDamage = other.GetComponent<RecieveDamage>();

        if (pRecieveDamage != null)
        {
            print("go in hole");
            isTracking = false;
            isInHole = true;
            pRecieveDamage = null;
            target = null;
            animator.SetTrigger("isGoingInHole");
        }
    }
}
