using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabwormLarva : PhysicsObject
{
    public enum State { Patrolling, Attacking, Idle, Hurt, InAirInWind, Dead}

    [Space]
    [Header("Enemy States:")]
    public State currentState;
    private bool isIdle;
    private bool isPatrolling;
    private bool isInAirInWind;
    //private bool isMovingRight = true;
    private bool isHit = false;

    [Space]
    [Header("Enemy Combat:")]
    public int damageOutput = 2;
    public float cooldown;
    public float currentHP;
    [SerializeField] float startHP = 10f;
    [SerializeField] private float knockBack;
    [SerializeField] private float knockUp;
    [SerializeField] private float stunTime;
    public float knockbackTimeLength;
    [SerializeField] float currentKnockBackTime;

    [Space]
    [Header("Enemy Movement:")]
    public float patrolSpeed = 1.25f;
    [SerializeField] float baseMoveSpeed;
    [SerializeField] float baseMoveSpeedStart = 50f;
    [SerializeField] float maxMoveSpeed;
    [SerializeField] float minMoveSpeed;

    [Space]
    [Header("Enemy AI:")]
    public float pauseTime = 2f;
    private float detectionRange = 8f;
    private float wallDistance = 1f;
    private float groundDistance = 0.25f;
    [SerializeField] private Vector2 move;
    //[SerializeField] private Vector2 newDirection;
    [SerializeField] private Vector2 hitDirection;

    [Space]
    [Header("Ray casts:")]
    public Transform eyeRange;
    public Transform wallDetection;
    public Transform groundDetection;
    LayerMask groundLayerMask;

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

        baseMoveSpeed = baseMoveSpeedStart;
        currentKnockBackTime = knockbackTimeLength;
        currentHP = startHP;

        groundLayerMask = ((1 << 8));

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


    // << --------------------------------------- STATES -------------------------------->> //

    private void Patrolling()
    {
        DetectWallCollisions();
        DetectGroundCollisions();
        Move();
        isPatrolling = true;
    }

    private void Attacking()
    {
        // lunges at player 
        Move();
    }
    
    private void Idle()
    {

    }

    private void Hurt()
    {

    }

    private void InAirInWind()
    {

    }

    private void Dead()
    {

    }


    // << --------------------------------------- ANIMATIONS -------------------------------->> //
    void Animations()
    {
        animator.SetBool("isPatrolling", isPatrolling);
        animator.SetBool("isIdle", isIdle);
    }


    // << --------------------------------------- MOVEMENT -------------------------------->> //

    void Move()
    {
        ComputeBaseMoveSpeed();

        if (isGrounded && currentState == State.Patrolling)
        {
            if (direction == Vector2.right)
                move.x = baseMoveSpeed;
            else
                move.x = -baseMoveSpeed;
        }
        else if (currentState == State.Attacking)
        {
            if (direction == Vector2.right)
                move.x = baseMoveSpeed * 100;
            else
                move.x = -baseMoveSpeed * 100;
        }

        targetVelocity.x = move.x * Time.deltaTime;
    }

    void ComputeBaseMoveSpeed()
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

            // limit the max base move speed of the enemy
            if (baseMoveSpeed > maxMoveSpeed)
                baseMoveSpeed = maxMoveSpeed;
            // limit the min move speed of the unit
            else if (baseMoveSpeed < minMoveSpeed)
                baseMoveSpeed = minMoveSpeed;
        }
        else
        {
            baseMoveSpeed = baseMoveSpeedStart;
        }
    }


    // << ------------------------------------- RAYCAST CHECKS -------------------------------->> //

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController pCon = other.gameObject.GetComponent<PlayerController>();

        if (pCon != null)
        {
            currentState = State.Attacking;
            animator.SetTrigger("isAttacking");
            target = pCon.gameObject;

            Vector2 targetDirection = gameObject.transform.position - target.transform.position;

            if (direction == targetDirection)
            {
                FlipSprite();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        PlayerController pCon = other.gameObject.GetComponent<PlayerController>();

        if (pCon != null)
        {
            target = null;
        }
    }

    void DetectWallCollisions()
    {
        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, direction, wallDistance, groundLayerMask);
        Debug.DrawRay(wallDetection.position, direction * wallDistance, Color.white);

        // If moving right, raycast right - if moving left, raycast left
        if (direction == Vector2.right)
            direction = Vector2.right; 
        else
            direction = Vector2.left;

        // If we have detected a wall && we are NOT detecting a trigger zone
        if (wallInfo.collider != null && !wallInfo.collider.isTrigger)
        {
            FlipSprite();
        }
    }

    void DetectGroundCollisions()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, groundDistance, groundLayerMask);
        Debug.DrawRay(groundDetection.position, Vector2.down * groundDistance, Color.red);

        // Checks for a loss of vertical collision (end of platform)
        if (!groundInfo.collider.isTrigger)
        {
            if (!groundInfo.collider && this.currentState != State.Attacking)
            {
                isGrounded = false;
                FlipSprite();
            }
            else
            {
                isGrounded = true;
            }
        }

    }


    // << ------------------------------------- FLIP SPRITE -------------------------------->> //

    private void FlipSprite()
    {
        print("Enemy flip sprite");

        if (direction == Vector2.right)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            direction = Vector2.left;
            //isMovingRight = false;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            direction = Vector2.right;
            //isMovingRight = true;
        }
    }
}
