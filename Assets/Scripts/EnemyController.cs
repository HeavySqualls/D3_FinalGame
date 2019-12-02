using System.Collections;
using UnityEngine;

public class EnemyController : PhysicsObject
{
    enum State { Patrolling, Hunting, Hurt, Dead }
    State currentState;

    [Space]
    [Header("Enemy Status:")]
    public bool isHunter = true;
    public bool isHit = false;
    private bool isMoving = false;
    private bool movingRight = true;
    private bool isDead = false;

    [Space]
    [Header("Enemy Stats:")]
    public float patrolSpeed = 1.25f;
    public float huntSpeed = 3f;
    public int damageOutput = 2;
    public int health = 10;

    [Space]
    [Header("Enemy AI:")]
    public float pauseTime = 2f;
    private float detectionRange = 8f;
    private float wallDistance = 1f;
    private float groundDistance = 0.25f;
    private Vector2 newDirection;

    [Space]
    [Header("Enemy References:")]
    public Transform eyeRange;
    public Transform wallDetection;
    public Transform groundDetection;
    private PlayerController pCon;
    private BoxCollider2D col;
    //private Animator animator;
    private GameObject target;

    private float x;
    private float y; 

    void Start()
    {
        this.currentState = State.Patrolling;
        col = GetComponent<BoxCollider2D>();

        //animator = GetComponent<Animator>();
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
            case State.Dead:
                this.Dead();
                break;
        }
    }


    // ---- STATES ---- //

    private void Patrolling()
    {
        Patrol();
    }

    private void Hunting()
    {
        // If enemy has not charged, get the current location of them, 
        // flip sprite if necessary and charge at their last known location.
        if (isMoving)
        {
            DetectCollisions();
            PlayerCheckCast(newDirection);

            // Move enemy forward with an increased speed, until it hits the end of the platform it is on
            transform.Translate(Vector2.right * huntSpeed * Time.deltaTime);
        }
    }

    private void Hurt()
    {
        targetVelocity.x += x;
        velocity.y += y;

        if (!isHit)
        {
            targetVelocity.x = 0;         
            GoPatrolling();
        }
    }

    private void Dead()
    {
        // wait to be destroyed by take damage call
    }


    // ---- METHODS ---- //

    public void UpdateTargVelocity(float _x, float _y)
    {
        x = _x;
        velocity.y = _y;
    }

    public void GoPatrolling()
    {
        this.currentState = State.Patrolling;
    }

    public void TakeDamage(int _damage)
    {
        this.currentState = State.Hurt;

        if (isHunter)
        {
            health -= _damage;
            //StartCoroutine(IFlashRed(GetComponent<SpriteRenderer>()));

            //if (health <= 0)
            //{
            //    isDead = true;
            //    //animator.SetTrigger("isDead");
            //    gravityModifier = 0f;
            //    col.enabled = false;
            //    //pCon.enemiesKilled++;
            //    Destroy(gameObject, 3f);
            //    this.currentState = State.Dead;
            //}
        }
    }

    public void StartChaseWait()
    {
        StartCoroutine(ChaseWait());
    }

    private IEnumerator ChaseWait()
    {
        isMoving = false;
        //animator.SetBool("moving", isMoving);

        yield return new WaitForSeconds(pauseTime);

        isMoving = true;
    }

    private void Patrol()
    {
        if (isGrounded)
        {
            isMoving = true;
            transform.Translate(Vector2.right * patrolSpeed * Time.deltaTime);
            //animator.SetBool("moving", isMoving);
        }
        else
        {
            isMoving = false;
        }

        if (isHunter)
        {
            PlayerCheckCast(newDirection);
        }

        DetectCollisions();
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
        LayerMask walls = LayerMask.GetMask("Ground") | LayerMask.GetMask("Player");

        RaycastHit2D wallInfo;
        if (!movingRight)
        {
            newDirection = Vector2.left; // If moving left, raycast left
            wallInfo = Physics2D.Raycast(wallDetection.position, newDirection, wallDistance, walls);
            Debug.DrawRay(wallDetection.position, newDirection * wallDistance, Color.white);
        }
        else
        {
            newDirection = Vector2.right; // If moving right, raycast right
            wallInfo = Physics2D.Raycast(wallDetection.position, newDirection, wallDistance, walls);
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

                if (movingRight && wallInfo.collider.tag == "Player")
                {

                }
                else if (!movingRight && wallInfo.collider.tag == "Player")
                {

                }


                // If the enemy has reached the end of the platform, return to patrolling state
                if (this.currentState == State.Hunting && target == null)
                {
                    this.currentState = State.Patrolling;
                }
            }
            else if (true)
            {

            }
        }

        // ---- CHECK FOR GROUND ---- //

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, groundDistance, walls);
        Debug.DrawRay(groundDetection.position, Vector2.down * groundDistance, Color.blue);

        // Checks for a loss of vertical collision (end of platform)
        if (!groundInfo.collider)
        {
            FlipSprite();

            if (!isHunter)
            {
                ChaseWait();
            }

            // If the enemy has reached the edge of the platform, return to patrolling state
            if (this.currentState == State.Hunting)
            {
                this.currentState = State.Patrolling;
            }
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

    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    if (!isDead)
    //    {
    //        _OnPlayerHitEnemy pHitE = other.gameObject.GetComponent<_OnPlayerHitEnemy>();

    //        if (pHitE != null)
    //        {
    //            pHitE.OnHit(other, this);
    //        }
    //    }
    //}
}
