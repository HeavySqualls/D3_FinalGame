using System.Collections;
using UnityEngine;

public class PlayerController : PhysicsObject
{
    [Space]
    [Header("--- PLAYER CONTROLLER ---")]
    public Vector2 move;

    [Space]
    [Header("Input:")]
    public bool isDisabled = false;
    public bool isController = false;

    [Space]
    [Header("Accel & Deccel Speeds:")]
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
    [Header("Movement & Wind:")]
    public bool canMove = true;
    public bool isMoving = false;
    public bool isMovingInWind = false;
    [Tooltip("Maximum movement speed.")]
    public float maxSpeed = 3.75f;
    public Vector2 accessibleDirection; // a player direction vector that other scripts can read and use without making the physics object public
    public bool backToWind = true;
    private bool windAffectUnit = true;
    private bool pIsFaceLeft;
    private bool canFlipSprite = true;
    private float windRatio; // Ratio between the wind power and the players velocity

    [Space]
    [Header("Jump:")]
    public bool inAir;
    public bool canJump = true;
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
    public float landDelay = 0.5f;
    public float slidingSurfaceJumpForceY = 25f;
    private bool isPressingJumpButton = false;
    private float currentGraceTime;
    public float airTime = 0f;
    private float jumpHoldTime = 0;
    private bool quickJump = true;

    [Space]
    [Header("Mag Boots:")]
    public bool isMagBootsEnabled = true;
    public bool magBootsOn = false;
    [Tooltip("Rate at which the player gets pulled down towards the ground with the magnetic boots enabled.")]
    public float onGravValue = 10f;
    public ParticleSystem bootSparks;

    [Space]
    [Header("Ground Check:")]
    [Tooltip("Distance of ground check raycast from the bottom of the player sprite.")]
    public float groundCheckDistance = 1.75f;
    public Transform groundCheck; // for determining quick landing jump
    public float groundSlideSpeed = 18f;
    public bool isGroundSliding;

    [Space]
    [Header("Wall Check:")]
    [Tooltip("Distance of the wall check raycast from the chest of the player sprite.")]
    public float wallCheckDistance = 1;
    public float wallSlidingSpeed;
    public Vector2 wallJumpDirection;
    public float wallJumpForce = 3.25f;
    public Transform wallCheck; // for checking if against wall
    public bool isWallSliding = false;
    public float terminalWallSlidingVelocity = 20f;
    public bool isWallJumping = false;
    private bool isTouchingWall = false;
    private bool canWallSlide = true;

    [Space]
    [Header("Ledge Check:")]
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
    [Header("References:")]
    public Controls controls;
    public Animator animator;
    private PlayerFeedback pFeedback;
    private PlayerAudioController pAudio;
    private PlayerHealthSystem pHealth;
    private RipplePostProcessor ripPP;
    private SpriteRenderer spriteRenderer;
    private LayerMask groundLayerMask;
    private LayerMask wallLayerMask;
    private int groundLayer = 8;
    private int breakableFloorsLayer = 17;
    private int slidingSurfaceLayer = 18;
    private int breakableObjectsLayer = 19;
    private int rollingObjectsLayer = 15;

    void Awake()
    {
        Toolbox.GetInstance().GetPlayerManager().SetPlayerController(this);
        isController = Toolbox.GetInstance().GetPlayerManager().GetControlType();

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

        pFeedback = GetComponent<PlayerFeedback>();
        pAudio = GetComponent<PlayerAudioController>();
        pHealth = GetComponent<PlayerHealthSystem>();

        groundLayerMask = ((1 << groundLayer)) | ((1 << breakableFloorsLayer)) | ((1 << slidingSurfaceLayer)) | ((1 << breakableObjectsLayer)) | ((1 << rollingObjectsLayer));
        wallLayerMask = ((1 << groundLayer)) | ((1 << breakableFloorsLayer)) | ((1 << breakableObjectsLayer));

        //if (controls != null)
        //{
        //    if (isController)
        //        controls.ControllerMovement();
        //    else
        //        controls.KeyboardMovement();
        //}
        //else
        //    Debug.Log("Player does not have the controls component attached!");
    }

    float horizontalInput;
    bool isInputLeftORRight;

