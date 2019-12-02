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
        //eCon.GetComponent<EnemyController>();
    }

    void Update()
    {
        if (eCon.isHit)
        {
            knockbackCount -= Time.deltaTime;

            if (knockbackCount <= 0)
            {
                eCon.isHit = false;
            }
        }
    }

    public void IsKnocked(Vector2 _hitDirection)
    {
        print("Knocked!");
        eCon.isHit = true;
        eCon.TakeDamage(2);

        knockbackCount = knockbackLength;

        if (_hitDirection == Vector2.right) // knock to the left
        {
            eCon.UpdateTargVelocity(knockback, knockUp);
        }
        else if (_hitDirection == Vector2.left) // knock to the right 
        {
            eCon.UpdateTargVelocity(-knockback, knockUp);
        }
    }
}
