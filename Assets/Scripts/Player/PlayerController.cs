using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsObject
{
    [Space]
    [Header("STATUS:")]
    public bool isController = false;
    public bool isInteractable;
    public bool canMove = true;
    public bool isMoving = false;
    public bool isMovingInWind = false;
    public bool magBootsOn = false;
    public bool pIsFlipped;
    [SerializeField] private bool canFlipSprite = true;
    [SerializeField] private bool canJump = true;

    public AnimationCurve accelerationCurve;
    public float accelerationTime = 1;

    [Space]
    [Header("MOVEMENT:")]
    public float maxSpeed = 2f;
    public Vector2 accessibleDirection; // a player direction vector that other scripts can read and use without making the physics object public
    private bool windAffectUnit = true;
    private float windRatio; // Ratio between the wind power and the players velocity

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
    public Animator animator;
    public Controls controls;
    private RipplePostProcessor ripPP;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        direction = Vector2.right;
        accessibleDirection = direction;
        ripPP = Camera.main.GetComponent<RipplePostProcessor>();
    }

    void Start()
    {
        whatIsGround = LayerMask.GetMask("Ground");

        if (controls != null)
        {
            if (isController)
                controls.ControllerControls();
            else
                controls.KeyboardControls();
        }
        else
            Debug.Log("Player does not have the controls component attached!");

        //speed * accelerationCurve.Evaluate(accelerationTime)
        //if(accelerationTime+Time.deltaTime < 1) accelerationTime = Mathf.Min(accelerationTime+Time.deltaTime, 1);
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


    protected override void ComputeVelocity()
    {
        if (canMove)
        {
            Vector2 move = Vector2.zero;

            if (Input.GetAxis(controls.xMove) > 0.2f || Input.GetAxis(controls.xMove) < 0f)
            {
                isMoving = true;

                if (inWindZone)
                {
                    isMovingInWind = true;
                }

                move.x = Input.GetAxis(controls.xMove) + 0.1f;

                if (canFlipSprite)
                {
                    float minXFlip = 0.001f; // minimum velocity on the x axis to trigger the sprite flip
                    bool flipPlayerSprite = (spriteRenderer.flipX ? (velocity.x > minXFlip) : (velocity.x < -minXFlip));

                    if (inWindZone && directionOfSource && pIsFlipped && isGrounded)
                    {
                        if (velocity.x > windRatio + 1) // 1 is just a random int to get a value outside the limit of windForce
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
            }

            if (Input.GetAxis(controls.xMove) == 0)
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

            if (!windAffectUnit && Input.GetAxis(controls.xMove) > 0.2f || !windAffectUnit && Input.GetAxis(controls.xMove) < 0f)
            {
                StartCoroutine(LeaveWallCountdown());
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

                if (canFlipSprite)
                {
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
            }
            else
            {
                animator.SetBool("isMovingInWind", isMoving);
                animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            }
            
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

    private void ChangeDirection()
    {
        if (canFlipSprite)
        {
            if (direction == Vector2.right)
                direction = Vector2.left;
            else
                direction = Vector2.right;

            accessibleDirection = direction;
        }
    }

    public void CanFlipSprite()
    {
        if (canFlipSprite)
            canFlipSprite = false;
        else
            canFlipSprite = true;
    }



    // ---- MAG BOOTS METHODS ---- //


    public void MagBoots()
    {
        if (Input.GetButtonUp(controls.magBoots) && !magBootsOn)
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
        else if (Input.GetButtonUp(controls.magBoots) && magBootsOn)
        {
            print("MagBoots Activated: " + magBootsOn);
            magBootsOn = false;
            gravityModifier = gravStart;
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
            if (Input.GetButtonDown(controls.jump) && isGrounded && canJump ||
                Input.GetButtonDown(controls.jump) && currentGraceTime > 0 && canJump ||
                isTouchingGround && !isGrounded && Input.GetButtonDown(controls.jump) && canJump
                )
            {
                velocity.y = jumpTakeoffSpeed;
                animator.SetTrigger("jumping");
                currentGraceTime = 0;
                canJump = false;
            }
            else if (Input.GetButtonUp(controls.jump))
            {            
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * 0.5f;
                }
            }
        }

        if (Input.GetButtonUp(controls.jump))
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
        if (Input.GetButton(controls.jump))
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

    private IEnumerator LeaveWallCountdown()
    {
        yield return new WaitForSeconds(1f);
        windAffectUnit = true;
    }
}