    protected override void Update()
    {
        base.Update();

        if (!isDisabled)
        {
            if (isController)
            {
                horizontalInput = Input.GetAxisRaw(controls.xMoveController);
            }
            else
            {
                horizontalInput = Input.GetAxisRaw(controls.xMoveKeys);
            }

            isInputLeftORRight = horizontalInput > 0f || horizontalInput < 0f;
            CheckSurroundings();
            CheckIfWallSliding();
            TrackAirTime();
            ComputeVelocity();
            Jump();
            RapidJump();
            MagBoots();
            GraceJumpTimer();
            CheckLedgeClimb();
        }

        RunAnimations();

        if (isWallSliding && velocity.y < -terminalWallSlidingVelocity)
        {
            velocity.y = -terminalWallSlidingVelocity;
        }
        else if (velocity.y < -terminalVelocity)
        {
            velocity.y = -terminalVelocity;
        }

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
        pAudio.PlaySlideSound();

        yield return new WaitForSeconds(GetAnimTime() / 4.25f);

        canFlipSprite = true;
        isSkidding = false;

        yield break;
    }

    public bool isHit = false;

    public IEnumerator PlayerKnocked(Vector2 _hitDirection, float _knockBack, float _knockUp, float _stunTime)
    {
        //print(gameObject.name + " was hit!");

        isHit = true;
        DisablePlayerController();

        float timer = 0.0f;

        while (timer < _stunTime)
        {
            velocity.y += _knockUp;
            targetVelocity.x += _knockBack;
            timer += Time.deltaTime;
            yield return null;
        }

        targetVelocity.x = 0;
        isHit = false;

        if (!pHealth.isDead)
            EnablePlayerController();

        yield break;
    }


    // <<----------------------------------------------------- COMPUTE VELOCITY (IN AND OUT OF WIND ZONES) ------------------------------------------- //
    public void SetPlayerVelocityToZero()
    {
        targetVelocity.x = 0;
        move.x = 0;
    }

