using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float knockback;

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
            print("hit: " + hit.collider.gameObject.name);

            Rigidbody2D rb = hit.collider.GetComponent<Rigidbody2D>();
            hit.collider.GetComponent<Knockback>().IsKnocked(pCon.accessibleDirection);
            // Apply knockback to enemy 
            //rb.AddRelativeForce(pCon.accessibleDirection, ForceMode2D.Impulse);
            //rb.AddForce(pCon.accessibleDirection * knockback);

            // Remove health from enemy
        }
    }
}
