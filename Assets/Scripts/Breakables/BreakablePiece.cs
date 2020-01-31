using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePiece : MonoBehaviour
{
    private bool isShake = false;
    public bool isEarlyBreakPiece = false;

    public Vector3 startingPos;
    public Quaternion startingTrans;

    public BoxCollider2D boxColl;
    public MeshRenderer meshRenderer;
    public Rigidbody2D rb2D;
    ShakeManager shakeMan;

    void Start()
    {      
        boxColl = GetComponent<BoxCollider2D>();
        meshRenderer = GetComponent<MeshRenderer>();
        rb2D = GetComponent<Rigidbody2D>();

        startingPos = transform.position;
        startingTrans = transform.rotation;
        shakeMan = Toolbox.GetInstance().GetShakeManager();
    }

    // Shake object for attackable object pieces
    public void ShakeGameObject(GameObject _objectToShake, float _shakeDuration, float _decreasePoint, float _shakeSpeed, float _rotAngle, bool _objectIs2D = false)
    {
        if (isShake)
        {
            print("ShakyShaky");
            StopCoroutine(shakeMan.shakeGameObjectCOR(_objectToShake, _shakeDuration, _decreasePoint, _shakeSpeed, _rotAngle, _objectIs2D));
            isShake = false;
        }

        isShake = true;

        StartCoroutine(shakeMan.shakeGameObjectCOR(_objectToShake, _shakeDuration, _decreasePoint, _shakeSpeed, _rotAngle, _objectIs2D));
    }


    // Logic for destroying the basic object piece after being attacked
    public void BlowOutPiece(Vector2 _dir, bool _isPlatform)
    {
        rb2D.bodyType = RigidbodyType2D.Dynamic;

        if (!_isPlatform)
        {
            rb2D.AddForce(_dir * Random.Range(1f, 6f), ForceMode2D.Impulse);
        }
        else
        {
            rb2D.AddForce(Vector2.down * Random.Range(0.2f, 3f), ForceMode2D.Impulse);
        }
    }


    // Shake object for crumbling platform pieces
    public void ShakePlatform(GameObject _objectToShake, float _shakeDuration, float _decreasePoint, float _shakeSpeed, float _rotAngle, bool _objectIs2D = false)
    {
        StartCoroutine(shakeMan.shakeGameObjectCOR(_objectToShake, _shakeDuration, _decreasePoint, _shakeSpeed, _rotAngle, _objectIs2D));
    }
}
