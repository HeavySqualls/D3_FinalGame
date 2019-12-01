using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsObject
{
    [Space]
    [Header("STATUS:")]
    public bool isInteractable;
    public bool canMove = true;
    public bool isMoving = false; // move to compute velocity
    public bool isMovingInWind = false; // move to compute velocity
    public bool magBootsOn = false;
    public bool inWindZone = false;
    public bool pIsFlipped;
    [SerializeField] private bool canJump = true;

    [Space]
    [Header("MOVEMENT:")]
    public float maxSpeed = 2f;
    private bool windAffectUnit = true;
    private Vector2 windDir;
    private float windPwr;
    private float windRatio; // Ratio between the wind power and the players velocity
    private bool directionOfSource;

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
    [SerializeField] private bool isTouchingLedge = false;

    [Space]
    [Header("WALL CHECK:")]
    public float wallCheckDistance = 1;
    public Transform wallCheck; // for checking if against wall
    private bool isTouchingWall = false;

    [Space]
    [Header("GROUND CHECK:")]
    public Vector3 bottom;
    public float groundCheckDistance = 2f;
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
        CheckSurroundings();
        ComputeVelocity();
        MagBoots();
        JumpTimer();
        CheckLedgeClimb();
        TrackAirTime();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    // ---- LOCOMOTION METHODS ---- //

    public void WindZoneStats(Vector2 _windDir, float _windPwr, bool _directionOfSource)
    {
        windDir = _windDir;
        windPwr = _windPwr;
        directionOfSource = _directionOfSource;
    }

    protected override void ComputeVelocity()
    {
        if (canMove)
        {
            Vector2 move = Vector2.zero;

            if (Input.GetButton("Horizontal"))
            {
                isMoving = true;

                if (inWindZone)
                {
                    isMovingInWind = true;
                }

                move.x = Input.GetAxis("Horizontal") + 0.1f;

                float minXFlip = 0.001f; // minimum velocity on the x axis to trigger the sprite flip
                bool flipPlayerSprite = (spriteRenderer.flipX ? (velocity.x > minXFlip) : (velocity.x < -minXFlip));

                if (inWindZone && directionOfSource && pIsFlipped && isGrounded)
                {
                    if (velocity.x > windRatio +1) // 1 is just a random int to get a value outside the limit of windForce
                    {
                        ChangeDirection();
                        pIsFlipped = !pIsFlipped;
                        spriteRenderer.flipX = !spriteRenderer.flipX;
                    }
                    else
                    {
                        pIsFlipped = true;
                    }                 
                }
                else if (inWindZone && !directionOfSource && !pIsFlipped && isGrounded)
                {
                    if (velocity.x > windRatio + 1) // 1 is just a random int to get a value outside the limit of windForce
                    {
                        pIsFlipped = false;
                    }
                    else
                    {
                        print("flip");
                        ChangeDirection();
                        pIsFlipped = !pIsFlipped;
                        spriteRenderer.flipX = !spriteRenderer.flipX;
                    }
                }
                else if (flipPlayerSprite)
                {
                    ChangeDirection();
                    pIsFlipped = !pIsFlipped;
                    spriteRenderer.flipX = !spriteRenderer.flipX;
                }
            }

            if (Input.GetButtonUp("Horizontal"))
            {
                isMoving = false;
                isMovingInWind = false;
            }

            // If the player is in the windzone without mag boots and not against a wall, add in wind force vector to move vector
            if (inWindZone && !magBootsOn && windAffectUnit)
            {              
                if (velocity.x > 0.001f || velocity.x < -0.001f) // player moving right/left 
                {
                    move.x += windDir.x * windPwr;
                }
                else if (velocity.x == 0)
                {
                    move.x = windDir.x * windPwr;
                }

                if (isTouchingWall)
                {
                    windAffectUnit = false;
                }
            }

            if (!windAffectUnit && Input.GetButton("Horizontal"))
            {
                StartCoroutine(Countdown());
            }

            Jump();

            RapidJump();

            animator.SetBool("grounded", isGrounded);
            animator.SetBool("inWind", inWindZone);

            if (!isMoving)
            {
                windRatio = velocity.x;
            }

            // Used to always face the player in to the direction of the wind - if in wind zone and not moving 
            //  - negative windForce value is used when wind is coming from the right
            if (inWindZone && velocity.x == windRatio && isGrounded || 
                inWindZone && velocity.x == -windRatio && isGrounded || 
                inWindZone && magBootsOn && velocity.x == 0 && isGrounded
                )
            {
                //isMoving = false;
                animator.SetBool("isMovingInWind", isMovingInWind);

                if (directionOfSource && !pIsFlipped)
                {
                    ChangeDirection();
                    pIsFlipped = !pIsFlipped;
                    spriteRenderer.flipX = !spriteRenderer.flipX;
                }
                else if (!directionOfSource && pIsFlipped)
                {
                    ChangeDirection();
                    pIsFlipped = !pIsFlipped;
                    spriteRenderer.flipX = !spriteRenderer.flipX;
                }
            }
            else
            {
                animator.SetBool("isMovingInWind", isMoving);
                animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            }
            
            targetVelocity = move * maxSpeed;
        }
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(1f);
        windAffectUnit = true;
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
        EnableMovement(false);
        magBootsOn = true;
        gravityModifier = 0;
        rb2d.velocity = Vector3.zero;
        ripPP.CauseRipple(groundCheck, 15f, 0.95f);

        yield return new WaitForSeconds(0.25f);

        gravityModifier = onGravValue;
        EnableMovement(true);
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

            EnableMovement(false);
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
        EnableMovement(true);
        ledgeDetected = false;
        canClimbLedge = false;
        canJump = true;
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
            print("can jump");
            canJump = true;
        }
    }

    public void RapidJump()
    {
        if (isTouchingGround && !isGrounded && canJump)
        {
            StartCoroutine(QuickJumpTimer(1));
        }
    }

    private IEnumerator QuickJumpTimer(float _time)
    {
        if (Input.GetButton("Jump"))
        {
            velocity.y = jumpTakeoffSpeed;
            animator.SetTrigger("jumping");
            currentGraceTime = 0;

            StopTrackAirTime();
            inAir = true;
        }

        yield return new WaitForSeconds(_time);
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

    // TODO: Player does not hard land on medium height falls if he just walks off the edge. 
    //       Look in to adding something that detects that player did not jump before falling? 
    void TrackAirTime() 
    {
        if (inAir)
        {
            airTime += Time.deltaTime;

            if (isGrounded)
            {
                if (airTime > hardLandTime) // Light hard landing 
                {
                    animator.SetTrigger("land");
                    StartCoroutine(LandingPause(animator.GetCurrentAnimatorStateInfo(0).length));
                    ripPP.CauseRipple(groundCheck, 12f, 0.8f);
                }
                else if (airTime > heavyLandTime) // Heavy hard landing 
                {
                    animator.SetTrigger("land");
                    StartCoroutine(LandingPause(animator.GetCurrentAnimatorStateInfo(0).length + 0.5f));
                    ripPP.CauseRipple(groundCheck, 30f, 0.9f);
                }

                if (magBootsOn) // Landing with Mag Boots engaged
                {
                    animator.SetTrigger("land");
                    StartCoroutine(LandingPause(animator.GetCurrentAnimatorStateInfo(0).length));
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

    public void EnableMovement(bool _enable) // Called by the Animator at the moment
    {
        canMove = _enable;
    }

    private IEnumerator LandingPause(float _pauseTime)
    {
        EnableMovement(false);

        yield return new WaitForSeconds(_pauseTime);
        
        EnableMovement(true);
    }
}
