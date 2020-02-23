using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsObject
{
    public Transform respawnZone;
    public Vector2 move;

    [Space]
    [Header("INPUT:")]
    public bool isController = false;

    [Space]
    [Header("ACCEL/DECEL SPEEDS:")]
    [Tooltip("Speed of acceleration when not at max speed or in air.")]
    public float accelSpeedNormal = 1.5f;
    [Tooltip("Speed of acceleration when player is moving at max speed.")]
    public float accelSpeedMaxSpeed = 1;
    [Tooltip("Speed of acceleration when player is in the air.")]
    public float accelSpeedAir = 1.5f;
    [Tooltip("Speed of decceleration when player has stopped moving.")]
    public float decelSpeed = 1;
    [Tooltip("Time it takes while moving at full speed to enable a slide on direction change.")]
    public float skidTimeLimit = 2f;
    [Tooltip("Time it takes to reach full speed.")]
    private float timeFromZeroToMax = 2.5f;
    private float accelRatePerSecond;
    private float decelRatePerSecond;
    private float timeAtMaxSpeed = 0;
    private bool isLeft = false;

    [Space]
    [Header("MOVEMENT / WIND:")]
    public bool canMove = true;
    public bool isMoving = false;
    public bool isMovingInWind = false;
    [Tooltip("Maximum movement speed.")]
    public float maxSpeed = 3.75f;
    public Vector2 accessibleDirection; // a player direction vector that other scripts can read and use without making the physics object public
    private bool windAffectUnit = true;
    private bool pIsFaceLeft;
    private bool canFlipSprite = true;
    private bool backToWind = true;
    private float windRatio; // Ratio between the wind power and the players velocity

    [Space]
    [Header("JUMP:")]
    public bool inAir;
    [Tooltip("Initial vertical boost speed.")]
    public float jumpTakeoffSpeed = 6f;
    [Tooltip("Time in air required to enable the 'Hard Landing' animation.")]
    public float hardLandTime = 1f;
    [Tooltip("Time in air required to enable the 'Heavy Landing' animation.")]
    public float heavyLandTime = 1.5f;
    [Tooltip("Time between the jump button pressed and the jump velocity added to the player sprite.")]
    public float jumpDelayTime = 0.02f;
    [Tooltip("Maximum time for the 'Quick Jump' - buffer time between button press while in the air and the player landing on the ground to trigger a quick jump.")]
    public float quickJumpDelay = 0.02f;
    [Tooltip("Time from when the player leaves the ground without jumping until they are no longer allowed to jump.")]
    public float maxGraceTime = 0.12f;
    [Tooltip("Time it takes while holding the jump button to enable the 'Jump Flip' animation.")]
    public float jumpHoldTimeMax = 1.5f;
    public float airDisableTimer = 0.2f;
    private bool isPressingJumpButton = false;
    private float currentGraceTime;
    private float airTime = 0f;
    float currentJumpDelayTime = 0;
    [SerializeField] float jumpHoldTime = 0;
    [SerializeField] private bool canJump = true;
    private bool quickJump = true;

    [Space]
    [Header("MAG BOOTS:")]
    public bool magBootsOn = false;
    [Tooltip("Rate at which the player gets pulled down towards the ground with the magnetic boots enabled.")]
    public float onGravValue = 10f;
    public ParticleSystem bootSparks;

    [Space]
    [Header("GROUND CHECK:")]
    public Vector3 bottom;
    [Tooltip("Distance of ground check raycast from the bottom of the player sprite.")]
    public float groundCheckDistance = 1.75f;
    public Transform groundCheck; // for determining quick landing jump
    public float groundSlideSpeed = 18f;
    private int whatIsGround;
    private bool isGroundSliding;

    [Space]
    [Header("WALL CHECK:")]
    [Tooltip("Distance of the wall check raycast from the chest of the player sprite.")]
    public float wallCheckDistance = 1;
    public float wallSlidingSpeed;
    public Vector2 wallJumpDirection;
    public float wallJumpForce = 3.25f;
    public Transform wallCheck; // for checking if against wall
    public bool isWallSliding = false;
    private bool isTouchingWall = false;
    private bool canWallSlide = true;

    [Space]
    [Header("LEDGE CHECK:")]
    public bool canClimbLedge = false;
    [Tooltip("Distance of the ledge check raycast from the head of the player sprite.")]
    public float ledgeCheckDistance = 1;
    public Transform ledgeCheck; // for checking if on the edge of a collider
    [Tooltip("Position of the player sprites x axis when hanging from a wall (right).")]
    public float ledgeClimbXOffset1 = 0f;
    [Tooltip("Position of the player sprites y axis when hanging from a wall (right).")]
    public float ledgeClimbYOffset1 = 0f;
    [Tooltip("Position of the player sprites x axis when hanging from a wall (left).")]
    public float ledgeClimbXOffset2 = 0f;
    [Tooltip("Position of the player sprites y axis when hanging from a wall (left).")]
    public float ledgeClimbYOffset2 = 0f;
    private bool isTouchingLedge = false;
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


    private LayerMask groundLayerMask;
    private int groundLayer = 8;
    private int breakableFloorsLayer = 17;
    private int slidingSurfaceLayer = 18;
    private int breakableObjectsLayer = 19;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        direction = Vector2.right;
        accessibleDirection = direction;
        ripPP = Camera.main.GetComponent<RipplePostProcessor>();

        accelRatePerSecond = (maxSpeed / timeFromZeroToMax) * accelSpeedMaxSpeed;
        decelRatePerSecond = (maxSpeed / timeFromZeroToMax) * decelSpeed;
    }

    protected override void Start()
    {
        base.Start();
        whatIsGround = LayerMask.GetMask("Ground");

        groundLayerMask = ((1 << groundLayer)) | ((1 << breakableFloorsLayer)) | ((1 << slidingSurfaceLayer)) | ((1 << breakableObjectsLayer));

        if (controls != null)
        {
            if (isController)
                controls.ControllerMovement();
            else
                controls.KeyboardMovement();
        }
        else
            Debug.Log("Player does not have the controls component attached!");
    }

    float horizontalInput;
    bool isInputLeftORRight;

    protected override void Update()
    {
        base.Update();

        horizontalInput = Input.GetAxisRaw(controls.xMove);
        isInputLeftORRight = horizontalInput > 0f || horizontalInput < 0f;

        CheckSurroundings();
        CheckIfWallSliding();
        TrackAirTime();
        //ComputeVelocity(); // Realised that this was happening twice, here AND in physics object. Disabled it here for now to see the effects. 
        Jump();
        RapidJump();
        MagBoots();
        GraceJumpTimer();
        CheckLedgeClimb();
    }

    public float GetAnimTime() //TODO: Figure out why GetCurrentAnimatorClipInfo isn't returning the correct animation clip
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

    // <<----------------------------------------------------- LOCOMOTION METHODS ------------------------------------------- //

    bool isSkidding = false;
    bool isAtMaxSpeed = false;

    IEnumerator PlayerSkid()
    {
        print("Skidding");
        isSkidding = true;

        yield return new WaitForSeconds(GetAnimTime() / 4.25f);

        canFlipSprite = true;
        isSkidding = false;

        yield break;
    }

    public IEnumerator PlayerKnocked(Vector2 _hitDirection, float _knockBack, float _knockUp, float _stunTime)
    {
        print("Knocked!");
        canJump = false;
        canMove = false;
        canWallSlide = false;
        velocity.y += _knockUp;
        targetVelocity.x += _knockBack;

        yield return new WaitForSeconds(_stunTime);

        canJump = true;
        canMove = true;
        canWallSlide = true;
        targetVelocity.x = 0;
        yield break;
    }

    
    // <<----------------------------------------------------- COMPUTE VELOCITY (IN AND OUT OF WIND ZONES) ------------------------------------------- //


    protected override void ComputeVelocity()
    {
        // Checks to determine what acceleration speed the player will have depending on the situation
        if (inAir) // if the player is in the air
        {
            accelRatePerSecond = (maxSpeed / timeFromZeroToMax) * accelSpeedAir;
        }
        else if (isAtMaxSpeed) // if the player is running at max speed for "x" seconds
        {
            accelRatePerSecond = (maxSpeed / timeFromZeroToMax) * accelSpeedMaxSpeed;
        }
        else
        {
            accelRatePerSecond = (maxSpeed / timeFromZeroToMax) * accelSpeedNormal;
        }



        if (isWallJumping && !isInputLeftORRight)
        {
            return;
        }
        else if (canMove)
        {
            // Check to see if player is moving at max speed, and start counting seconds until the skid time limit is reached 
            // - then when player changes direction, they will skid 
            if (velocity.x <= -maxSpeed || velocity.x >= maxSpeed)
            {
                timeAtMaxSpeed += 0.1f;

                if (timeAtMaxSpeed > skidTimeLimit)
                {
                    timeAtMaxSpeed = skidTimeLimit;
                }
            }
            else if (velocity.x > -maxSpeed || velocity.x < maxSpeed)
            {
                timeAtMaxSpeed = 0;
            }

            // Determine if there is input
            if (isInputLeftORRight) 
            {
                isMoving = true;              

                if (horizontalInput > 0f) // Moving Right
                {
                    if (timeAtMaxSpeed == skidTimeLimit && !isSkidding && isLeft && !inWindZone)
                    {
                        canFlipSprite = false;                    
                        timeAtMaxSpeed = 0;
                        StartCoroutine(PlayerSkid());
                    }

                    move.x += accelRatePerSecond * Time.deltaTime;
                }
                else if (horizontalInput < 0f) // Moving Left
                {
                    if (timeAtMaxSpeed == skidTimeLimit && !isSkidding && !isLeft && !inWindZone)
                    {
                        print("2");
                        canFlipSprite = false;
                        timeAtMaxSpeed = 0;
                        StartCoroutine(PlayerSkid());                      
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
                        move.x = Mathf.Clamp(move.x, -maxSpeed * (windDir.x * windPwr), 2.3f / (windDir.x * windPwr));                      
                    }
                    else if (!windMovingRight)
                    {
                        move.x = Mathf.Clamp(move.x, 2.3f / (windDir.x * windPwr), -maxSpeed * (windDir.x * windPwr));
                    }
                }
                else if (!inWindZone)
                {
                    move.x = Mathf.Clamp(move.x, -maxSpeed, maxSpeed);
                }
            }         
            else if (!isInputLeftORRight)  // Determine if input has stopped
            {
                if (inWindZone) // If movement has stopped and player is in a windzone
                {
                    if (!magBootsOn) // If the player does not have boots activated - add wind force to idle player
                    {
                        move.x = windDir.x / windPwr;

                        // Determine wind ratio - The peak velocity that the player rests at when idle and being pushed back by the wind 
                        if (windRatio == 0)
                        {
                            windRatio = move.x * maxSpeed;
                            print(windRatio);
                        }
                    }
                    else // If the player HAS boots activated, they do not move 
                    {
                        move.x = 0;
                    }

                    if (isMoving)
                    {                   
                        isMoving = false;
                        isMovingInWind = false;
                    }
                }
                else
                {
                    windRatio = 0;

                    if (move.x != 0)
                    {
                        if (direction == Vector2.left)
                        {                            
                            move.x += decelRatePerSecond * Time.deltaTime;

                            if (move.x >= 0)
                            {                              
                                move.x = 0;
                                isMoving = false;
                                canFlipSprite = true;
                            }
                        }
                        else if (direction == Vector2.right)
                        {                           
                            move.x -= decelRatePerSecond * Time.deltaTime;

                            if (move.x <= 0)
                            {
                                move.x = 0;
                                isMoving = false;
                                canFlipSprite = true;
                            }
                        }
                    }
                }
            }
            // Send the move Vector will all related forces to the Physics Object
            targetVelocity = move * maxSpeed; // DO NOT MOVE FROM THIS LOCATION!!
        }

        // Determine if a player is up against a wall - if so, disable the wind effect while they are there 
        if (isTouchingWall && inWindZone)
            windAffectUnit = false;
        else
            windAffectUnit = true;

        // Determine direction that the sprite will face
        if (canFlipSprite && !isWallSliding)
        {
            if (inWindZone && isMovingInWind)
            {
                ComputeDirectionOfSpriteInWind();
            }
            else if (!inWindZone)
            {
                FlipSprite();
            }
        }

        // Animation settings
        animator.SetBool("isGroundSliding", isGroundSliding);
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isSkid", isSkidding);
        animator.SetBool("grounded", isGrounded);
        animator.SetBool("inWind", inWindZone);
        animator.SetBool("isBackToWind", backToWind);
        animator.SetBool("isMovingInWind", isMovingInWind);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
    }

    private void ComputeDirectionOfSpriteInWind()
    {
        if (windMovingRight & !isTouchingWall)
        {
            if (velocity.x > windRatio && pIsFaceLeft) // If player is moving with the wind - face to the right 
            {
                ChangeDirection();
                pIsFaceLeft = !pIsFaceLeft;
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
            else if (velocity.x < 0 && !pIsFaceLeft) // If the player is moving against the wind - face to the left 
            {
                ChangeDirection();
                pIsFaceLeft = !pIsFaceLeft;
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
        }
        else if (!windMovingRight)
        {
            if (velocity.x > 0 && pIsFaceLeft && !isTouchingWall) // If the player is moving against the wind - face to the right
            {
                ChangeDirection();
                pIsFaceLeft = !pIsFaceLeft;
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
            else if (velocity.x < windRatio && !pIsFaceLeft && isMovingInWind && !isTouchingWall) // If the player is moving with the wind - face to the left
            {
                ChangeDirection();
                pIsFaceLeft = !pIsFaceLeft;
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
        }

        if (direction != windDir)
        {
            backToWind = false;
        }
        else if (direction == windDir)
        {
            backToWind = true;
        }
    }


    // <<----------------------------------------------------- CHECK SURROUNDINGS ------------------------------------------- //


    private void CheckSurroundings()
    {
        RaycastHit2D hitGround = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayerMask);
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

        if (hitGround.collider != null)
        {
            int currentLayer = hitGround.collider.gameObject.layer;

            if (currentLayer == breakableObjectsLayer)
            {
                BreakableObject bo = hitGround.collider.gameObject.GetComponent<BreakableObject>();

                if (bo == null)
                {
                    Debug.LogError("Object on Breakable Objects layer does not have Breakable Object component!");
                }
                else if (bo.isPlatform && !bo.isFallingApart)
                {
                    bo.TriggerPlatformCollapse();
                }
            }
            else if (currentLayer == breakableFloorsLayer)
            {
                BreakableFloor bf = hitGround.collider.gameObject.GetComponent<BreakableFloor>();

                if (bf == null)
                {
                    Debug.LogError("Object on Breakable Floors layer does not have Breakable Floors component!");
                }
                else if (inAir)
                {
                    bf.TriggerObjectShake();
                }
            }
            else if (currentLayer == slidingSurfaceLayer && !isPressingJumpButton && !magBootsOn)
            {
                SlidingSurface ss = hitGround.collider.gameObject.GetComponent<SlidingSurface>();

                if (ss == null)
                {
                    Debug.LogError("Object on Sliding Surface layer does not have Sliding Surface component!");
                }
                else
                {
                    GroundSlide(ss.direction);
                }
            }
            else if (currentLayer == groundLayer && isGroundSliding || magBootsOn)
            {
                StopGroundSlide();
            }
        }
        else if(hitGround.collider == null && isGroundSliding)
        {
            if (direction == Vector2.right)
            {
                targetVelocity.x = 16;
            }
            else
            {
                targetVelocity.x = -16;
            }
        }

        if (canMove)
        {
            isTouchingWall = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, groundLayerMask);
            Debug.DrawRay(wallCheck.position, direction * wallCheckDistance, Color.red);

            isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, direction, ledgeCheckDistance, groundLayerMask);
            Debug.DrawRay(ledgeCheck.position, direction * ledgeCheckDistance, Color.red);

            // Check for ledge grab
            if (isTouchingWall && !isTouchingLedge && !ledgeDetected)// << ----------- OPTION TO PUT MANUAL LEDGE GRAB HERE 
            {
                ledgeDetected = true;
                ledgePosBot = wallCheck.position;
            }

            // Check for crumbling wall
            if (isWallSliding)
            {
                RaycastHit2D hitWall = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, groundLayerMask);

                if (hitWall.collider != null)
                {
                    int currentLayer = hitWall.collider.gameObject.layer;

                    if (currentLayer == breakableObjectsLayer)
                    {
                        BreakableObject boWall = hitWall.collider.gameObject.GetComponent<BreakableObject>();

                        if (boWall == null)
                        {
                            Debug.LogError("Object on Breakable Object layer does not have Breakable Object component!");
                        }
                        else
                        {
                            boWall.TriggerPlatformCollapse();
                            canWallSlide = false;
                        }
                    }
                }
            }
        }
    }


    // <<----------------------------------------------------- GROUND SLIDE ------------------------------------------- //

    private Vector2 slideDirection;

    private void GroundSlide(Vector2 _slideDirection)
    {
        canMove = false;
        isGroundSliding = true;
        slideDirection = _slideDirection;

        if (_slideDirection == Vector2.right)
        {
            targetVelocity.x = groundSlideSpeed;
        }
        else
        {
            targetVelocity.x = -groundSlideSpeed;
        }

        velocity.y = -15;

        if (direction != _slideDirection)
        {
            FlipSprite();
        }
    }

    private void StopGroundSlide()
    {
        isGroundSliding = false;
        canMove = true;
    }

    // <<----------------------------------------------------- WALL SLIDE ------------------------------------------- //

    private void CheckIfWallSliding()
    {
        if (canWallSlide && isTouchingWall && !isGrounded && isTouchingLedge && velocity.y <= 0)
        {
            isWallSliding = true;
            gravityModifier = wallSlidingSpeed;
        }
        else
        {
            isWallSliding = false;
            gravityModifier = gravStart;
        }

        animator.SetBool("isWallSliding", isWallSliding);
    }


    // <<----------------------------------------------------- CHANGE DIRECTION / FLIP SPRITE ------------------------------------------- //

    private void FlipSprite()
    {
        float minXFlip = 0f; // minimum velocity on the x axis to trigger the sprite flip                   
        bool flipPlayerSprite = (spriteRenderer.flipX ? (velocity.x > minXFlip) : (velocity.x < minXFlip));

        if (flipPlayerSprite)
        {
            print("flip");
            ChangeDirection();
            pIsFaceLeft = !pIsFaceLeft;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    private void ChangeDirection()
    {
        if (canFlipSprite)
        {
            if (direction == Vector2.right)
            {
                direction = Vector2.left;
                isLeft = true;
            }             
            else
            {
                direction = Vector2.right;
                isLeft = false;
            }               

            accessibleDirection = direction;
        }
    }


    // <<-----------------------------------------------------MAG BOOTS METHODS ------------------------------------------- //


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


    // <<----------------------------------------------------- LEDGE CLIMB METHODS ------------------------------------------- //


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


    // <<----------------------------------------------------- JUMP METHODS ------------------------------------------- //


    private void Jump()
    {
        if (!magBootsOn)
        {
            if (Input.GetButtonDown(controls.jump))
            {
                isPressingJumpButton = true;

                if (currentGraceTime > 0 && canJump || isWallSliding)
                {
                    StartCoroutine(JumpDelayTime());
                    currentGraceTime = 0;
                    canJump = false;
                }
            }
            else if (Input.GetButtonUp(controls.jump)) // << -- for determining jump height 
            {
                isPressingJumpButton = false;
                canJump = true;

                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * 0.5f;
                }
            }
        }

        // Start timer to determine jump height and proper animation
        if (isPressingJumpButton)
        {
            jumpHoldTime += Time.deltaTime;

            if (jumpHoldTime > jumpHoldTimeMax)
            {
                print("flip jump");
                animator.SetTrigger("jumpingFlip");
                isPressingJumpButton = false;
                jumpHoldTime = 0;
            }
        }
        else
        {
            jumpHoldTime = 0;
        }
    }

    IEnumerator JumpDelayTime()
    {
        yield return new WaitForSeconds(jumpDelayTime);

        if (isWallSliding)
        {
            StartCoroutine(WallJumpTimer());
            PushOffWall();
        }
         else if (isGroundSliding)
        {
            velocity.y = jumpTakeoffSpeed;
            PushOffSlidingSurface();
        }
        else
        {
            velocity.y = jumpTakeoffSpeed;
        }

        animator.SetTrigger("jumping");
    }


    // <<----------------------------------------------------- SLIDING SURFACE JUMP ------------------------------------------- //

    float slidingSurfaceJumpForceY = 28;

    // TODO: Figure out why jumping in the opposite direction of the slope causes the player to travel further than jumping with the direction of the slope
    private void PushOffSlidingSurface() 
    {
        isGroundSliding = false;
        canMove = true;
        velocity.y = slidingSurfaceJumpForceY;
    }


    // <<----------------------------------------------------- WALL JUMP ------------------------------------------- //


    IEnumerator WallJumpTimer()
    {
        velocity.y = 6.5f * maxSpeed;
        isWallJumping = true;
        canMove = false;

        yield return new WaitForSeconds(airDisableTimer);

        isWallJumping = false;
        canMove = true;

        if (direction == Vector2.right)
        {
            move.x = 14f;
        }
        else
        {
            move.x = -14f;
        }  
    }

    private void PushOffWall()
    {
        Debug.Log("WALL JUMP");

        if (direction == Vector2.right)
        {
            targetVelocity.x = 0;
            targetVelocity.x += -wallJumpForce * maxSpeed;
        }
        else
        {
            targetVelocity.x = 0;
            targetVelocity.x += wallJumpForce * maxSpeed;
        }
    }


    // <<----------------------------------------------------- ADDITIONAL JUMP METHODS ------------------------------------------- //


    IEnumerator QuickJumpTimer()
    {
        quickJump = false;

        yield return new WaitForSeconds(quickJumpDelay);

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

    public void RapidJump()
    {
        if (!isGrounded && canJump && Input.GetButtonDown(controls.jump) && quickJump)
        {
            StartCoroutine(QuickJumpTimer());
        }
    }

    void TrackAirTime() 
    {
        if (inAir)
        {
            airTime += Time.deltaTime;

            if (isGrounded)
            {
                canWallSlide = true;

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


    // <<----------------------------------------------------- UTILITY ------------------------------------------- //


    public void EnableMovement(bool _enable) // Called by the Animator at the moment
    {
        canMove = _enable;
    }

    private IEnumerator LandingPause(float _pauseTime)
    {
        EnableMovement(false);
        isMoving = false;
        velocity.x = 0;
        move.x = 0;
        animator.SetBool("isMoving", isMoving);

        yield return new WaitForSeconds(_pauseTime);
        
        EnableMovement(true);
    }
}
