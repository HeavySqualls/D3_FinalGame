using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : PhysicsObject
{
    public enum State { Patrolling, Hunting, Attacking, Idle, Hurt, InAirInWind, Dead }
    [Space]
    [Header("--- ENEMY BASE ---")]

    public State currentState;
    protected bool isIdle;
    protected bool isPatrolling;
    protected bool isHunting;
    protected bool isDead;
    protected bool isInAirInWind;
    protected bool isHit = false;
    protected bool objectHit = false;
    protected bool isHurt = false;

    [Space]
    [Header("Unit Health:")]
    [SerializeField] public float currentHP;
    [SerializeField] float startHP = 10f;

    [Space]
    [Header("Enemy Combat:")]
    public int damageOutput = 2;
    public float attackCooldown = 0.5f;
    [SerializeField] protected float knockBack;
    [SerializeField] protected float knockUp;
    [SerializeField] protected float stunTime;

    [Space]
    [Header("Enemy Movement:")]
    [SerializeField] protected float patrolMoveSpeed = 2f;
    [SerializeField] protected float huntingMoveSpeed = 5f;
    [SerializeField] protected float attackingMoveSpeed = 10f;
    [SerializeField] private float currentMoveSpeed;
    protected Vector2 move;

    [Space]
    [Header("Raycasts:")]
    [SerializeField] protected Transform eyeRange;
    [SerializeField] protected float eyeRangeDistance = 10f;
    [SerializeField] protected Transform wallDetection;
    [SerializeField] protected float wallDistance = 1f;
    [SerializeField] protected Transform groundDetection;
    [SerializeField] protected float groundDistance = 0.25f;
    protected LayerMask groundLayerMask;
    protected LayerMask playerLayerMask;

    [Space]
    [Header("Respawn Location:")]
    public Transform respawnZone;

    [Space]
    [Header("Enemy References:")]
    protected Animator animator;
    protected GameObject target;

    protected override void Start()
    {
        base.Start();

        currentHP = startHP;
        animator = GetComponent<Animator>();

        groundLayerMask = ((1 << 8) | (1 << 19));
        playerLayerMask = ((1 << 12));

        // Set the enemy in the correct direction at Start
        if (direction == Vector2.zero)
            direction = Vector2.right;
    }

    protected override void Update()
    {
        base.Update();
    }


    // << --------------------------------------- COMBAT -------------------------------- >> //

    public IEnumerator UnitKnocked(Vector2 _hitDirection, float _knockBack, float _knockUp, float _stunTime)
    {
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

    protected IEnumerator AttackCoolDown()
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

        AfterAttackCooldown();

        yield break;
    }

    protected virtual void AfterAttack()
    {
        // Behaviour handled in specific enemy controller
    }

    protected virtual void AfterAttackCooldown()
    {
        // Behaviour handled in specific enemy controller
    }

    protected virtual void KillUnit()
    {
        // Behaviour handled in specific enemy controller
    }


    // << --------------------------------------- MOVEMENT -------------------------------- >> //

    protected override void ComputeVelocity()
    {
        Move();

        if (inWindZone)
        {
            // Wind is moving to the right 
            if (windDir == Vector2.right)
            {
                if (windDir == direction) // if we are moving with the wind direction 
                    currentMoveSpeed -= windDir.x * windPwr;
                else if (windDir != direction)
                    currentMoveSpeed += windDir.x * windPwr;
            }
            // Wind is moving to the left
            else if (windDir == Vector2.left)
            {
                if (windDir != direction) // if we are moving against the wind direction
                    currentMoveSpeed += windDir.x * windPwr;
                else if (windDir == direction) // if we are moving with the wind direction 
                    currentMoveSpeed -= windDir.x * windPwr;
            }
        }
    }

    void Move()
    {
        if (currentState == State.Patrolling)
            currentMoveSpeed = patrolMoveSpeed;
        else if (currentState == State.Hunting)
            currentMoveSpeed = huntingMoveSpeed;
        else if (currentState == State.Attacking)
            currentMoveSpeed = attackingMoveSpeed;
        else if (currentState == State.Idle)
            currentMoveSpeed = 0;

        if (direction == Vector2.right)
            move.x = currentMoveSpeed;
        else
            move.x = -currentMoveSpeed;

        // Sends to computed target velocity to physics object to translate in to movement
        targetVelocity.x = move.x;
    }


    // << ------------------------------------- RAYCAST CHECKS -------------------------------- >> //

    protected void DetectWallCollisions()
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

    protected void DetectGroundCollisions()
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


    // << ------------------------------------- FLIP SPRITE -------------------------------- >> //

    protected void FlipSprite()
    {
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