    protected override void ComputeVelocity()
    {
        //targetVelocity = Vector2.zero;

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
                if (inWindZone)
                    isMovingInWind = true;
                
                isMoving = true;              

                if (horizontalInput > 0f) // Moving Right
                {
                    if (timeAtMaxSpeed == skidTimeLimit && !isSkidding && isLeft && !windAffectMovement)
                    {
                        canFlipSprite = false;                    
                        timeAtMaxSpeed = 0;
                        StartCoroutine(PlayerSkid());
                    }

                    move.x += accelRatePerSecond * Time.deltaTime;
                }
                else if (horizontalInput < 0f) // Moving Left
                {
                    if (timeAtMaxSpeed == skidTimeLimit && !isSkidding && !isLeft && !windAffectMovement)
                    {
                        canFlipSprite = false;
                        timeAtMaxSpeed = 0;
                        StartCoroutine(PlayerSkid());                      
                    }

                    move.x -= accelRatePerSecond * Time.deltaTime;                                
                }

                //TODO: Find out why this is stronger with a smaller number?

                // Add force from wind to players move.x if applicable
                if (inWindZone && windAffectMovement)
                {                
                    if (windAffectUnit)
                    {
                        if (magBootsOn)
                        {
                            move.x = Mathf.Clamp(move.x, -maxSpeed, maxSpeed);
                        }
                        else if (isFromLeft)
                        {
                            move.x = Mathf.Clamp(move.x, -maxSpeed * (windDir.x * windPwr), 2.3f / (windDir.x * windPwr));
                        }
                        else if (!isFromLeft)
                        {
                            move.x = Mathf.Clamp(move.x, 2.3f / (windDir.x * windPwr), -maxSpeed * (windDir.x * windPwr));
                        }
                    }        
                }
                else if (!inWindZone || inWindZone && !windAffectMovement)
                {
                    move.x = Mathf.Clamp(move.x, -maxSpeed, maxSpeed);
                }
            }         
            else if (!isInputLeftORRight)  // Determine if input has stopped
            {
                if (isMoving)
                {
                    isMoving = false;
                    isMovingInWind = false;
                }

                if (inWindZone && windAffectMovement) // If movement has stopped and player is in a windzone
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
                }                    
                else if(!inWindZone || inWindZone && !windAffectMovement)
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
                FlipSpriteBasedOnInput();
            }
        }
    }

    private void RunAnimations()
    {
        // Animation settings
        animator.SetBool("isGroundSliding", isGroundSliding);
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isSkid", isSkidding);
        animator.SetBool("grounded", isOnGround/*isGrounded*/);
        animator.SetBool("inWind", inWindZone);
        animator.SetBool("isBackToWind", backToWind);
        animator.SetBool("isMovingInWind", isMovingInWind);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
    }

    private void ComputeDirectionOfSpriteInWind() // TODO: Solve the issue where the boras wind confuses the players sprite flip
    {
        if (isFromLeft)
        {
            if (velocity.x > windRatio && pIsFaceLeft) // If player is moving with the wind - face to the right 
            {
                ChangeDirection();
                pIsFaceLeft = !pIsFaceLeft;
                spriteRenderer.flipX = !spriteRenderer.flipX;
                print("Player direction = " + direction);
            }
            else if (velocity.x < 0 && !pIsFaceLeft) // If the player is moving against the wind - face to the left 
            {
                ChangeDirection();
                pIsFaceLeft = !pIsFaceLeft;
                spriteRenderer.flipX = !spriteRenderer.flipX;
                print("Player direction = " + direction);
            }
        }
        else if (!isFromLeft)
        {
            if (velocity.x > 0 && pIsFaceLeft && !isTouchingWall) // If the player is moving against the wind - face to the right
            {
                ChangeDirection();
                pIsFaceLeft = !pIsFaceLeft;
                spriteRenderer.flipX = !spriteRenderer.flipX;
                print("Player direction = " + direction);
            }
            else if (velocity.x < windRatio && !pIsFaceLeft && isMovingInWind && !isTouchingWall) // If the player is moving with the wind - face to the left
            {
                ChangeDirection();
                pIsFaceLeft = !pIsFaceLeft;
                spriteRenderer.flipX = !spriteRenderer.flipX;
                print("Player direction = " + direction);
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


    // <<----------------------------------------------------- CHANGE DIRECTION / FLIP SPRITE ------------------------------------------- //

    private void FlipSpriteBasedOnInput()
    {
        if (canFlipSprite)
        {
            bool flipPlayerSprite = (spriteRenderer.flipX ? (horizontalInput > 0f) : (horizontalInput < 0f));

            if (flipPlayerSprite)
            {
                //print("flip");
                ChangeDirection();
            }
        }
    }

    private void ChangeDirection()
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

        if (!inWindZone)
        {
            pIsFaceLeft = !pIsFaceLeft;
            spriteRenderer.flipX = !spriteRenderer.flipX;
            accessibleDirection = direction;
        }
    }


    // <<----------------------------------------------------- CHECK SURROUNDINGS ------------------------------------------- //

    public bool isOnGround = false;

    private void CheckSurroundings()
    {
        // ---- CHECK FOR GROUND

        RaycastHit2D hitGround = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayerMask);
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

        if (hitGround.collider != null)
        {
            isOnGround = true;

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
                    SlopeSlide(ss.direction);
                }
            }
            else if (currentLayer != slidingSurfaceLayer && isGroundSliding || isGroundSliding && magBootsOn)
            {
                print("Stop");
                StopSlopeSlide();
            }
        }
        else if (hitGround.collider == null && isGroundSliding)
        {
            if (direction == Vector2.right)
            {
                move.x = 16;
            }
            else
            {
                move.x = -16;
            }

            targetVelocity.x = move.x;
        }
        else if (hitGround.collider == null)
        {
            isOnGround = false;
        }

        // ---- CHECK FOR WALLS

        if (canMove && !isDisabled)
        {
            isTouchingWall = Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, wallLayerMask);
            Debug.DrawRay(wallCheck.position, direction * wallCheckDistance, Color.red);

            isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, direction, ledgeCheckDistance, wallLayerMask);
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
                        BreakableObject crumblingWall = hitWall.collider.gameObject.GetComponent<BreakableObject>();

                        if (crumblingWall != null)
                        {
                            print("hit crumbling wall");
                            crumblingWall.TriggerPlatformCollapse();
                            StopWallSliding();
                        }
                        else
                            Debug.LogError("Object on Breakable Object layer does not have Breakable Object component!");
                    }
                }
            }
        }


    }


    // <<----------------------------------------------------- SLOPE SLIDE ------------------------------------------- //

    private Vector2 slideDirection;

    private void SlopeSlide(Vector2 _slideDirection)
    {
        if (!magBootsOn)
        {
            canMove = false;
            //isGrounded = true;
            if (!isGroundSliding)
            {
                isTouchingWall = false;
                isWallSliding = false;
                isGroundSliding = true;
            }

            if (direction != _slideDirection)
            {
                ChangeDirection();
            }

            canFlipSprite = false;

            animator.SetBool("isGroundSliding", isGroundSliding);

            slideDirection = _slideDirection;

            if (_slideDirection == Vector2.right)
            {
                move.x = groundSlideSpeed;
            }
            else
            {
                move.x = -groundSlideSpeed;
            }

            targetVelocity.x = move.x;
            velocity.y = -15;
        }
        else
        {
            isGroundSliding = false;
        }
        
    }

    private void StopSlopeSlide()
    {
        canFlipSprite = true;
        isGroundSliding = false;
        move.x = 0;
        canMove = true;
    }

    // <<----------------------------------------------------- WALL SLIDE ------------------------------------------- //

    private void CheckIfWallSliding()
    {
        if (!isDisabled)
        {
            if (canWallSlide && isTouchingWall && /*!isGrounded*/ !isOnGround && isTouchingLedge && velocity.y <= 0)
            {
                isWallSliding = true;
                StopTrackAirTime();
                gravityModifier = wallSlidingSpeed;
            }
            else if ((!isTouchingWall || isOnGround))
            {
                isWallSliding = false;
                gravityModifier = gravStart;
            }
        }

        animator.SetBool("isWallSliding", isWallSliding);
    }

    public void StopWallSliding()
    {
        isWallSliding = false;
        canWallSlide = false;
        canJump = false;
    }

    // <<----------------------------------------------------- MAG BOOTS METHODS ------------------------------------------- >> //

    //bool coroutineRunning = false;
    bool bootsSwitchedOn = false;

    public void MagBoots()
    {
        if (isMagBootsEnabled)
        {
            if (Input.GetButton(controls.magBoots)/* && !magBootsOn*/ && !canClimbLedge)
            {
                if (!isGrounded && !bootsSwitchedOn)
                {
                    StartCoroutine(MagBootsOn());
                    bootsSwitchedOn = true;
                }
                else if (isGrounded && !bootsSwitchedOn)
                {
                    magBootsOn = true;
                    print("MagBoots Activated: " + magBootsOn);
                    bootsSwitchedOn = true;
                    rb2d.velocity = Vector3.zero;
                    gravityModifier = onGravValue;
                }

                ripPP.CauseRipple(groundCheck, 4f, 0.5f);
            }
            else if (Input.GetButtonUp(controls.magBoots)/* && magBootsOn*/ && !canClimbLedge)
            {
                bootsSwitchedOn = false;
                magBootsOn = false;
                gravityModifier = gravStart;
                print("MagBoots Activated: " + magBootsOn);
            }
        }
        else
        {
            //TODO: Play some sort of disabled sound?
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


    // << -------------------------------------------------- LEDGE CLIMB METHODS ------------------------------------------- >> //


    private void CheckLedgeClimb()
    {
        if (ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;
            pAudio.PlayClimbSound();
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
        if (!magBootsOn && !isDisabled)
        {
            if (Input.GetButtonDown(controls.jump))
            {
                isPressingJumpButton = true;

                if (currentGraceTime > 0 && canJump || isWallSliding)
                {
                    pAudio.PlayJumpSound();
                    pFeedback.JumpingParticleEffect();
                    StartCoroutine(JumpDelayTime());
                    currentGraceTime = 0;
                    canJump = false;
                }
            }
            else if (Input.GetButtonUp(controls.jump)) // << -- for determining jump height 
            {
                isPressingJumpButton = false;
                //canJump = true; <<--- moved to TrackAirTime

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
                //print("flip jump");

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

        animator.ResetTrigger("jumpingFlip"); // reseting this trigger to prevent it from being stuck in the "true" position
        animator.SetTrigger("jumping");
    }


    // <<----------------------------------------------------- SLIDING SURFACE JUMP ------------------------------------------- //

    private void PushOffSlidingSurface() 
    {
        StopSlopeSlide();

        isGroundSliding = false;
        canMove = true;// TODO: block this off for sliding surface
        velocity.y = slidingSurfaceJumpForceY;
    }


    // <<----------------------------------------------------- WALL JUMP ------------------------------------------- //


    IEnumerator WallJumpTimer()
    {
        velocity.y = 6.5f * maxSpeed;
        isWallJumping = true;
        canMove = false;
        inAir = true;

        yield return new WaitForSeconds(airDisableTimer);

        isWallJumping = false;
        canMove = true;

        if (direction == Vector2.right)
        {
            print("ding");
            move.x = 3.75f;
        }
        else
        {
            print("dong");
            move.x = -3.75f;
        }
    }

    private void PushOffWall()
    {
        Debug.Log("WALL JUMP");

        isTouchingWall = false;
        isWallSliding = false;

        if (direction == Vector2.right)
        {
            move.x = 0;
            move.x += -wallJumpForce * maxSpeed;
        }
        else
        {
            move.x = 0;
            move.x += wallJumpForce * maxSpeed;
        }

        targetVelocity.x = move.x;
    }


    // <<----------------------------------------------------- ADDITIONAL JUMP METHODS ------------------------------------------- //


    IEnumerator QuickJumpTimer()
    {
        quickJump = false;

        yield return new WaitForSeconds(quickJumpDelay);

        if (isGrounded)
        {
            Jump();
            //print("Quick jump!");
            //velocity.y = jumpTakeoffSpeed;
            //animator.SetTrigger("jumping");
            //currentGraceTime = 0;
            //StopTrackAirTime();
            //inAir = true;
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
        if (!isOnGround && canJump && Input.GetButtonDown(controls.jump) && quickJump)
        {
            StartCoroutine(QuickJumpTimer());
        }
    }

    void TrackAirTime() 
    {
        if (inAir)
        {
            if (velocity.y < 0)
                airTime += Time.deltaTime;

            if (isGrounded)
            {
                canWallSlide = true;

                if (airTime >= heavyLandTime)// Heavy hard landing 
                {
                    print("HEAVY LANDING");
                    pFeedback.LandingParticles("LandingParticleSystem-Heavy");
                    ripPP.CauseRipple(groundCheck, 30f, 0.9f);
                    pFeedback.HeavyLandShake();
                    pAudio.PlayLandSound(false, true);

                    animator.SetTrigger("land");
                    StartCoroutine(LandingPause(GetAnimTime() + 0.5f));
                    pHealth.TakeFallDamage();
                }
                else if (airTime >= hardLandTime && airTime < heavyLandTime) // Light hard landing 
                {
                    print("HARD LANDING");
                    pFeedback.LandingParticles("LandingParticleSystem-Heavy");
                    ripPP.CauseRipple(groundCheck, 12f, 0.8f);
                    pFeedback.HardLandShake();
                    pAudio.PlayLandSound(true, false);

                    animator.SetTrigger("land");
                    StartCoroutine(LandingPause(GetAnimTime()));
                }
                else // normal landing
                {
                    pFeedback.LandingParticles("LandingParticleSystem-Light");
                    pAudio.PlayLandSound(false, false);
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
                canJump = true;
            }
        }
    }

    void StopTrackAirTime()
    {
        airTime = 0;
        inAir = false;
    }


    // <<----------------------------------------------------- UTILITY ------------------------------------------- //

    public void DisablePlayerController()
    {
        //windAffectMovement = false;
        SetPlayerVelocityToZero();
        isDisabled = true;
        isMoving = false;
        isMovingInWind = false;
        canJump = false;
        canMove = false;
        StopWallSliding();
        canFlipSprite = false;
        pAudio.StopFootSteps();
    }

    public void EnablePlayerController()
    {
        //print("Player controller is enabled");
        //windAffectMovement = true;
        isDisabled = false;
        canFlipSprite = true;
        canJump = true;
        canMove = true;
        canWallSlide = true;
        //targetVelocity = Vector2.zero;
    }



    public void EnableMovement(bool _enable) // Called by the Animator at the moment
    {
        canMove = _enable;
        targetVelocity = Vector2.zero;
    }
    private IEnumerator LandingPause(float _pauseTime)
    {
        EnableMovement(false);
        isMoving = false;
        move.x = 0;
        animator.SetBool("isMoving", isMoving);

        yield return new WaitForSeconds(landDelay);
        
        EnableMovement(true);
    }
}
