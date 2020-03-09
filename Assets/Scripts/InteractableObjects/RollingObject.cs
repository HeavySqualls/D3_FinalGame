using UnityEngine;

public class RollingObject : MonoBehaviour
{
    public Vector2 direction;

    Rigidbody2D rb2D;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(Vector2 _hitDir, float _damage, float _knockBack, float _knockUp, float _stun)
    {
        print("hit");
        rb2D.AddForce(_hitDir * (_damage * 20));
        direction = _hitDir;
    }
}
