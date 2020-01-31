using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Space]
    [Header("TYPE:")]
    [Tooltip("Is this object a platform?")]
    public bool isPlatform = false;
    [Tooltip("Is this object a crumbling wall?")]
    public bool isCrumblingWall = false;
    [Tooltip("Is this object a breakable floor?")]
    public bool isBreakableFloor = false;
    [Tooltip("Will this object respawn?")]
    public bool isRespawnable = false;
    [HideInInspector]
    public bool isFallingApart = false;

    [Space]
    [Header("VARIABLES:")]
    public float startHP;
    private float currentHP;
    [Tooltip("The length of time the object will shake.")]
    public float shakeDuration;
    [Tooltip("The length of time the platform will shake.")]
    public float earlyShakeDuration;
    [Tooltip("The point during the shake at which the shake strength will begin to decrease back to 0.")]
    public float decreasePoint;
    [Tooltip("The speed at which the objects will shake.")]
    public float shakeSpeed;
    [Tooltip("The max angle at which the objects will rotate when shaken.")]
    public float rotationAngle;
    [Tooltip("The time it takes for the platform to respawn.")]
    public float respawnTime;
    private bool hitByRollingObject = false;
    private Vector2 boulderHitFromDirection;

    [Space]
    [Header("OBJECT PIECES:")]
    public List<BreakablePiece> objPieces = new List<BreakablePiece>();
    public List<BreakablePiece> earlyBreakPieces = new List<BreakablePiece>();
    protected BoxCollider2D boxCollider;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        currentHP = startHP;

        FindEveryChild(gameObject.transform);
    }

    // Finds every child in the game object with a BreakablePiece component and adds it to the list 
    private void FindEveryChild (Transform parent)
    {
        int count = parent.childCount;

        for (int i = 0; i < count; i++)
        {
            if (!parent.GetChild(i).transform.GetComponent<BreakablePiece>().isEarlyBreakPiece)
            {
                objPieces.Add(parent.GetChild(i).transform.GetComponent<BreakablePiece>());
            }          
        }
    }

    // Determines if this object has been hit by a rolling object & is somthing other than a breakable floor, if so get direction of the rolling object and destroy this object
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.GetComponent<RollingObject>() && !isBreakableFloor && !isCrumblingWall)
        {
            hitByRollingObject = true;
            boulderHitFromDirection = other.collider.gameObject.GetComponent<RollingObject>().direction;
            StartCoroutine(CollapseAndRespawnCounter());
        }
    }

    // Implements damage to the object with variables passed on by the RecieveDamage component 
    public void TakeDamage(Vector2 _hitDir, float _dmg, float _knockback, float _knockUp)
    {
        currentHP -= _dmg;

        if (currentHP <= 0f) // if object has no more hit points, destroy
        {
            Debug.Log("Wall is broken");
            boxCollider.enabled = false;
            foreach (BreakablePiece bp in objPieces)
            {
                bp.BlowOutPiece(_hitDir, isPlatform);
            }

            Destroy(gameObject, 3f);
        }
        else // if object still has hit points, shake
        {
            foreach (BreakablePiece bp in objPieces)
            {
                bp.ShakeGameObject(bp.gameObject, shakeDuration, decreasePoint, shakeSpeed, rotationAngle, false);
            }
        }
    }


    // Shakes the floor when the player lands on it
    public void TriggerObjectShake()
    {
        Debug.Log("Floor is shaking!");

        ShakeObject();
    }


    // Shake platform and then trigger platform collapse & respawn coroutine
    public void TriggerPlatformCollapse()
    {
        isFallingApart = true;
        ShakeObject();

        StartCoroutine(CollapseAndRespawnCounter());
    }


    // Drop the pre-selected early drop pieces when the player lands on the platform
    IEnumerator EarlyBreakDrop(BreakablePiece _ebp)
    {
        yield return new WaitForSeconds(Random.Range(0.04f, 0.8f));
        _ebp.boxColl.enabled = true;
        _ebp.rb2D.bodyType = RigidbodyType2D.Dynamic;
        _ebp.rb2D.AddForce(Vector2.down * Random.Range(250f, 450f));

        yield break;
    }


    protected IEnumerator CollapseAndRespawnCounter()
    {
        // If the object has not been hit by a rolling object, wait for shake to finish
        if (!hitByRollingObject)
        {
            yield return new WaitForSeconds(shakeDuration);
        }

        // Disable box collider on parent object
        boxCollider.enabled = false;

        // If the object has been hit by a rolling object
        if (hitByRollingObject && !isPlatform)
        {
            BlowOutObject();
        }
        else // let gravity take its course...
        {
            DropObject();
        }

        yield return new WaitForSeconds(Random.Range(1, 5)); // wait for a few seconds - then hide breakable pieces

        HideObject();

        // if the object IS respawnable, wait the chosen amount of time, then reset in the original starting position
        if (isRespawnable)
        {
            yield return new WaitForSeconds(respawnTime);

            isFallingApart = false;
            boxCollider.enabled = true;

            RespawnObject();
        }
        else // If the object is NOT respawnable, destroy
        {
            Destroy(gameObject);
        }

        hitByRollingObject = false;

        yield break;
    }

    private void ShakeObject()
    {
        foreach (BreakablePiece bp in objPieces)
        {
            bp.ShakePlatform(bp.gameObject, shakeDuration, decreasePoint, shakeSpeed, rotationAngle, false);
        }

        if (earlyBreakPieces.Count > 0)
        {
            foreach (BreakablePiece ebp in earlyBreakPieces)
            {
                StartCoroutine(EarlyBreakDrop(ebp));
            }
        }
    }

    private void DropObject()
    {
        foreach (BreakablePiece bp in objPieces)
        {
            bp.rb2D.bodyType = RigidbodyType2D.Dynamic;
            bp.rb2D.gravityScale = 2f;
            bp.boxColl.enabled = true;
        }

        if (earlyBreakPieces.Count > 0)
        {
            foreach (BreakablePiece ebp in earlyBreakPieces)
            {
                ebp.rb2D.bodyType = RigidbodyType2D.Dynamic;
                ebp.rb2D.gravityScale = 2f;
                ebp.boxColl.enabled = true;
            }
        }
    }

    private void BlowOutObject()
    {
        foreach (BreakablePiece bp in objPieces)
        {
            bp.BlowOutPiece(boulderHitFromDirection, isPlatform);
        }

        if (earlyBreakPieces.Count > 0)
        {
            foreach (BreakablePiece ebp in earlyBreakPieces)
            {
                ebp.BlowOutPiece(boulderHitFromDirection, isPlatform);
            }
        }
    }

    private void HideObject()
    {
        foreach (BreakablePiece bp in objPieces)
        {
            bp.meshRenderer.enabled = false;
            bp.boxColl.enabled = false;
        }

        if (earlyBreakPieces.Count > 0)
        {
            foreach (BreakablePiece ebp in earlyBreakPieces)
            {
                ebp.meshRenderer.enabled = false;
                ebp.boxColl.enabled = false;
            }
        }
    }

    private void RespawnObject()
    {
        foreach (BreakablePiece bp in objPieces)
        {
            bp.rb2D.velocity = Vector2.zero;
            bp.rb2D.angularVelocity = 0f;
            bp.rb2D.bodyType = RigidbodyType2D.Kinematic;
            if (isCrumblingWall)
            {
                bp.boxColl.enabled = true;
            }
            bp.gameObject.transform.position = bp.startingPos;
            bp.gameObject.transform.rotation = bp.startingTrans;
            bp.meshRenderer.enabled = true;
        }

        if (earlyBreakPieces.Count > 0)
        {
            foreach (BreakablePiece ebp in earlyBreakPieces)
            {
                ebp.rb2D.velocity = Vector2.zero;
                ebp.rb2D.angularVelocity = 0f;
                ebp.rb2D.bodyType = RigidbodyType2D.Kinematic;
                if (isCrumblingWall)
                {
                    ebp.boxColl.enabled = true;
                }
                ebp.gameObject.transform.position = ebp.startingPos;
                ebp.gameObject.transform.rotation = ebp.startingTrans;
                ebp.meshRenderer.enabled = true;
            }
        }
    }
}
