using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePiece : MonoBehaviour
{
    public bool isHit = false;
    private bool isShake = false;

    Vector3 startingPos; // TODO: ---- >> for respawning ledges
    Rigidbody2D rb2D;
    ShakeManager shakeMan;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        startingPos = transform.position;
        shakeMan = Toolbox.GetInstance().GetShakeManager();
    }

    public void DestroyWall(Vector2 _dir, float _dmg, float _knockback, float _knockUp)
    {
        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.AddForce(_dir * Random.Range(100f, 600f));
        Destroy(gameObject, Random.Range(0.5f, 1.5f));
    }

    public void ShakeGameObject(GameObject _objectToShake, float _shakeDuration, float _decreasePoint, float _shakeSpeed, float _rotAngle, bool _objectIs2D = false)
    {
        if (isShake)
        {
            StopCoroutine(shakeMan.shakeGameObjectCOR(_objectToShake, _shakeDuration, _decreasePoint, _shakeSpeed, _rotAngle, _objectIs2D));
        }

        isShake = true;

        StartCoroutine(shakeMan.shakeGameObjectCOR(_objectToShake, _shakeDuration, _decreasePoint, _shakeSpeed, _rotAngle, _objectIs2D));
    }
}
