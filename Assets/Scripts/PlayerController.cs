using UnityEngine;

public class PlayerController : Character_Base
{
    [Space]
    [Header("Player Status:")]
    public bool pIsFlipped;
    public bool isInteractable;
    public bool isDead = false;
    public bool magBootsOn = false;
    //public bool isFrozen = true;

    [Space]
    [Header("Player Stats:")]
    public float jumpTakeoffSpeed = 6f;
    public float maxSpeed = 2f;
    //public float damageOutput;
    //public int health = 100;

    //[Space]
    //[Header("Player Score:")]
    //public int timesPlayerDied = 0;
    //public int enemiesKilled = 0;
    //public float totalTime = 0;

    [Space]
    [Header("Player Refrences:")]
    //public Transform spawnZone;
    //public GameObject interactableItem;
    //private GameManager gm;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    //private PlayerUI pUI;

    void Awake()
    {
        //gm = Toolbox.GetInstance().GetGameManager();
        //pUI = GetComponentInChildren<PlayerUI>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        //pUI.SetAmmo(ammo);
    }

    protected override void Update()
    {
        base.Update();
        ComputeVelocity();
        MagBoots();
        //Interaction();

        //if (isFrozen)
        //    isGrounded = true;
    }

    public void Destruction()
    {
        Destroy(gameObject);
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
        if (!isDead /*&& !isFrozen*/)
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

    //public void DeliverScore()
    //{
    //    gm.RecieveScore(timesPlayerDied, bulletsCollected, enemiesKilled);
    //}

    //public void Respawn()
    //{
    //    gm.RestartLevel(false);
    //}

    //public void TakeDamage(int _damage)
    //{
    //    health -= _damage;
    //    UpdateHealthUI();
    //    StartCoroutine(IFlashRed(spriteRenderer));

    //    if (health <= 0)
    //    {
    //        print("Player is DEAD");
    //        isDead = true;
    //        animator.SetBool("isDead", isDead);
    //    }
    //}

    //public void UpdateHealthUI()
    //{
    //    pUI.SetHealth(health);
    //}

    //private void Interaction()
    //{
    //    if (interactableItem != null && !isDead)
    //    {
    //        if (interactableItem.GetComponent<Interactable_DoubleJump>() && Input.GetKeyDown(KeyCode.Mouse1))
    //        {
    //            interactableItem.GetComponent<Interact_Base>().OnInteracted();
    //            print("Double jumped!");
    //        }
    //        else if (Input.GetKeyUp(KeyCode.E))
    //        {
    //            interactableItem.GetComponent<Interact_Base>().OnInteracted();
    //            print("Picked up item");
    //        }
    //    }
    //}

    //public void InteractableItem(GameObject _interactableObject)
    //{
    //    interactableItem = _interactableObject;
    //}
}
