using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float knockback;
    public float knockUp;
    public float knockbackLength;
    public bool knockFromRight;
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

    public void IsKnocked(Vector2 _hitDirection, float _dmg)
    {    
        print("Enemy Hit!");

        if (_hitDirection == Vector2.right) // knock to the left
        {
            eCon.TakeDamage(_hitDirection, _dmg, knockback, knockUp);
            //eCon.
        }
        else if (_hitDirection == Vector2.left) // knock to the right 
        {
            eCon.TakeDamage(_hitDirection, _dmg, -knockback, knockUp);
        }
    }
}
