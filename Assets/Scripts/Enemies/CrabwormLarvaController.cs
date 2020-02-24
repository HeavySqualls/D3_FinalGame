using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabwormLarvaController : PhysicsObject
{
    public enum State { Patrolling, Hunting, Attacking, Idle, Hurt, InAirInWind, Dead}

    [Space]
    [Header("Enemy States:")]
    public State currentState;
    private bool isIdle;
    private bool isPatrolling;
    private bool isHunting;
    private bool isDead;
    private bool isInAirInWind;
    private bool isHit = false;
    private bool objectHit = false;
    private bool isHurt = false;

    [Space]
    [Header("Unit Health:")]
    public float currentHP;
    [SerializeField] float startHP = 10f;

    [Space]
    [Header("Enemy Combat:")]
    [SerializeField] Transform hitboxPos;
    public int damageOutput = 2;
    public float attackCooldown = 0.5f;
    [SerializeField] float knockBack;
    [SerializeField] float knockUp;
    [SerializeField] float stunTime;
    [SerializeField] public float knockbackTimeLength;
    [SerializeField] float currentKnockBackTime;

    [Space]
    [Header("Enemy Movement:")]
    public float patrolSpeed = 1.25f;
    [SerializeField] float baseMoveSpeed = 2f;
    [SerializeField] float huntingMoveSpeed = 5f;
    [SerializeField] float attackingMoveSpeed = 10f;
    private float maxMoveSpeed;
    private float minMoveSpeed;

    [Space]
    [Header("Enemy AI:")]
    public float pauseTime = 2f;
    [SerializeField] private Vector2 move;
    [SerializeField] private Vector2 hitDirection;

    [Space]
    [Header("Raycasts:")]
    [SerializeField] Transform eyeRange;
    [SerializeField] float eyeRangeDistance = 10f;
    [SerializeField] Transform wallDetection;
    [SerializeField] float wallDistance = 1f;
    [SerializeField] Transform groundDetection;
    [SerializeField] float groundDistance = 0.25f;
    LayerMask groundLayerMask;
    LayerMask playerLayerMask;

    [Space]
    [Header("Respawn Location:")]
    public Transform respawnZone;

    [Space]
    [Header("Enemy References:")]
    private Animator animator;
    private GameObject target;
    private MeshRenderer mR;
    private BoxCollider2D coll;

    protected override void Start()
    {
        base.Start();

        mR = GetComponent<MeshRenderer>();
        coll = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        currentHP = startHP;
        currentKnockBackTime = knockbackTimeLength;

        groundLayerMask = ((1 << 8) | (1<< 19));
        playerLayerMask = ((1 << 12));

        // Set the enemy in the correct direction at Start
        if (direction == Vector2.zero)
            direction = Vector2.right;

        this.currentState = State.Patrolling;
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

    // NOTES: - Actual movement of the unit is handled in PhysicsObject, but each state controls 
    //          how that movement is sent to the physics object.
    //        - Detection is also individually controlled by the separate states. 

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
    
    private void Idle()
    {
        isIdle = true;
        isPatrolling = false;
        isHunting = false;
    }

    private void Hurt()
    {
        // Do nothing
        isIdle = false;
        isPatrolling = false;
        isHunting = false;
    }

    private void InAirInWind()
    {

    }

    private void Dead()
    {
        isIdle = false;
        isPatrolling = false;
        isHunting = false;
        targetVelocity.x = 0;
        // Wait to die
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

    public IEnumerator UnitKnocked(Vector2 _hitDirection, float _knockBack, float _knockUp, float _stunTime)
    {
        print(gameObject.name + " start coroutine!");
        isHurt = true;
        targetVelocity.x = 0;

        currentState = State.Hurt;

        float timer = 0.0f;

        while (timer < _stunTime)
        {
            velocity.y += _knockUp;
            targetVelocity.x += _knockBack;
            timer += Time.deltaTime;
            yield return null;
        }

        isHurt = false;

        if (currentHP <= 0)
            KillUnit();
        else
            AfterAttack();

        yield break;
    }

    public void AfterAttack()
    {
        currentState = State.Idle;
        StartCoroutine(AfterAttackCoolDown());
    }

    IEnumerator AfterAttackCoolDown()
    {
        yield return new WaitForSeconds(attackCooldown);

        Vector2 targetDirection;

        if (target != null)
        {
            targetDirection = gameObject.transform.position - target.transform.position;

            if (direction != targetDirection)
            {
                FlipSprite();
            }
        }

        objectHit = false;
        currentState = State.Hunting;

        yield break;
    }

    public void KillUnit()
    {
        isDead = true;
        currentState = State.Dead;
        Destroy(gameObject, 10f);
    }


    // << --------------------------------------- MOVEMENT -------------------------------- >> //

    protected override void ComputeVelocity()
    {
        if (inWindZone)
        {
            // Wind is moving to the right 
            if (windDir == Vector2.right)
            {
                if (windDir == direction) // if we are moving with the wind direction 
                    baseMoveSpeed += windDir.x * windPwr;
                else if (windDir != direction)
                    baseMoveSpeed -= windDir.x * windPwr;
            }
            // Wind is moving to the left
            else if (windDir == Vector2.left)
            {
                if (windDir != direction) // if we are moving against the wind direction
                    baseMoveSpeed -= windDir.x * windPwr;
                else if (windDir == direction) // if we are moving with the wind direction 
                    baseMoveSpeed += windDir.x * windPwr;
            }

            // limit the max base move speed of the unit in wind
            if (baseMoveSpeed > maxMoveSpeed)
                baseMoveSpeed = maxMoveSpeed;
            // limit the min move speed of the unit in wind
            else if (baseMoveSpeed < minMoveSpeed)
                baseMoveSpeed = minMoveSpeed;
        }

        Move();
    }

    void Move()
    {
        if (currentState == State.Patrolling)
        {
            if (direction == Vector2.right)
                move.x = baseMoveSpeed;
            else
                move.x = -baseMoveSpeed;
        }
        else if (currentState == State.Hunting)
        {
            if (direction == Vector2.right)
                move.x = huntingMoveSpeed;
            else
                move.x = -huntingMoveSpeed;
        }
        else if (currentState == State.Attacking)
        {
            if (direction == Vector2.right)
                move.x = attackingMoveSpeed;
            else
                move.x = -attackingMoveSpeed; 
        }
        else if (currentState == State.Idle)
        {
            move.x = 0;
        }

        // Sends to computed target velocity to physics object to translate in to movement
        targetVelocity.x = move.x;
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

    void DetectWallCollisions()
    {
        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, direction, wallDistance, groundLayerMask);

        // If we have detected a wall && we are NOT detecting a trigger zone
        if (wallInfo.collider != null && !wallInfo.collider.isTrigger)
        {
            CrumblingWall crumblingWall = wallInfo.collider.gameObject.GetComponent<CrumblingWall>();

            if (crumblingWall != null)
            {
                crumblingWall.TriggerPlatformCollapse();                    
            }

            FlipSprite();

            if (currentState == State.Hunting)
            {
                currentState = State.Patrolling;
                target = null;
            }
        }
    }

    void DetectGroundCollisions()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, groundDistance, groundLayerMask);

        // Checks for a loss of vertical collision (end of platform)
        if (!groundInfo.collider && this.currentState != State.Attacking)
        {
            FlipSprite();

            if (currentState == State.Hunting)
            {
                currentState = State.Patrolling;
                target = null;
            }
        }
    }

    void OnDrawGizmos()
    {
        // Hit Box
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitboxPos.position, 0.35f);

        // Sight Lines
        Debug.DrawRay(eyeRange.position, direction * eyeRangeDistance, Color.yellow);
        Debug.DrawRay(eyeRange.position, direction * eyeRangeDistance / 2, Color.red);
        Debug.DrawRay(wallDetection.position, direction * wallDistance, Color.white);
        Debug.DrawRay(groundDetection.position, Vector2.down * groundDistance, Color.red);
    }


    // << ------------------------------------- COLLISION CHECKS -------------------------------- >> //

    // Called every frame when in Attacking state 
    public void CastForPlayer()
    {
        if (!objectHit)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(hitboxPos.position, 0.35f, direction, 0f, playerLayerMask);
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

    //TODO: why can I not use this without it being detected by physics object regardless of layer masking??

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    PlayerController pCon = other.gameObject.GetComponent<PlayerController>();

    //    if (pCon != null)
    //    {
    //        currentState = State.Attacking;
    //        animator.SetTrigger("isAttacking");
    //        target = pCon.gameObject;

    //        Vector2 targetDirection = gameObject.transform.position - target.transform.position;

    //        if (direction == targetDirection)
    //        {
    //            FlipSprite();
    //        }
    //    }
    //}

    //void OnTriggerExit2D(Collider2D other)
    //{
    //    PlayerController pCon = other.gameObject.GetComponent<PlayerController>();

    //    if (pCon != null)
    //    {
    //        target = null;
    //    }
    //}


    // << ------------------------------------- FLIP SPRITE -------------------------------- >> //

    private void FlipSprite()
    {
        print("Enemy flip sprite");

        if (direction == Vector2.right)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            direction = Vector2.left;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            direction = Vector2.right;
        }
    }
}
