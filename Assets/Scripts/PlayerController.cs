using UnityEngine;

public class PlayerController : PhysicsObject
{
    [Space]
    [Header("STATUS:")]
    public bool pIsFlipped;
    public bool isInteractable;
    public bool canMove = true;
    public bool magBootsOn = false;


    [Space]
    [Header("MOVEMENT/JUMP:")]
    public float maxSpeed = 2f;
    public float jumpTakeoffSpeed = 6f;
    [SerializeField] private float maxGraceTime = 0.12f;
    [SerializeField] private float currentGraceTime;


    [Space]
    [Header("LEDGE CHECK:")]
    private bool isTouchingLedge = false;
    public float ledgeCheckDistance = 1;
    public Transform ledgeCheck; // for checking if on the edge of a collider

    public bool canClimbLedge = false;
    private bool ledgeDetected;

    private Vector2 ledgePosBot;

    private Vector2 ledgePos1;
    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f;

    private Vector2 ledgePos2;
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;


    [Space]
    [Header("WALL CHECK:")]
    private bool isTouchingWall = false;
    public float wallCheckDistance = 1;
    public Transform wallCheck; // for checking if against wall


    [Space]
    [Header("GROUND CHECK:")]
    [SerializeField] private bool isTouchingGround = false;
    int whatIsGround;
    public float groundCheckDistance = 1;
    public Transform groundCheck; // for determining quick landing jump


    [Space]
    [Header("REFERENCES:")]
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        direction = Vector2.right;
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
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        CheckSurroundings();
    }

    void JumpTimer()
    {
        if (!isGrounded)
        {
            currentGraceTime -= Time.deltaTime;
        }
        else
        {
            currentGraceTime = maxGraceTime;
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

            if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
            {
                ledgeDetected = true;
                ledgePosBot = wallCheck.position;
            }
        }
    }

    private void CheckLedgeClimb()
    {
        if (ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;

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

    protected override void ComputeVelocity()
    {
        if (canMove)
        {
            Vector2 move = Vector2.zero;
            move.x = Input.GetAxis("Horizontal");

            if (!magBootsOn)
            {
                if (Input.GetButtonDown("Jump") && isGrounded || Input.GetButtonDown("Jump") && currentGraceTime > 0 || isTouchingGround && !isGrounded && Input.GetKey(KeyCode.Space))
                {
                    velocity.y = jumpTakeoffSpeed;
                    animator.SetTrigger("jumping");
                    currentGraceTime = 0;
                }
                else if (Input.GetButtonUp("Jump"))
                {
                    if (velocity.y > 0)
                    {
                        velocity.y = velocity.y * 0.5f;
                    }
                }
            }

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

    public void RapidJump()
    {
        if (isTouchingGround && !isGrounded && Input.GetKey(KeyCode.Space))
        {
            velocity.y = jumpTakeoffSpeed;
            animator.SetTrigger("jumping");
            currentGraceTime = 0;
        }
    }

    private void ChangeDirection()
    {
        if (direction == Vector2.right)
            direction = Vector2.left;
        else
            direction = Vector2.right;
    }

    public void MagBoots()
    {
        if (Input.GetKeyUp(KeyCode.E) && !magBootsOn)
        {
            print("MagBoots Activated: " + magBootsOn);
            magBootsOn = true;
            rb2d.velocity = Vector3.zero;
        }
        else if (Input.GetKeyUp(KeyCode.E) && magBootsOn)
        {
            print("MagBoots Activated: " + magBootsOn);
            magBootsOn = false;
        }
    }

    public void Destruction()
    {
        Destroy(gameObject);
    }
}
