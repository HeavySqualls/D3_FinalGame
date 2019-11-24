using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float minGroundNormalY = .65f;
    public float gravityModifier = 1f;

    protected Vector2 targetVelocity;

    protected Vector2 direction;

    [SerializeField] public bool isGrounded;

    protected Vector2 groundNormal;
    public Rigidbody2D rb2d;
    [SerializeField] public Vector2 velocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        contactFilter.useTriggers = false; // not checking collisions against triggers
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer)); // only grabs collisions on the layer that the player is on (see project settings > physics2d)
        contactFilter.useLayerMask = true;
    }

    protected virtual void Update()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {

    }

    protected virtual void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        isGrounded = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        // First calculate X movement
        Vector2 move = moveAlongGround * deltaPosition.x;
        Movement(move, false);

        //Second calculate Y movement
        move = Vector2.up * deltaPosition.y;
        Movement(move, true);
    }

    protected IEnumerator NotGroundedDelay()
    {
        yield return new WaitForSeconds(1f);
        isGrounded = false;
    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance)
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for (int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;

                if (currentNormal.y > minGroundNormalY) // represents the angle of the Y position relative to the ground (ie slopes)
                {
                    isGrounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                // Prevents player from loosing velocity when interacting with another slope while in the air
                float projection = Vector2.Dot(velocity, currentNormal); // Vector2.Dot returns zero if vectors are perpendicular
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance; // if ModifiedDistance is less than distance, use modified distance, otherwise use distance
            }
        }

        rb2d.position = rb2d.position + move.normalized * distance;
    }

    protected IEnumerator IFlashRed(SpriteRenderer _thisFlashRenderer)
    {
        _thisFlashRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _thisFlashRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        _thisFlashRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _thisFlashRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
    }
}

