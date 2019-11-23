using UnityEngine;

public class PlayerController : Character_Base
{
    [Space]
    [Header("Player Status:")]
    public bool pIsFlipped;
    public bool isInteractable;
    public bool isDead = false;
    public bool magBootsOn = false;

    [Space]
    [Header("Player Stats:")]
    public float jumpTakeoffSpeed = 6f;
    public float maxSpeed = 2f;

    [Space]
    [Header("Player Refrences:")]
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        ComputeVelocity();
        MagBoots();
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

    protected override void ComputeVelocity()
    {
        if (!isDead)
        {
            Vector2 move = Vector2.zero;
            move.x = Input.GetAxis("Horizontal");

            if (!magBootsOn)
            {
                if (Input.GetButtonDown("Jump") && isGrounded)
                {
                    velocity.y = jumpTakeoffSpeed;
                    animator.SetTrigger("jumping");
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
                pIsFlipped = !pIsFlipped;
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }

            animator.SetBool("grounded", isGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            targetVelocity = move * maxSpeed;
        }
    }

    public void DoubleJump()
    {
        velocity.y = jumpTakeoffSpeed;
    }

    public void Destruction()
    {
        Destroy(gameObject);
    }
}
