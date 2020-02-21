using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecieveDamage : MonoBehaviour
{
    [Tooltip("Add in the game object that this script is attached to.")]
    public GameObject go;
    private PlayerHandleInteract pCon;
    private EnemyController enCon;
    private BreakableObject bO;
    private RollingObject rO;

    void Start()
    {
        if (go != null)
        {
            if (go.GetComponent<PlayerController>()) //<<------ if this object is the player
            {
                pCon = go.GetComponent<PlayerHandleInteract>();
            }
            else if (go.GetComponent<EnemyController>()) //<<------------ if this object is an enemy
            {
                enCon = go.GetComponent<EnemyController>();
            }
            else if (go.GetComponent<BreakableObject>()) //<<------- if this object is a breakable object
            {
                bO = go.GetComponent<BreakableObject>();
            }
            else if (go.GetComponent<RollingObject>()) //<<--------- if this object is a rolling object
            {
                rO = go.GetComponent<RollingObject>();
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

        if (enCon != null)
        {
            if (_hitDirection == Vector2.right) // knock to the left
            {
                enCon.TakeDamage(_hitDirection, _dmg, _knockback, _knockUp, _stunTime);
            }
            else if (_hitDirection == Vector2.left) // knock to the right 
            {
                enCon.TakeDamage(_hitDirection, _dmg, -_knockback, _knockUp, _stunTime);
            }
            else if (_hitDirection == Vector2.zero) // below or above
            {
                enCon.TakeDamage(_hitDirection, _dmg, -_knockback, _knockUp, _stunTime);
            }
        }
        else if (pCon != null)
        {
            if (_hitDirection == Vector2.right) // knock to the left
            {
                pCon.TakeDamage(_hitDirection, _dmg, _knockback, _knockUp, _stunTime);
            }
            else if (_hitDirection == Vector2.left) // knock to the right 
            {
                pCon.TakeDamage(_hitDirection, _dmg, -_knockback, _knockUp, _stunTime);
            }
            else if (_hitDirection == Vector2.zero) // below or above
            {
                pCon.TakeDamage(_hitDirection, _dmg, -_knockback, _knockUp, _stunTime);
            }
        }
        else if (bO != null)
        {
            if (_hitDirection == Vector2.right) // knock to the left
            {
                bO.TakeDamage(_hitDirection, _dmg, _knockback, _knockUp, _stunTime);
            }
            else if (_hitDirection == Vector2.left) // knock to the right 
            {
                bO.TakeDamage(_hitDirection, _dmg, -_knockback, _knockUp, _stunTime);
            }
            else if (_hitDirection == Vector2.zero) // below or above
            {
                bO.TakeDamage(_hitDirection, _dmg, -_knockback, _knockUp, _stunTime);
            }
        }
        else if (rO != null)
        {
            if (_hitDirection == Vector2.right) // knock to the left
            {
                rO.TakeDamage(_hitDirection, _dmg, _knockback, _knockUp, _stunTime);
            }
            else if (_hitDirection == Vector2.left) // knock to the right 
            {
                rO.TakeDamage(_hitDirection, _dmg, -_knockback, _knockUp, _stunTime);
            }
            else if (_hitDirection == Vector2.zero) // below or above
            {
                rO.TakeDamage(_hitDirection, _dmg, -_knockback, _knockUp, _stunTime);
            }
        }

    }
}
