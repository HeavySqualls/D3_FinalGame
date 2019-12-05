﻿using System.Collections;
using UnityEngine;

public class EnemyController : PhysicsObject
{
    public enum State { Patrolling, Hunting, Hurt, InWind, Dead }
    public State currentState;

    [Space]
    [Header("Enemy Status:")]
    public bool isHunter = true;
    public bool isHit = false;
    private bool movingRight = true;

    [Space]
    [Header("Enemy Stats:")]
    public float patrolSpeed = 1.25f;
    public float huntSpeed = 3f;
    public int damageOutput = 2;
    public float currentHP;
    [SerializeField] float hpStart = 10f;
    [SerializeField] float baseMoveSpeed;
    [SerializeField] float baseMoveSpeedStart = 50f;


    [Space]
    [Header("Enemy AI:")]
    public float pauseTime = 2f;
    private float detectionRange = 8f;
    private float wallDistance = 1f;
    private float groundDistance = 0.25f;
    [SerializeField] private Vector2 move;
    [SerializeField] private Vector2 newDirection;
    [SerializeField] private Vector2 hitDirection;

    [Space]
    [Header("Enemy References:")]
    public Transform respawnZone;
    public ParticleSystem deathParticles;
    public Transform eyeRange;
    public Transform wallDetection;
    public Transform groundDetection;
    private GameObject target;
    private MeshRenderer mR;
    private BoxCollider2D coll;

    [SerializeField] private float x;
    [SerializeField] private float y; 

    void Start()
    {
        mR = GetComponent<MeshRenderer>();
        coll = GetComponent<BoxCollider2D>();

        baseMoveSpeed = baseMoveSpeedStart;
        currentHP = hpStart;

        this.currentState = State.Patrolling;
    }

    protected override void Update()
    {
        base.Update();

        switch (this.currentState)
        {
            case State.Patrolling:
                this.Patrolling();
                break;
            case State.Hunting:
                this.Hunting();
                break;
            case State.Hurt:
                this.Hurt();
                break;
            case State.InWind:
                this.InWind();
                break;
            case State.Dead:
                this.Dead();
                break;
        }
    }

    void ComputeBaseMoveSpeed()
    {
        if (inWindZone)
        {
            if (windDir == Vector2.right) // Wind is moving to the right 
            {
                if (windDir == newDirection)
                {
                    baseMoveSpeed += windDir.x * windPwr;

                    if (baseMoveSpeed > 60)
                    {
                        baseMoveSpeed = 60;
                    }
                }
                else if (windDir != newDirection)
                {
                    baseMoveSpeed -= windDir.x * windPwr;

                    if (baseMoveSpeed < 10)
                    {
                        baseMoveSpeed = 10;
                    }
                }
            }
            else if (windDir == Vector2.left) // Wind is moving to the left 
            {
                if (windDir != newDirection)
                {
                    baseMoveSpeed -= windDir.x * windPwr;

                    if (baseMoveSpeed > 10)
                    {
                        baseMoveSpeed = 10;
                    }
                }
                else if (windDir == newDirection)
                {
                    baseMoveSpeed += windDir.x * windPwr;

                    if (baseMoveSpeed < 60)
                    {
                        baseMoveSpeed = 60;
                    }
                }
            }
        }
        else
        {
            baseMoveSpeed = baseMoveSpeedStart;
        }
    } // TODO: Find a way to simplify this 

    // ---- STATES ---- //

    private void Patrolling()
    {
        move = Vector2.zero;

        ComputeBaseMoveSpeed();

        if (isGrounded)
        {
            if (movingRight)
            {
                move.x = baseMoveSpeed;
            }
            else
            {
                move.x = -baseMoveSpeed;
            }
        }

        if (isHunter)
        {
            PlayerCheckCast(newDirection);
        }

        if (isHit)
        {
            isHit = false;
        }

        DetectCollisions();
        targetVelocity = move * patrolSpeed * Time.deltaTime;
    }

    private void Hunting()
    {
        move = Vector2.zero;

        ComputeBaseMoveSpeed();

        if (isGrounded)
        {
            DetectCollisions();
            PlayerCheckCast(newDirection);

            if (movingRight)
            {
                move.x = baseMoveSpeed;
            }
            else
            {
                move.x = -baseMoveSpeed;
            }
        }

        targetVelocity = move * huntSpeed * Time.deltaTime;
    }

    private void Hurt()
    {
        if (inWindZone && isHit)
        {
            this.currentState = State.InWind;
        }
        else
        {
            targetVelocity.x += x;
            velocity.y += y;
        }

        if (currentHP <= 0)
        {
            Killed();
            this.currentState = State.Dead;
        }
    }

