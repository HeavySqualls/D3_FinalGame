using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsObject
{
    [Space]
    [Header("INPUT:")]
    public bool isController = false;

    [Space]
    [Header("MOVEMENT / WIND:")]
    public bool canMove = true;
    public bool isMoving = false;
    public bool isMovingInWind = false;
    public float maxSpeed = 2f;
    public Vector2 accessibleDirection; // a player direction vector that other scripts can read and use without making the physics object public
    public float accelSpeed = 1;
    private float timeFromZeroToMax = 2.5f;
    private float accelRatePerSecond;
    private bool windAffectUnit = true;
    private bool pIsFaceLeft;
    private bool canFlipSprite = true;
    private bool backToWind = true;
    [SerializeField] private float windRatio; // Ratio between the wind power and the players velocity
    public AnimationCurve accelerationCurve;

    [Space]
    [Header("JUMP:")]
    public float jumpTakeoffSpeed = 6f;
    public float hardLandTime = 1f;
    public float heavyLandTime = 1.5f;
    public float jumpDelayTime = 0.02f;
    [SerializeField] float maxGraceTime = 0.12f;
    [SerializeField] float currentGraceTime;
    [SerializeField] bool inAir;
    [SerializeField] float airTime = 0f;
    private bool canJump = true;

    [Space]
    [Header("MAG BOOTS:")]
    public bool magBootsOn = false;
    public float onGravValue = 10f;
    public ParticleSystem bootSparks;

    [Space]
    [Header("GROUND CHECK:")]
    public Vector3 bottom;
    public float groundCheckDistance = 1.75f;
    public Transform groundCheck; // for determining quick landing jump
    private int whatIsGround;

    [Space]
    [Header("WALL CHECK:")]
    public float wallCheckDistance = 1;
    public Transform wallCheck; // for checking if against wall
    [SerializeField] private bool isTouchingWall = false;

    [Space]
    [Header("LEDGE CHECK:")]
    public float ledgeCheckDistance = 1;
    public Transform ledgeCheck; // for checking if on the edge of a collider
    public bool canClimbLedge = false;
    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f;
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;
    [SerializeField] private bool isTouchingLedge = false;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;
    private Vector2 ledgePosBot;
    private bool ledgeDetected;

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

        accelRatePerSecond = (maxSpeed / timeFromZeroToMax) * accelSpeed;
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
        GraceJumpTimer();
        CheckLedgeClimb();
        TrackAirTime();
    }

    public float GetAnimTime()
    {
        if (animator != null)
        {
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

            if (clipInfo.Length > 0 && clipInfo != null)
            {
                return clipInfo[0].clip.length;
            }
        }
        return 0;    
    }

    // ---- LOCOMOTION METHODS ---- //

    Vector2 move;
    bool skid = false;

    IEnumerator PlayerSkid()
    {
        print("Skidding");
        canFlipSprite = false;
        skid = true;

        yield return new WaitForSeconds(GetAnimTime() / 4);

        canFlipSprite = true;
        skid = false;
        move.x = 0;
        canMove = true;

        yield break;
    }

    protected override void ComputeVelocity()
    {
        if (canMove)
        {
            // Determine if there is input
            if (Input.GetAxisRaw(controls.xMove) > 0 || Input.GetAxisRaw(controls.xMove) < 0f) 
            {
                isMoving = true;

                if (Input.GetAxisRaw(controls.xMove) > 0) // Moving Right
                {
                    if (direction == Vector2.left && velocity.x < -10)
                    {                    
                        StartCoroutine(PlayerSkid());       
                        canMove = false;
                    }

                    move.x += accelRatePerSecond * Time.deltaTime;
                }
                else if (Input.GetAxisRaw(controls.xMove) < 0f) // Moving Left
                {
                    if (direction == Vector2.right && velocity.x > 10)
                    {                     
                        StartCoroutine(PlayerSkid());
                        canMove = false;
                    }

                    move.x -= accelRatePerSecond * Time.deltaTime;              
                }

                // Add force from wind to players move.x if applicable
                if (inWindZone && windAffectUnit)
                {
                    isMovingInWind = true;

                    if (magBootsOn)
                    {
                        move.x = Mathf.Clamp(move.x, -maxSpeed, maxSpeed);
                    }
                    else if (windMovingRight)
                    {
                        move.x = Mathf.Clamp(move.x, -maxSpeed * (windDir.x * windPwr), maxSpeed / (windDir.x * windPwr));                      
                    }
                    else if (!windMovingRight)
                    {
                        move.x = Mathf.Clamp(move.x, maxSpeed / (windDir.x * windPwr), -maxSpeed * (windDir.x * windPwr));
                    }
                }
                else if (!inWindZone)
                {
                    move.x = Mathf.Clamp(move.x, -maxSpeed, maxSpeed);
                }
            }         
            else if (Input.GetAxisRaw(controls.xMove) == 0 && !skid)  // Determine if input has stopped
            {
                if (inWindZone && !magBootsOn)
                {
                    move.x = windDir.x / windPwr;
                    isMoving = false;
                    isMovingInWind = false;

                    // Determine wind ratio - The peak velocity that the player rests at when idle and being pushed back by the wind 
                    if (windRatio == 0)
                    {
                        windRatio = move.x * maxSpeed;
                        print(windRatio);
                    }
                }
                else
                {
                    windRatio = 0;

                    if (move.x != 0)
                    {
                        if (direction == Vector2.left)
                        {
                            move.x += accelRatePerSecond * Time.deltaTime;

                            if (move.x >= 0)
                            {                                
                                move.x = 0;
                                isMoving = false;
                            }
                        }
                        else if (direction == Vector2.right)
                        {
                            move.x -= accelRatePerSecond * Time.deltaTime;

                            if (move.x <= 0)
                            {
                                move.x = 0;
                                isMoving = false;
                            }
                        }
                    }
                }
            }

            // Determine if a player is up against a wall - if so, disable the wind effect while they are there 
            if (isTouchingWall && inWindZone)
                windAffectUnit = false;
            else
                windAffectUnit = true;

            // Determine direction that the sprite will face
            if (canFlipSprite)
            {
                if (inWindZone && isMovingInWind)
                {
                    ComputeDirectionOfSpriteInWind();
                }
                else if (!inWindZone)
                {
                    float minXFlip = 0f; // minimum velocity on the x axis to trigger the sprite flip                   
                    bool flipPlayerSprite = (spriteRenderer.flipX ? (velocity.x > minXFlip) : (velocity.x < minXFlip));
                    if (flipPlayerSprite)
                    {
                        ChangeDirection();
                        pIsFaceLeft = !pIsFaceLeft;
                        spriteRenderer.flipX = !spriteRenderer.flipX;
                    }
                }
            }

            // Animation settings
            animator.SetBool("isSkid", skid);
            animator.SetBool("grounded", isGrounded);
            animator.SetBool("inWind", inWindZone);
            animator.SetBool("isBackToWind", backToWind);
            animator.SetBool("isMovingInWind", isMovingInWind);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            // Send the move Vector will all related forces to the Physics Object
            targetVelocity = move * maxSpeed;
        }

        // Jump methods
        Jump();
        RapidJump();
    }

    private void ComputeDirectionOfSpriteInWind()
    {
        if (windMovingRight & !isTouchingWall)
        {
            if (velocity.x > windRatio && pIsFaceLeft) // If player is moving with the wind - face to the right 
            {
                //print("fuck1");
                ChangeDirection();
                pIsFaceLeft = !pIsFaceLeft;
                spriteRenderer.flipX = !spriteRenderer.flipX;
                backToWind = true;
            }
            else if (velocity.x == windRatio && !isMovingInWind) // If the player is not moving, but being pushed with the wind - continue facing in what ever direction they were in, but with idle animation
            {
                if (pIsFaceLeft)
                    backToWind = false;
                else
                    backToWind = true;
            }
            else if (velocity.x < 0 && !pIsFaceLeft) // If the player is moving against the wind - face to the left 
            {
                //print("fuck2");
                ChangeDirection();
                pIsFaceLeft = !pIsFaceLeft;
                spriteRenderer.flipX = !spriteRenderer.flipX;
                backToWind = false;
            }
        }
        else if (!windMovingRight)
        {
            if (velocity.x > 0 && pIsFaceLeft && !isTouchingWall) // If the player is moving against the wind - face to the right
            {
                //print("fuck3");
                ChangeDirection();
                pIsFaceLeft = !pIsFaceLeft;
                spriteRenderer.flipX = !spriteRenderer.flipX;
                backToWind = false;
            }
            else if (velocity.x == windRatio && !isMovingInWind) // If the player is not moving, but being pushed with the wind - continue facing in what ever direction they were in, but with idle animation
            {
                if (pIsFaceLeft)
                    backToWind = true;
                else
                    backToWind = false;
            }
            else if (velocity.x < windRatio && !pIsFaceLeft && isMovingInWind && !isTouchingWall) // If the player is moving with the wind - face to the left
            {
                //print("fuck4");
                ChangeDirection();
                pIsFaceLeft = !pIsFaceLeft;
                spriteRenderer.flipX = !spriteRenderer.flipX;
                backToWind = true;
            }
        }
    }

    private void CheckSurroundings()
    {
        if (canMove)
        {
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
            if (hit.collider != null)
            {
                var go = hit.collider.gameObject;

                if (go.GetComponent<DissolvingPlatform>())
                {
                    go.GetComponent<DissolvingPlatform>().CallEnumerator();
                }
            }

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

    private IEnumerator LeaveWallCountdown()
    {
        yield return new WaitForSeconds(0.5f);
        windAffectUnit = true;
        yield break;
    }


    // ---- MAG BOOTS METHODS ---- //


    public void MagBoots()
    {
        if (Input.GetButtonUp(controls.magBoots) && !magBootsOn && !canClimbLedge)
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
        else if (Input.GetButtonUp(controls.magBoots) && magBootsOn && !canClimbLedge)
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
        yield break;
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
            canFlipSprite = false;

            animator.SetBool("canClimbLedge", canClimbLedge);
        }

        if (canClimbLedge)
        {
            transform.position = ledgePos1;
        }
    }

    public void FinishLedgeClimb() // Called from the animator
    {
        transform.position = ledgePos2;
        EnableMovement(true);
        ledgeDetected = false;
        canClimbLedge = false;
        canJump = true;
        canFlipSprite = true;
        animator.SetBool("canClimbLedge", canClimbLedge);
    }


    // ---- JUMP METHODS ---- //

    private void Jump()
    {
        if (!magBootsOn)
        {
            if (Input.GetButtonDown(controls.jump) && currentGraceTime > 0 && canJump)
            {
                velocity.y = jumpTakeoffSpeed;
                currentGraceTime = 0;
                canJump = false;
                animator.SetTrigger("jumping");
            }
            else if (Input.GetButtonUp(controls.jump)) // << -- for determining jump height 
            {
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * 0.5f;
                }
            }
        }

        if (Input.GetButtonUp(controls.jump))
        {
            canJump = true;
        }
    }

    bool quickJump = true;

    public void RapidJump()
    {
        if (!isGrounded && canJump && Input.GetButtonDown(controls.jump) && quickJump)
        {
            StartCoroutine(QuickJumpTimer());
        }
    }

    IEnumerator QuickJumpTimer()
    {
        quickJump = false;

        yield return new WaitForSeconds(jumpDelayTime);

        if (isGrounded)
        {
            print("Quick jump!");
            velocity.y = jumpTakeoffSpeed;
            animator.SetTrigger("jumping");
            currentGraceTime = 0;
            StopTrackAirTime();
            inAir = true;
        }
        else
        {
            print("Missed quick jump!");
        }

        quickJump = true;
        yield break;
    }

    void GraceJumpTimer()
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
                if (airTime > hardLandTime) // Light hard landing 
                {
                    animator.SetTrigger("land");
                    StartCoroutine(LandingPause(GetAnimTime()));
                    ripPP.CauseRipple(groundCheck, 12f, 0.8f);
                }
                else if (airTime > heavyLandTime) // Heavy hard landing 
                {
                    animator.SetTrigger("land");
                    StartCoroutine(LandingPause(GetAnimTime() + 0.5f));
                    ripPP.CauseRipple(groundCheck, 30f, 0.9f);
                }

                if (magBootsOn) // Landing with Mag Boots engaged
                {
                    animator.SetTrigger("land");
                    StartCoroutine(LandingPause(GetAnimTime()));
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
