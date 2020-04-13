using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : PhysicsObject
{
    public enum State { Patrolling, Hunting, Attacking, Idle, Hurt, InAirInWind, Dead }
    [Space]
    [Header("--- ENEMY BASE ---")]

    public State startingState;
    protected State currentState;
    private Vector3 startingPos;

    public bool isUnitPaused = false;
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

    [Tooltip("The current amount of health on the unit.")]
    [SerializeField] public float currentHP;
    [Tooltip("The starting health of the unit.")]
    [SerializeField] float startHP = 10f;

    [Space]
    [Header("Enemy Combat:")]

    [Tooltip("The damage this enemy will cause to the player.")]
    public int damageOutput = 2;
    [Tooltip("The time between attacks on the player.")]
    public float attackCooldown = 0.5f;
    [Tooltip("The distance the player will be knocked back when hit by the player. (total distance is relative to the duration of the stun time variable)")]
    [SerializeField] protected float knockBack;
    [Tooltip("The distance the player will be knocked in tp the air when hit by this unit. (total distance is relative to the duration of the stun time variable)")]
    [SerializeField] protected float knockUp;
    [Tooltip("The amount of time the player will be stunned when hit by the unit.")]
    [SerializeField] protected float stunTime;

    [Space]
    [Header("Enemy Movement:")]

    [Tooltip("The speed that the enemy will move when in the Patrol state.")]
    [SerializeField] protected float patrolMoveSpeed = 2f;
    [Tooltip("The speed that the enemy will move when in the Hunting state.")]
    [SerializeField] protected float huntingMoveSpeed = 5f;
    [Tooltip("The speed that the enemy will move when in the Attacking state.")]
    [SerializeField] protected float attackingMoveSpeed = 10f;
    [Tooltip("The current speed that the enemy is moving at.")]
    [SerializeField] private float currentMoveSpeed;
    protected Vector2 move;

    [Space]
    [Header("Raycasts:")]

    [Tooltip("The location of the units eye range.")]
    [SerializeField] protected Transform eyeRange;
    [Tooltip("The distance of the units eye range.")]
    [SerializeField] protected float eyeRangeDistance = 10f;
    [Tooltip("The location of the units wall checking range.")]
    [SerializeField] protected Transform wallDetection;
    [Tooltip("The distance of the units wall checking range (how close will they get to the wall before turning around).")]
    [SerializeField] protected float wallDistance = 1f;
    [Tooltip("The location of the units ground checking range.")]
    [SerializeField] protected Transform groundDetection;
    [Tooltip("The distance of the units ground checking range.")]
    [SerializeField] protected float groundDistance = 0.25f;
    protected LayerMask groundLayerMask;
    protected LayerMask playerLayerMask;

    [Space]
    [Header("Respawn Location:")]

    [Tooltip("The location that the unit will respawn.")]
    public Transform respawnZone;

    [Space]
    [Header("Enemy References:")]

    protected Animator animator;
    protected GameObject target;

    private void OnEnable()
    {
        SpawnManager.onResetLevelObjects += ResetBaseUnit;
    }

    private void OnDisable()
    {
        SpawnManager.onResetLevelObjects -= ResetBaseUnit;
    }

    protected override void Start()
    {
        base.Start();

        Toolbox.GetInstance().GetLevelManager().AddBaseEnemies(this);

        currentState = startingState;
        startingPos = transform.position;

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
        ComputeVelocity();
    }

    public void ResetBaseUnit()
    {
        isDead = false;
        Debug.Log(gameObject.name + " was reset");
        gameObject.SetActive(true);
        transform.position = startingPos;
        currentState = startingState;
        currentHP = startHP;
    }


    // << --------------------------------------- COMBAT -------------------------------- >> //

    public IEnumerator ThisUnitHit(Vector2 _hitDirection, float _knockBack, float _knockUp, float _stunTime)
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
            AfterThisUnitWasAttacked();

        yield break;
    }

    protected IEnumerator AttackCoolDown()
    {
        currentState = State.Idle;

        yield return new WaitForSeconds(attackCooldown);

        if (target != null)
        {
            Vector2 targetDirection;

            targetDirection = (transform.position + target.transform.position);
            targetDirection.x = Mathf.Clamp(targetDirection.x, -1f, 1f);

            if (targetDirection.x > 0 && direction == Vector2.right || targetDirection.x < 0 && direction == Vector2.left)
            {
                FlipSprite();
            }

            currentState = State.Hunting;
        }
        else
        {
            currentState = State.Patrolling;
        }

        objectHit = false;

        yield break;
    }

    protected virtual void AfterThisUnitWasAttacked()
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
        //Toolbox.GetInstance().GetLevelManager().RemoveBaseEnemies(this);
        //currentState = State.Dead;
    }


    // << --------------------------------------- MOVEMENT -------------------------------- >> //

    protected override void ComputeVelocity()
    {
        if (!isUnitPaused)
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
        else
        {
            targetVelocity.x = 0;
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
        if (!groundInfo.collider && this.currentState != State.Attacking && isGrounded)
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
