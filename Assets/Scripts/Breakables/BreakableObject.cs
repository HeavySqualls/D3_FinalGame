using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Space]
    [Header("TYPE:")]
    [Tooltip("Is this object a platform?")]
    public bool isPlatform = false;
    [Tooltip("Will this object respawn?")]
    public bool isRespawnable = false;
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

    [Space]
    [Header("OBJECT PIECES:")]
    public List<BreakablePiece> objPieces = new List<BreakablePiece>();
    public List<BreakablePiece> earlyBreakPieces = new List<BreakablePiece>();
    BoxCollider2D boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        FindEveryChild(gameObject.transform);
        currentHP = startHP;
    }

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

    public void TakeDamage(Vector2 _hitDir, float _dmg, float _knockback, float _knockUp)
    {
        currentHP -= _dmg;

        Debug.Log("Wall is shaking!");

        if (currentHP <= 0f)
        {
            Debug.Log("Wall is broken");
            boxCollider.enabled = false;
            foreach (BreakablePiece bp in objPieces)
            {
                bp.DestroyObject(_hitDir, _dmg, _knockback, _knockUp, isPlatform);
            }

            Destroy(gameObject, 3f);
        }
        else
        {
            foreach (BreakablePiece bp in objPieces)
            {
                bp.ShakeGameObject(bp.gameObject, shakeDuration, decreasePoint, shakeSpeed, rotationAngle, false);
            }
        }
    }

    public void TriggerPlatformCollapse()
    {
        Debug.Log("Platform is collapsing!");

        isFallingApart = true;

        StartCoroutine(RespawnCounter(objPieces));

        foreach (BreakablePiece bp in objPieces)
        {
            bp.ShakeAndDrop(bp.gameObject, platformShakeDuration, decreasePoint, shakeSpeed, rotationAngle, false);
        }

        if (earlyBreakPieces.Count > 0)
        {
            foreach (BreakablePiece ebp in earlyBreakPieces)
            {
                StartCoroutine(EarlyBreakDrop(ebp));
                //ebp.rb2D.bodyType = RigidbodyType2D.Dynamic;
                //ebp.rb2D.AddForce(Vector2.down * Random.Range(250f, 450f));
            }
        }    
    }

    IEnumerator EarlyBreakDrop(BreakablePiece _ebp)
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.8f));
        _ebp.rb2D.bodyType = RigidbodyType2D.Dynamic;
        _ebp.rb2D.AddForce(Vector2.down * Random.Range(250f, 450f));
    }

    IEnumerator RespawnCounter(List<BreakablePiece> _list)
    {
        yield return new WaitForSeconds(platformShakeDuration);

        boxCollider.enabled = false;

        foreach (BreakablePiece bp in _list)
        {
            bp.rb2D.bodyType = RigidbodyType2D.Dynamic;
            bp.rb2D.AddForce(Vector2.down * Random.Range(250f, 450f));
        }

        yield return new WaitForSeconds(Random.Range(1, 5));

        foreach (BreakablePiece bp in _list)
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

        yield return new WaitForSeconds(respawnTime);

        isFallingApart = false;
        boxCollider.enabled = true;

        foreach (BreakablePiece bp in _list)
        {
            bp.rb2D.velocity = Vector2.zero;
            bp.rb2D.bodyType = RigidbodyType2D.Kinematic;
            bp.gameObject.transform.position = bp.startingPos;
            bp.meshRenderer.enabled = true;
        }

        if (earlyBreakPieces.Count > 0)
        {
            foreach (BreakablePiece ebp in earlyBreakPieces)
            {
                ebp.rb2D.velocity = Vector2.zero;
                ebp.rb2D.bodyType = RigidbodyType2D.Kinematic;
                ebp.gameObject.transform.position = ebp.startingPos;
                ebp.meshRenderer.enabled = true;
            }
        }
    }
}
