using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsObject
{
    [Space]
    [Header("STATUS:")]
    public bool isInteractable;
    public bool canMove = true;
    public bool magBootsOn = false;
    private bool pIsFlipped;
    private bool canJump = true;

    [Space]
    [Header("MOVEMENT:")]
    public float maxSpeed = 2f;

    [Space]
    [Header("JUMP:")]
    public float jumpTakeoffSpeed = 6f;
    public float hardLandTime = 1f;
    public float heavyLandTime = 1.5f;
    [SerializeField] bool inAir;
    [SerializeField] float airTime = 0f;
    [SerializeField] float maxGraceTime = 0.12f;
    [SerializeField] float currentGraceTime;

    [Space]
    [Header("MAG BOOTS:")]
    public float onGravValue = 10f;
    public ParticleSystem bootSparks;

    [Space]
    [Header("LEDGE CHECK:")]
    public float ledgeCheckDistance = 1;
    public Transform ledgeCheck; // for checking if on the edge of a collider
    public bool canClimbLedge = false;
    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f; 
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;
    private Vector2 ledgePosBot;
    private bool ledgeDetected;
    private bool isTouchingLedge = false;

    [Space]
    [Header("WALL CHECK:")]
    public float wallCheckDistance = 1;
    public Transform wallCheck; // for checking if against wall
    private bool isTouchingWall = false;

    [Space]
    [Header("GROUND CHECK:")]
    public Vector3 bottom;
    public float groundCheckDistance = 1;
    public Transform groundCheck; // for determining quick landing jump
    [SerializeField] private bool isTouchingGround = false;
    private int whatIsGround;

    [Space]
    [Header("REFERENCES:")]
    private RipplePostProcessor ripPP;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        direction = Vector2.right;
        ripPP = Camera.main.GetComponent<RipplePostProcessor>();
    }

    void Start()
    {
        whatIsGround = LayerMask.GetMask("Ground");
    }

    protected override void Update()
    {
        base.Update();
        ComputeVelocity();
        MagBoots();
        JumpTimer();
        CheckLedgeClimb();
        TrackAirTime();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        CheckSurroundings();
    }


    // ---- LOCOMOTION METHODS ---- //

    protected override void ComputeVelocity()
    {
        if (canMove)
        {
            Vector2 move = Vector2.zero;
            move.x = Input.GetAxis("Horizontal");

            Jump();

            RapidJump();

            bool flipPlayerSprite = (spriteRenderer.flipX ? (move.x > 0.0001f) : (move.x < -0.0001f));
            if (flipPlayerSprite)
            {
                ChangeDirection();
                pIsFlipped = !pIsFlipped;
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }

            animator.SetBool("grounded", isGrounded);

            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            targetVelocity = move * maxSpeed;
        }
    }

    private void CheckSurroundings()
    {
        if (canMove)
        {
            isTouchingGround = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
            Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

            isTouchingWall = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, whatIsGround);
            Debug.DrawRay(wallCheck.position, direction * wallCheckDistance, Color.red);

            isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, direction, ledgeCheckDistance, whatIsGround);
            Debug.DrawRay(ledgeCheck.position, direction * ledgeCheckDistance, Color.red);

            if (isTouchingWall && !isTouchingLedge && !ledgeDetected)// << ----------- OPTION TO PUT MANUAL LEDGE GRAB HERE 
            {
                ledgeDetected = true;
                ledgePosBot = wallCheck.position;
            }
        }
    }
    public void MagBoots()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1) && !magBootsOn)
        {
            print("MagBoots Activated: " + magBootsOn);

            if (!isGrounded)
            {
                StartCoroutine(MagBootsOn());
            }
            else
            {
                magBootsOn = true;
                rb2d.velocity = Vector3.zero;
                gravityModifier = onGravValue;
                ripPP.CauseRipple(groundCheck, 12f, 0.5f);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1) && magBootsOn)
        {
            print("MagBoots Activated: " + magBootsOn);
            magBootsOn = false;
            gravityModifier = 6.35f;
        }
    }

    private IEnumerator MagBootsOn()
    {
        magBootsOn = true;
        canMove = false;
        gravityModifier = 0;
        rb2d.velocity = Vector3.zero;
        ripPP.CauseRipple(groundCheck, 15f, 0.95f);

        yield return new WaitForSeconds(0.25f);

        gravityModifier = onGravValue;
        canMove = true;
    }

    private void ChangeDirection()
    {
        if (direction == Vector2.right)
            direction = Vector2.left;
        else
            direction = Vector2.right;
    }



    // ---- LEDGE CLIMB METHODS ---- //


    private void CheckLedgeClimb()
    {
        if (ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;

            StopTrackAirTime();

            if (direction == Vector2.right)
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }

            canMove = false;
            //canFlip = false;

            animator.SetBool("canClimbLedge", canClimbLedge);
        }

        if (canClimbLedge)
        {
            transform.position = ledgePos1;
        }
    }

    public void FinishLedgeClimb()
    {
        transform.position = ledgePos2;
        canMove = true;
        ledgeDetected = false;
        canClimbLedge = false;
        animator.SetBool("canClimbLedge", canClimbLedge);
    }



    // ---- JUMP METHODS ---- //

    private void Jump()
    {
        if (!magBootsOn)
        {
            if (Input.GetButtonDown("Jump") && isGrounded && canJump ||
                Input.GetButtonDown("Jump") && currentGraceTime > 0 && canJump ||
                isTouchingGround && !isGrounded && Input.GetButtonDown("Jump") && canJump
                )
            {
                velocity.y = jumpTakeoffSpeed;
                animator.SetTrigger("jumping");
                currentGraceTime = 0;
                canJump = false;
            }
            else if (Input.GetButtonUp("Jump"))
            {            
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * 0.5f;
                }
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            canJump = true;
        }
    }

    public void RapidJump()
    {
        if (isTouchingGround && !isGrounded && Input.GetButtonDown("Jump") && canJump)
        {
            velocity.y = jumpTakeoffSpeed;
            animator.SetTrigger("jumping");
            currentGraceTime = 0;

            StopTrackAirTime();
            inAir = true;
        }
    }

    void JumpTimer()
    {
        if (!isGrounded)
        {
            inAir = true;
            currentGraceTime -= Time.deltaTime;
        }
        else
        {
            currentGraceTime = maxGraceTime;
        }
    }

    void TrackAirTime()
    {
        if (inAir)
        {
            airTime += Time.deltaTime;

            if (isGrounded)
            {
                if (airTime > hardLandTime)
                {
                    animator.SetTrigger("land");
                    ripPP.CauseRipple(groundCheck, 12f, 0.8f);
                }
                else if (airTime > heavyLandTime)
                {
                    animator.SetTrigger("land");
                    ripPP.CauseRipple(groundCheck, 30f, 0.9f);
                }

                if (magBootsOn)
                {
                    ripPP.CauseRipple(groundCheck, 30f, 0.9f);
                    ParticleSystem ps = Instantiate(bootSparks, transform.position, transform.rotation) as ParticleSystem;
                    Destroy(ps.gameObject, ps.main.startLifetime.constantMax);
                }

                airTime = 0;
                inAir = false;
            }
        }
    }

    void StopTrackAirTime()
    {
        airTime = 0;
        inAir = false;
    }


    // ---- UTILITY ---- //

    public void EnableMovement() // Called by the Animator at the moment
    {
        canMove = true;
    }

    public void DisableMovement() // Called by the Animator at the moment
    {
        canMove = false;
    }
}
