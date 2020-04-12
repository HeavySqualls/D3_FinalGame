using System.Collections;
using UnityEngine;

public class SandOstrich : Enemy_Turret_Base
{
    [Space]
    [Header("--- SAND OSTRICH ---")]

    [Space]
    [Header("States: ")]
    public bool isDamageDelay = false;
    public bool isStartInHole = false;
    public bool isInHole = false;
    private bool isPreBurp = false;
    private bool isTracking = false;

    [Space]
    [Header("Combat: ")]
    [Tooltip("The transform of the empty game object used to position the units hit box.")]
    public Transform hitBoxPos;
    [Tooltip("The time in between getting hit, which the player can get hurt again by this units attack.")]
    [SerializeField] float damageDelay = 1f;
    [Tooltip("The current distance the player is to the unit.")]
    [SerializeField] float distanceToTarget;
    [Tooltip("The distanceToTarget distance before the unit will enter its burp animation phase.")]
    [SerializeField] float burpDistance;
    [Tooltip("The distanceToTarget distance before the unit will enter its flame attack animation phase.")]
    [SerializeField] float flameDistance;


    [Space]
    [Header("References: ")]
    RecieveDamage pRecieveDamage;
    ParticleSystem partSyst;

    protected override void Start()
    {
        base.Start();

        currentState = State.Idle;
        partSyst = GetComponentInChildren<ParticleSystem>();

        if (isStartInHole)
        {
            animator.SetTrigger("isGoingInHole");
            isInHole = true;
        }
        else
            isInHole = false;
    }

    public void SetToIdle() // used by the cinematic trigger controller 
    {
        Destroy(gameObject);
        print("Hey!");
        //currentState = State.Idle;
        //isIdle = false;
        //isInHole = true;
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
        // Do nothing and wait for state change
    }

    private void Dead()
    {
        // Do nothing and wait to be destroyed
    }


    // << --------------------------------------- COMBAT -------------------------------- >> //

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

    public void ShootFlames() // called from inside the animator (for now)
    {
        if (!isTurretPaused)
        {
            partSyst.Play();
        }
    }

    public void DealDamage()
    {
        print(gameObject.name + " has hit " + pRecieveDamage.name);

        if (pRecieveDamage != null)
            pRecieveDamage.GetHit(direction, damageOutput, knockBack, knockUp, stunTime);

        StartCoroutine(DamageDelay());
        isDamageDelay = true;
    }

    IEnumerator DamageDelay()
    {
        yield return new WaitForSeconds(damageDelay);

        isDamageDelay = false;
    }

    protected override void AfterThisUnitWasAttacked()
    {
        base.AfterThisUnitWasAttacked();
        partSyst.Stop();
        currentState = State.Hurt;
        animator.SetTrigger("isHit");
        StartCoroutine(AttackCoolDown());
    }

    protected override void KillUnit()
    {
        base.KillUnit();

        print("Die");

        isIdle = false;
        isPreBurp = false;
        isDead = true;

        currentState = State.Dead;
        //gameObject.SetActive(false);
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