    private void InWind()
    {
        gravityModifier = 5f; // <<--- Gets reset to start grav when landed 

        targetVelocity.x = 0;
        move.x = 0;

        move.x += (windDir.x * windPwr * 2000);

        if (isGrounded)
        {
            this.currentState = State.Patrolling;
        }

        targetVelocity.x = move.x * Time.deltaTime;
    }

    private void Dead()
    {
        // wait to be destroyed by take damage call
    }


    // ---- METHODS ---- //

    public void Killed()
    {
        StartCoroutine(Respawn());
        ParticleSystem ps = Instantiate(deathParticles, transform.position, transform.rotation) as ParticleSystem;
        ps.transform.parent = transform; 
        Destroy(ps, 2);
    }

    IEnumerator Respawn()
    {
        mR.enabled = false;
        coll.enabled = false;

        yield return new WaitForSeconds(3);

        if (respawnZone)
        {
            mR.enabled = true;
            coll.enabled = true;
            currentHP = hpStart;
            gravityModifier = gravStart;
            transform.position = respawnZone.position;
            move = Vector2.zero;
        }
        else
            Debug.Log("No Respawn Zone Assigned To " + gameObject.name);
    }

    public void TakeDamage(Vector2 _hitDir, float _damage, float _x, float _y)
    {
        isHit = true;
        hitDirection = _hitDir;
        currentHP -= _damage;
        x = _x;
        velocity.y = _y;

        if (hitDirection == newDirection)
        {
            FlipSprite();
        }

        this.currentState = State.Hurt;
    }


    // ---- COLLISION/PLAYER CHECKS ---- //


    private void PlayerCheckCast(Vector2 direction)
    {
        LayerMask player = LayerMask.GetMask("Player");

        RaycastHit2D hit = Physics2D.Raycast(eyeRange.position, direction * detectionRange, detectionRange, player);
            Debug.DrawRay(eyeRange.position, direction * detectionRange, Color.red);

        if (hit.collider != null && !hit.collider.isTrigger)
        {
            if (hit.collider.GetComponent<PlayerController>())
            {
                target = hit.collider.gameObject;
                this.currentState = State.Hunting;
            }

            if (!hit.collider.GetComponent<PlayerController>() && target != null)
            {
                target = null;
            }
        }
    }

    private void DetectCollisions()
    {
        // ---- CHECK FOR WALL ---- //
        LayerMask wallORplayer = LayerMask.GetMask("Ground") | LayerMask.GetMask("Player");
        RaycastHit2D wallInfo;

        if (movingRight)
        {
            newDirection = Vector2.right; // If moving right, raycast right
            wallInfo = Physics2D.Raycast(wallDetection.position, newDirection, wallDistance, wallORplayer);
                Debug.DrawRay(wallDetection.position, newDirection * wallDistance, Color.white);
        }
        else
        {
            newDirection = Vector2.left; // If moving left, raycast left
            wallInfo = Physics2D.Raycast(wallDetection.position, newDirection, wallDistance, wallORplayer);
                Debug.DrawRay(wallDetection.position, newDirection * wallDistance, Color.white);
        }

        if (wallInfo.collider != null)
        {
            if (!wallInfo.collider.isTrigger)
            {
                // Walls 
                if (movingRight && wallInfo.collider.tag == "Platform")
                {
                    transform.eulerAngles = new Vector3(0, -180, 0);
                    movingRight = false;
                }
                else if (!movingRight && wallInfo.collider.tag == "Platform")
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    movingRight = true;
                }

                //if (movingRight && wallInfo.collider.tag == "Player")
                //{
                //    // Attack?
                //}
                //else if (!movingRight && wallInfo.collider.tag == "Player")
                //{
                //    // Attack?
                //}

                // If the enemy has reached the end of the platform, return to patrolling state
                if (this.currentState == State.Hunting && target == null)
                {
                    this.currentState = State.Patrolling;
                }
            }
        }

        // ---- CHECK FOR GROUND ---- //
        LayerMask platform = LayerMask.GetMask("Ground");
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, groundDistance, platform);
        Debug.DrawRay(groundDetection.position, Vector2.down * groundDistance, Color.blue);

        // Checks for a loss of vertical collision (end of platform)
        if (!groundInfo.collider)
        {
            isGrounded = false;

            if (!isHit)
            {
                FlipSprite();
            }

            if (this.currentState == State.Hunting)
            {
                this.currentState = State.Patrolling;
            }
        }
        else
        {
            isGrounded = true;
        }
    }

    private void FlipSprite()
    {
        if (movingRight)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            newDirection = Vector2.left;
            movingRight = false;
        }
        else if (!movingRight)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            newDirection = Vector2.right;
            movingRight = true;
        }
    }
}