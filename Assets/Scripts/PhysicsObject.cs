using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float minWallNormalX = 0.8f;
    public float maxWallNormalX = 0.5f;
    public float minGroundNormalY = 0.65f; // 
    public float gravityModifier = 1f;

    [SerializeField] public Vector2 velocity;
    protected Vector2 targetVelocity;
    protected Vector2 direction;

    public bool isOnWall = false;
    [SerializeField] public bool isGrounded;

    protected Vector2 groundNormal;
    public Rigidbody2D rb2d;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f; // Minimum distance the player must be moving in order to trigger movement 
    protected const float shellRadius = 0.05f;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        contactFilter.useTriggers = false; // not checking collisions against triggers
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(8)); // only grabs collisions on the layer that the player is on (see project settings > physics2d)
        contactFilter.useLayerMask = true;
    }

    protected virtual void Update()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {
        // Handled inside the controller script
    }

    protected virtual void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        isGrounded = false;
        isOnWall = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        // First calculate X movement
        Vector2 move = moveAlongGround * deltaPosition.x;
        Movement(move, false);

        //Second calculate Y movement
        move = Vector2.up * deltaPosition.y;
        Movement(move, true);
    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance)
        {
            // Send out a raycast in the form of the attached collider (in this case a box, with an additional shell radius buffer) 
            // at the projected new position
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);  
            Debug.DrawRay(transform.position, move, Color.red);

            // Clear old values in hitbuffer list 
            hitBufferList.Clear(); 

            // Take only the items in hitBuffer that have colliders and add them to hitBufferList 
            for (int i = 0; i < count; i++) 
            {
                hitBufferList.Add(hitBuffer[i]);
            }

            // Compare the normal of each item in hitBufferList and compare them to the players current normal to determine the angle that the 
            // box collider is colliding with.
            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal; // returns the normal of each item in the array 

                // THIS IS WHERE THE PLAYER IS DETERMINED TO BE ON THE GROUND OR NOT
                if (currentNormal.y > minGroundNormalY) // checks if the normal of the item is greater than the minimum required angle for it to be considered ground
                {
                    isGrounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                if (currentNormal.x < minWallNormalX || currentNormal.x > maxWallNormalX)
                {
                    print(currentNormal.x);                
                    rb2d.velocity = Vector2.zero;
                    isOnWall = true;
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

