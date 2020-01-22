using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float knockbackLength;
    [SerializeField] float knockbackCount;

    public EnemyController eCon;

    void Start()
    {
        knockbackCount = knockbackLength;
    }

    void Update()
    {
        if (eCon.isHit)
        {
            knockbackCount -= Time.deltaTime;

            if (knockbackCount <= 0 && eCon.isGrounded)
            {
                knockbackCount = knockbackLength;
                eCon.currentState = EnemyController.State.Patrolling;
            }
        }
    }

    public void IsKnocked(Vector2 _hitDirection, float _dmg, float _knockback, float _knockUp)
    {    
        print("Enemy Hit!");

        if (_hitDirection == Vector2.right) // knock to the left
        {
            eCon.TakeDamage(_hitDirection, _dmg, _knockback, _knockUp);
        }
        else if (_hitDirection == Vector2.left) // knock to the right 
        {
            eCon.TakeDamage(_hitDirection, _dmg, -_knockback, _knockUp);
        }
    }
}
