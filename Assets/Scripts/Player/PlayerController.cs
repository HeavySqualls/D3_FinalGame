﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsObject
{
    public Transform respawnZone;

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
    public float timeFromZeroToMax = 2.5f;
    [Tooltip("Speed that the player will slide along the ground.")]
    public float groundSlideSpeed = 16f;
    public bool isGroundSliding = false;
    public Vector2 groundSlidingDirection;
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
    public float graceTimeStart = 0.12f;
    [Tooltip("Time it takes while holding the jump button to enable the 'Jump Flip' animation.")]
    public float jumpHoldTimeMax = 1.5f;
    public float airDisableTimer = 0.2f;
    private bool isPressingJumpButton = false;
    [SerializeField] private float currentGraceTime;
    private float airTime = 0f;
    [SerializeField] float jumpHoldTime = 0;
    private bool canJump = true;
    private bool canQuickJump = true;

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
    private int whatIsGround;

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

    //// Input Actions
    PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        InitInputDelegates();

        inputActions.Enable();

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        direction = Vector2.right;
        accessibleDirection = direction;
        ripPP = Camera.main.GetComponent<RipplePostProcessor>();

        accelRatePerSecond = (maxSpeed / timeFromZeroToMax) * accelSpeedMaxSpeed;
        decelRatePerSecond = (maxSpeed / timeFromZeroToMax) * decelSpeed;
    }

    public void InitInputDelegates()
    {
        // Movement
        inputActions.PlayerControls.Move.performed -= OnMove;
        inputActions.PlayerControls.Move.performed += OnMove;

        // Jump Started 
        inputActions.PlayerControls.Jump.started -= OnJumpStart;
        inputActions.PlayerControls.Jump.started += OnJumpStart;

        // Jump Performed
        inputActions.PlayerControls.Jump.performed -= OnJumpPerformed;
        inputActions.PlayerControls.Jump.performed += OnJumpPerformed;

        // Jump Canceled
        inputActions.PlayerControls.Jump.canceled -= OnJumpCanceled;
        inputActions.PlayerControls.Jump.canceled += OnJumpCanceled;

        // MagBoots - TODO: move this to combat?
        inputActions.PlayerControls.MagneticBoots.performed -= OnMagBoots;
        inputActions.PlayerControls.MagneticBoots.performed += OnMagBoots;
    }


    public void UninitInputDelegates()
    {
        inputActions.PlayerControls.Move.performed -= OnMove;
        inputActions.PlayerControls.Jump.started -= OnJumpStart;
        inputActions.PlayerControls.Jump.performed -= OnJumpPerformed;
        inputActions.PlayerControls.Jump.canceled -= OnJumpCanceled;

        inputActions.PlayerControls.MagneticBoots.performed -= OnMagBoots;
    }

    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        horizontalInput = obj.ReadValue<Vector2>().x;
    }

    private void OnMagBoots(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        MagBoots();
    }

    protected override void Start()
    {
        base.Start();
        whatIsGround = LayerMask.GetMask("Ground");

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

        isInputLeftORRight = horizontalInput > 0f || horizontalInput < 0f;

        CheckIfGroundSliding();
        CheckSurroundings();
        CheckIfWallSliding();
        TrackAirTime();
        ComputeVelocity();
        //Jump();
        //RapidJump();
        //MagBoots();
        GraceJumpTimer();
        CheckLedgeClimb();
    }

    public float GetAnimTime() //TODO: Convert the dependencies of this to a public float
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

    public Vector2 move;
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

    protected override void ComputeVelocity()
    {
        if (isWallJumping && !isInputLeftORRight)
        {
            return;
        }
        else if (canMove)
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
                    if (timeAtMaxSpeed == skidTimeLimit && !isSkidding && isLeft)
                    {
                        canFlipSprite = false;                    
                        timeAtMaxSpeed = 0;
                        StartCoroutine(PlayerSkid());
                    }

                    move.x += accelRatePerSecond * Time.deltaTime;
                }
                else if (horizontalInput < 0f) // Moving Left
                {
                    if (timeAtMaxSpeed == skidTimeLimit && !isSkidding && !isLeft)
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
            targetVelocity = move * maxSpeed;
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

    public GameObject goGround;

    private void CheckSurroundings()
    {
        if (isGrounded)
        {
            canWallSlide = true;
        }

        RaycastHit2D hitGround = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround); // check all the layers 
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

        if (hitGround.collider != null)
        {
            //TODO: FIND A BETTER WAY TO DO THIS: players collider never actually touches anything so hard to figure out how to enngage with these other objects 

            goGround = hitGround.collider.gameObject;

            // if current layer is breakable objects layer - then get component (each type of object needs its own layer)
            //reakableObject bo = goGround.GetComponent<BreakableObject>() - check if null then conditions in if statement 
            if (goGround.GetComponent<BreakableObject>() && goGround.GetComponent<BreakableObject>().isPlatform && !goGround.GetComponent<BreakableObject>().isFallingApart)
            {
                goGround.GetComponent<BreakableObject>().TriggerPlatformCollapse();
            }
            else if (goGround.GetComponent<BreakableFloor>())
            {
                if (inAir)
                {
                    goGround.GetComponent<BreakableFloor>().TriggerObjectShake();
                }
            }
            else if (goGround.GetComponent<SlidingSurface>()) // if the player is standing on a sliding surface, make them slide down 
            {
                GroundSlide(goGround.GetComponent<SlidingSurface>().direction);
            }
            else if (!goGround.GetComponent<SlidingSurface>()) // if they are no longer on the sliding surface, stop them from sliding
            {
                isGroundSliding = false;
            }
        }

        if (canMove)
        {
            isTouchingWall = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, whatIsGround);
            Debug.DrawRay(wallCheck.position, direction * wallCheckDistance, Color.red);

            isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, direction, ledgeCheckDistance, whatIsGround);
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
                RaycastHit2D hitWall = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, whatIsGround);
                if (hitWall.collider != null)
                {
                    var goWall = hitWall.collider.gameObject;

                    if (goWall.GetComponent<BreakableObject>())
                    {
                        goWall.GetComponent<BreakableObject>().TriggerPlatformCollapse();
                        canWallSlide = false;                  
                    }
                }
            }
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

    public void CanFlipSprite()
    {
        if (canFlipSprite)
            canFlipSprite = false;
        else
            canFlipSprite = true;
    }

    public void GroundSlide(Vector2 _slideDirection)
    {
        isGroundSliding = true;
        groundSlidingDirection = _slideDirection;
    }

    private void CheckIfGroundSliding()
    {
        if (isGroundSliding)
        {
            canMove = false;

            if (groundSlidingDirection == Vector2.right)
            {
                targetVelocity.x = groundSlideSpeed;
            }
            else
            {
                targetVelocity.x = -groundSlideSpeed;
            }

            if (direction != groundSlidingDirection)
            {
                CanFlipSprite();
            }
        }
        else
        {
            canMove = true;
        }
    }

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


    // ---- MAG BOOTS METHODS ---- //


    public void MagBoots()
    {
        if (!magBootsOn && !canClimbLedge)
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
        else if (magBootsOn && !canClimbLedge)
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

    private void OnJumpStart(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (currentGraceTime > 0 && !magBootsOn)
        {
            if (!isGrounded && canJump && canQuickJump)
            {
                StartCoroutine(QuickJumpTimer());
            }
            else
            {
                isPressingJumpButton = true;

                if (canJump || isWallSliding)
                {
                    StartCoroutine(JumpDelayTime());
                    currentGraceTime = 0;
                    canJump = false;
                    animator.SetTrigger("jumping");
                }
            }
        }
    }

    private void OnJumpCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!magBootsOn)
        {
            print("canceled");
            canJump = true;

            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }
    }

    private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!magBootsOn)
        {
            print("flip jump");
            animator.SetTrigger("jumpingFlip");
            isPressingJumpButton = false;
            jumpHoldTime = 0;
            canJump = true;
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
        else
        {
            velocity.y = jumpTakeoffSpeed;
        }
    }

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

    IEnumerator QuickJumpTimer()
    {
        canQuickJump = false;

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

        canQuickJump = true;
        yield break;
    }

    void GraceJumpTimer()
    {
        if (!isGrounded)
        {
            inAir = true;
            currentGraceTime -= Time.deltaTime;

            if (currentGraceTime < 0)
            {
                currentGraceTime = 0;
            }
        }
        else
        {
            currentGraceTime = graceTimeStart;
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
        isMoving = false;
        velocity.x = 0;
        move.x = 0;
        animator.SetBool("isMoving", isMoving);

        yield return new WaitForSeconds(_pauseTime);
        
        EnableMovement(true);
    }
}
