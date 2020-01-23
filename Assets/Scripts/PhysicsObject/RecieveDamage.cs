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
        if (go.GetComponent<EnemyController>())
        {
            hitObj = go.GetComponent<EnemyController>();
        }
        else if (go.GetComponent<BreakableObject>())
        {
            hitObj = go.GetComponent<BreakableObject>();
        }
    }

    public void GetHit(Vector2 _hitDirection, float _dmg, float _knockback, float _knockUp)
    {    
        print("Object Hit!");

        if (_hitDirection == Vector2.right) // knock to the left
        {
            hitObj.TakeDamage(_hitDirection, _dmg, _knockback, _knockUp);
        }
        else if (_hitDirection == Vector2.left) // knock to the right 
        {
            hitObj.TakeDamage(_hitDirection, _dmg, -_knockback, _knockUp);
        }
    }
}
