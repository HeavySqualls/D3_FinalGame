using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float damage;

    PlayerController pCon;

    void Start() 
    {
        pCon = GetComponent<PlayerController>();
    }

    void Update()
    {
        Attacks();
    }

    private void Attacks()
    {
        // Punch
        if (Input.GetButtonUp("Punch"))
        {
            pCon.animator.SetTrigger("punch");
        }
    }

    public void CastForEnemies() // Called from the animator
    {
        LayerMask enemyMask = LayerMask.GetMask("Enemies");
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 1f, pCon.accessibleDirection, 1f, enemyMask);
        foreach (RaycastHit2D hit in hits)
        {
            var hitObj = hit.collider.gameObject;

            if (hitObj.tag == "Enemy")
            {
                print("hit: " + hitObj.name);

                // Apply knockback to enemy 
                if (hitObj.GetComponent<Knockback>() != null)
                {
                    hitObj.GetComponent<Knockback>().IsKnocked(pCon.accessibleDirection, damage);
                }
                else
                {
                    Debug.Log("No Knockback component on enemy!");
                }           
            }
        }
    }
}
