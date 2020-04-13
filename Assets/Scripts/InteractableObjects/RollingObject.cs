using UnityEngine;

public class RollingObject : MonoBehaviour
{
    public Vector2 direction;
    private Vector3 startPos;

    Rigidbody2D rb2D;

    private void OnEnable()
    {
        SpawnManager.onResetLevelObjects += ResetRollingObject;
    }

    private void OnDisable()
    {
        SpawnManager.onResetLevelObjects -= ResetRollingObject;
    }

    void Start()
    {
        startPos = gameObject.transform.position;
        rb2D = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(Vector2 _hitDir, float _damage, float _knockBack, float _knockUp, float _stun)
    {
        print("hit");
        rb2D.AddForce(_hitDir * (_damage * 100));
        direction = _hitDir;
    }

    public void ResetRollingObject()
    {
        gameObject.transform.position = startPos;
    }
}
