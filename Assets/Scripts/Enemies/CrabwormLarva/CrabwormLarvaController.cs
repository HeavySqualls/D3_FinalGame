using UnityEngine;

public class CrabwormLarvaController : Enemy_Base
{
    /// <summary>
    /// 
    /// - Actual movement of the unit is handled in PhysicsObject and calculated in Enemy_Base.
    /// 
    /// - Each enemy type controls its own states and player hunting logic.
    /// 
    /// - Detection is also individually controlled by the separate states. 
    /// 
    /// </summary>   


    [Space]
    [Header("--- CRABWORM LARVA ---")]

    [Tooltip("The location of the units hit box - activated when attacking.")]
    [SerializeField] Transform hitBoxPos;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (inWindZone && !isGrounded)
        {
            this.currentState = State.InAirInWind;
        }

        switch (this.currentState)
        {
            case State.Patrolling:
                this.Patrolling();
                break;
            case State.Hunting:
                this.Hunting();
                break;
            case State.Attacking:
                this.Attacking();
                break;
            case State.Idle:
                this.Idle();
                break;
            case State.Hurt:
                this.Hurt();
                break;
            case State.InAirInWind:
                this.InAirInWind();
                break;
            case State.Dead:
                this.Dead();
                break;
        }

        Animations();
    }


    // << --------------------------------------- STATES -------------------------------- >> //

    private void Patrolling()
    {
        DetectWallCollisions();
        DetectGroundCollisions();
        DetectPlayerCollisions();
        isIdle = false;
        isPatrolling = true;
        isHunting = false;
    }

    private void Hunting()
    {
        DetectWallCollisions();
        DetectGroundCollisions();
        DetectPlayerCollisions();
        isIdle = false;
        isPatrolling = false;
        isHunting = true;
    }

    private void Attacking()
    {
        DetectWallCollisions();
        DetectGroundCollisions();
        CastForPlayer();
        isIdle = false;
        isPatrolling = false;
        isHunting = false;
    }
    
    private void Idle()        // Do nothing
    {
        DetectWallCollisions();
        DetectGroundCollisions();
        DetectPlayerCollisions();
        isIdle = true;
        isPatrolling = false;
        isHunting = false;
    }

    private void Hurt()        // Do nothing
    {
        isIdle = false;
        isPatrolling = false;
        isHunting = false;
    }

    private void InAirInWind()        // Do nothing
    {

    }

    private void Dead()         // Wait to die
    {
        isIdle = false;
        isPatrolling = false;
        isHunting = false;
        targetVelocity.x = 0;
    }


    // << --------------------------------------- ANIMATIONS -------------------------------- >> //

    void Animations()
    {
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isPatrolling", isPatrolling);
        animator.SetBool("isHunting", isHunting);
        animator.SetBool("isHurt", isHurt);
        animator.SetBool("isDead", isDead);
    }


    // << --------------------------------------- COMBAT -------------------------------- >> //

    protected override void AfterThisUnitWasAttacked()
    {
        base.AfterThisUnitWasAttacked();

        currentState = State.Idle;
        StartCoroutine(AttackCoolDown());
    }

    protected override void AfterAttackCooldown()
    {
        base.AfterAttackCooldown();

        currentState = State.Hunting;
    }

    protected override void KillUnit()
    {
        base.KillUnit();
        isDead = true;
        currentState = State.Dead;
        Destroy(gameObject, 10f);
    }


    // << ------------------------------------- RAYCAST CHECKS -------------------------------- >> //

    void DetectPlayerCollisions()
    {
        RaycastHit2D huntingInfo = Physics2D.Raycast(eyeRange.position, direction, eyeRangeDistance, playerLayerMask);

        // If we have detected the player and are not currently attacking them
        if (huntingInfo.collider != null && currentState != State.Attacking)
        {
            target = huntingInfo.collider.gameObject;
            currentState = State.Hunting;

            RaycastHit2D attackInfo = Physics2D.Raycast(eyeRange.position, direction, eyeRangeDistance / 2, playerLayerMask);

            if (attackInfo.collider != null)
            {
                isPatrolling = false;
                currentState = State.Attacking;
                animator.SetTrigger("isAttacking");
            }
        }
    }

    public void AttackCall(GameObject _target)
    {
        target = _target;

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

        currentState = State.Attacking;
        animator.SetTrigger("isAttacking");
    }

    void OnDrawGizmos()
    {
        // Hit Box
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitBoxPos.position, 0.35f);

        // Sight Lines
        Debug.DrawRay(eyeRange.position, direction * eyeRangeDistance, Color.yellow);
        Debug.DrawRay(eyeRange.position, direction * eyeRangeDistance / 2, Color.red);
        Debug.DrawRay(wallDetection.position, direction * wallDistance, Color.white);
        Debug.DrawRay(groundDetection.position, Vector2.down * groundDistance, Color.red);
    }


    // << ------------------------------------- COLLISION CHECKS -------------------------------- >> //

    public void CastForPlayer() // Called every frame when in Attacking state 
    {
        if (!objectHit)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(hitBoxPos.position, 0.35f, direction, 0f, playerLayerMask);
            foreach (RaycastHit2D hit in hits)
            {
                RecieveDamage hitObj = hit.collider.gameObject.GetComponent<RecieveDamage>();

                if (hitObj != null)
                {
                    print("hit: " + hitObj.name);

                    hitObj.GetComponent<RecieveDamage>().GetHit(direction, damageOutput, knockBack, knockUp, stunTime);
                    objectHit = true; // prevents enemy attacking the player every frame - gets reset in AfterAttackCooldown()
                }
                else
                {
                    Debug.Log("No Recieve Damage component on player!");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController pCon = collision.gameObject.GetComponent<PlayerController>();

        if (pCon != null && pCon.magBootsOn)
        {
            KillUnit();
        }
    }
}
