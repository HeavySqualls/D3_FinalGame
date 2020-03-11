using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BreakablePiece : MonoBehaviour
{
    [Tooltip("Will this piece fall appart before the rest of the object?")]
    public bool isEarlyBreakPiece = false;
    bool isShake = false;

    Vector3 startingPos;
    Quaternion startingTrans;
    BoxCollider2D boxColl;
    MeshRenderer meshRenderer;
    Rigidbody2D rb;

    Tween shakeTween;

    void Start()
    {      
        boxColl = GetComponent<BoxCollider2D>();
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody2D>();
        startingPos = transform.position;
        startingTrans = transform.rotation;
    }


    // Drop this individual piece 
    public void DropPiece()
    {
        StartCoroutine(Drop());
    }

    IEnumerator Drop()
    {
        yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
        print("piece dropping");
        rb.isKinematic = false;// RigidbodyType.Dynamic;
        //rb.useGravity = 2f;
        boxColl.enabled = true;

        yield break;
    }


    // Logic for destroying the basic object piece after being attacked
    public void BlowOutPiece(Vector2 _dir, bool _isPlatform)
    {
        rb.isKinematic = false;// bodyType = RigidbodyType.Dynamic;

        if (!_isPlatform)
        {
            rb.AddForce(_dir * Random.Range(1f, 6f), ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(Vector2.down * Random.Range(0.2f, 3f), ForceMode2D.Impulse);
        }
    }

    // Shake object for crumbling platform pieces
    public void ShakePiece(GameObject _objectToShake, float _shakeDuration, float _strength, int _vibrato)
    {
        if (shakeTween != null)
        {
            shakeTween.Kill();
        }
        _objectToShake.transform.position = startingPos;
        shakeTween = _objectToShake.transform.DOShakePosition(_shakeDuration, _strength, _vibrato, 2f, false, false);
    }

    // Hide piece from sight & collisions
    public void HidePiece()
    {
        meshRenderer.enabled = false;
        boxColl.enabled = false;
    }

    // Respawn the piece in its original place
    public void RespawnPiece(bool _isCrumblingWall)
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;/*Vector3.zero;*/
        /*rb.isKinematic = true;*/rb.bodyType = RigidbodyType2D.Kinematic;

        if (_isCrumblingWall)
        {
            boxColl.enabled = true;
        }

        gameObject.transform.position = startingPos;
        gameObject.transform.rotation = startingTrans;
        meshRenderer.enabled = true;
    }
}
