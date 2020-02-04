using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecieveDamage : MonoBehaviour
{
    [Tooltip("Add in the game object that this script is attached to.")]
    public GameObject go;
    private dynamic hitObj;

    void Start()
    {
        if (go != null)
        {
            if (go.GetComponent<EnemyController>()) //<<------------ if this object is an enemy
            {
                hitObj = go.GetComponent<EnemyController>();
            }
            else if (go.GetComponent<BreakableObject>()) //<<------- if this object is a breakable object
            {
                hitObj = go.GetComponent<BreakableObject>();
            }
            else if (go.GetComponent<RollingObject>()) //<<--------- if this object is a rolling object
            {
                hitObj = go.GetComponent<RollingObject>();
            }
            else if (go.GetComponent<PlayerController>()) //<<------ if this object is the player
            {
                hitObj = go.GetComponent<PlayerHandleInteract>();
            }
        }
        else
        {
            Debug.LogError("No game object assigned to slot!");
        }

    }

    public void GetHit(Vector2 _hitDirection, float _dmg, float _knockback, float _knockUp, float _stunTime)
    {    
        print(gameObject.name + " hit!");

        if (_hitDirection == Vector2.right) // knock to the left
        {
            hitObj.TakeDamage(_hitDirection, _dmg, _knockback, _knockUp, _stunTime);
        }
        else if (_hitDirection == Vector2.left) // knock to the right 
        {
            hitObj.TakeDamage(_hitDirection, _dmg, -_knockback, _knockUp, _stunTime);
        }
        else if (_hitDirection == Vector2.zero) // below or above
        {
            hitObj.TakeDamage(_hitDirection, _dmg, -_knockback, _knockUp, _stunTime);
        }
    }
}
