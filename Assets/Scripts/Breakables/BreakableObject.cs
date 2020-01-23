using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public bool isPlatform = false;
    public bool isRespawnable = false;
    public bool isFallingApart = false;

    public float startHP;
    private float currentHP;

    [Tooltip("The length of time the object will shake.")]
    public float shakeDuration;
    [Tooltip("The length of time the platform will shake.")]
    public float platformShakeDuration;
    private float currentPlatformShakeDur;
    [Tooltip("The point during the shake at which the shake strength will begin to decrease back to 0.")]
    public float decreasePoint;
    [Tooltip("The speed at which the objects will shake.")]
    public float shakeSpeed;
    [Tooltip("The max angle at which the objects will rotate when shaken.")]
    public float rotationAngle;

    public float respawnTime;

    public List<BreakablePiece> objPieces = new List<BreakablePiece>();

    BoxCollider2D boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        currentPlatformShakeDur = platformShakeDuration;

        FindEveryChild(gameObject.transform);
        currentHP = startHP;
    }

    //void Update()
    //{
    //    if (isFallingApart)
    //    {
    //        currentPlatformShakeDur -= Time.deltaTime;

    //        if (currentPlatformShakeDur <= 0)
    //        {
    //            Bo
    //        }
    //    }
    //}

    private void FindEveryChild (Transform parent)
    {
        int count = parent.childCount;

        for (int i = 0; i < count; i++)
        {
            objPieces.Add(parent.GetChild(i).transform.GetComponent<BreakablePiece>());
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

        StartCoroutine(RespawnCounter());

        foreach (BreakablePiece bp in objPieces)
        {
            bp.ShakeAndDrop(bp.gameObject, platformShakeDuration, decreasePoint, shakeSpeed, rotationAngle, false);
        }
    }

    IEnumerator RespawnCounter()
    {
        yield return new WaitForSeconds(platformShakeDuration);

        boxCollider.enabled = false;

        foreach (BreakablePiece bp in objPieces)
        {
            bp.rb2D.bodyType = RigidbodyType2D.Dynamic;
            //bp.rb2D.gravityScale = 6.35f;
            bp.rb2D.AddForce(Vector2.down * Random.Range(250f, 450f));
        }

        yield return new WaitForSeconds(Random.Range(1, 5));


        foreach (BreakablePiece bp in objPieces)
        {
            bp.meshRenderer.enabled = false;
            bp.boxColl.enabled = false;
        }

        yield return new WaitForSeconds(respawnTime);

        isFallingApart = false;
        boxCollider.enabled = true;

        foreach (BreakablePiece bp in objPieces)
        {
            bp.rb2D.velocity = Vector2.zero;
            bp.rb2D.bodyType = RigidbodyType2D.Kinematic;
            bp.gameObject.transform.position = bp.startingPos;
            bp.meshRenderer.enabled = true;
        }
    }
}
