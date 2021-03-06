﻿using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    [Space]
    [Header("--- PHYSICS OBJECT ---")]
    //private bool isOnWall = false;
    public bool isGrounded; 
    public bool inWindZone = false;
    public Vector2 velocity;
    [SerializeField] protected Vector2 targetVelocity;
    public float gravStart = 6.35f;
    public float terminalVelocity = 40f;
    protected float gravityModifier;

    protected float minWallNormalX = 0.8f;
    protected float maxWallNormalX = 0.5f;
    protected float minGroundNormalY = 0.65f;

    [SerializeField] protected Vector2 direction;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.0001f; // Minimum distance the player must be moving in order to trigger movement 
    protected const float shellRadius = 0.1f;

    public Vector2 windDir;
    public float windPwr;
    protected bool isFromLeft;
    public bool windAffectMovement;
    public bool inBoras;
    public LayerMask layerMask;

    protected virtual void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        gravityModifier = gravStart;

        layerMask = ~((1 << 16) | (1 << 12) | (1 << 13) | (1 << 20));

        contactFilter.useTriggers = false; // not checking collisions against triggers
        contactFilter.SetLayerMask(layerMask); // only grabs collisions on the layer the specified layer (see project settings > physics2d)
        contactFilter.useLayerMask = true;
    }

    protected virtual void Update()
    {

    }

    public void WindZoneStats(Vector2 _windDir, float _windPwr, bool _directionOfSource, bool _windAffectMovement, bool _isBoras)
    {
        windDir = _windDir;
        windPwr = _windPwr;
        isFromLeft = _directionOfSource;
        windAffectMovement = _windAffectMovement;
        inBoras = _isBoras;
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
        //isOnWall = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        // First calculate X movement
        Vector2 move = moveAlongGround * deltaPosition.x;
        Movement(move, false);

        //Second calculate Y movement
        move = Vector2.up * deltaPosition.y;
        Movement(move, true);
    }

    void Movement(Vector2 _move, bool _yMovement)
    {
        float distance = _move.magnitude;

        if (distance > minMoveDistance)
        {
            // Send out a raycast in the form of the attached collider (in this case a box, with an additional shell radius buffer) 
            // at the projected new position
            int count = rb2d.Cast(_move, contactFilter, hitBuffer, distance + shellRadius);  

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
                    if (_yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                // THIS IS WHERE THE PLAYER IS DETERMINED TO BE ON A WALL OR NOT
                //if (currentNormal.x < minWallNormalX || currentNormal.x > maxWallNormalX && hitBufferList[i].collider.gameObject.tag == "Platform")
                //{
                //    rb2d.velocity = Vector2.zero;
                //    isOnWall = true;
                //    print("On Wall");
                //}

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

        rb2d.position = rb2d.position + _move.normalized * distance;
    }
}

