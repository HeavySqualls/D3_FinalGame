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
    public float platformShakeDuration;
    public float earlyPlatformShakeDuration;
    [Tooltip("The point during the shake at which the shake strength will begin to decrease back to 0.")]
    public float decreasePoint;
    [Tooltip("The speed at which the objects will shake.")]
    public float shakeSpeed;
    [Tooltip("The max angle at which the objects will rotate when shaken.")]
    public float rotationAngle;
    [Tooltip("The time it takes for the platform to respawn.")]
    public float respawnTime;
    private bool hitByBoulder = false;
    private Vector2 boulderHitFromDirection;

    [Space]
    [Header("OBJECT PIECES:")]
    [SerializeField] List<BreakablePiece> objPieces = new List<BreakablePiece>();
    [SerializeField] List<BreakablePiece> earlyBreakPieces = new List<BreakablePiece>();
    BoxCollider2D boxCollider;

    void Start()
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

    // Determines if this object has been hit by a rolling object, if so get direction of the rolling object and destroy this object
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.GetComponent<RollingObject>())
        {
            hitByBoulder = true;
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
                bp.DestroyObject(_hitDir, isPlatform);
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

    // Shake platform and then trigger platform collapse & respawn coroutine
    public void TriggerPlatformCollapse()
    {
        isFallingApart = true;

        foreach (BreakablePiece bp in objPieces)
        {
            bp.ShakePlatform(bp.gameObject, platformShakeDuration, decreasePoint, shakeSpeed, rotationAngle, false);
        }

        if (earlyBreakPieces.Count > 0)
        {
            foreach (BreakablePiece ebp in earlyBreakPieces)
            {
                StartCoroutine(EarlyBreakDrop(ebp));
            }
        }
        StartCoroutine(CollapseAndRespawnCounter());
    }

    // Drop the pre-selected early drop pieces when the player lands on the platform
    IEnumerator EarlyBreakDrop(BreakablePiece _ebp)
    {
        yield return new WaitForSeconds(Random.Range(0.04f, 0.8f));
        _ebp.rb2D.bodyType = RigidbodyType2D.Dynamic;
        _ebp.rb2D.AddForce(Vector2.down * Random.Range(250f, 450f));

        yield break;
    }


    IEnumerator CollapseAndRespawnCounter()
    {
        if (!hitByBoulder)
        {
            yield return new WaitForSeconds(platformShakeDuration);
        }

        boxCollider.enabled = false;

        if (hitByBoulder) // if hit by a boulder...
        {
            if (isPlatform)
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
            else
            {
                foreach (BreakablePiece bp in objPieces)
                {
                    bp.DestroyObject(boulderHitFromDirection, isPlatform);
                }

                if (earlyBreakPieces.Count > 0)
                {
                    foreach (BreakablePiece ebp in earlyBreakPieces)
                    {
                        ebp.DestroyObject(boulderHitFromDirection, isPlatform);
                    }
                }

                yield break; // end coroutine - object is destroyed
            }

        }
        else // if NOT hit by a boulder... let gravity take its course
        {
            foreach (BreakablePiece bp in objPieces)
            {
                bp.rb2D.bodyType = RigidbodyType2D.Dynamic;
                bp.boxColl.enabled = true;
                if (isPlatform)
                {
                    bp.rb2D.AddForce(Vector2.down * Random.Range(250f, 450f));
                }
            }
        }

        // wait for a few seconds - then hide breakable pieces
        yield return new WaitForSeconds(Random.Range(1, 5));

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

        // if the object IS respawnable, wait the chosen amount of time, then reset in the original starting position
        if (isRespawnable)
        {
            yield return new WaitForSeconds(respawnTime);

            isFallingApart = false;
            boxCollider.enabled = true;

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
        else // If the object is NOT respawnable, destroy
        {
            Destroy(gameObject);
        }

        hitByBoulder = false;

        yield break;
    }
}
