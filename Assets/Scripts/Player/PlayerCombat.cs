using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float damage;

    public float knockback;
    public float knockUp;

    private bool canAttack = true;

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
        if (canAttack)
        {
            // Punch
            if (Input.GetButtonUp(pCon.controls.punch))
            {
                SetAttackStats(2, 2, 8);
                pCon.animator.SetTrigger("punch");
                StartCoroutine(AttackCoolDown(pCon.GetAnimTime()));
            }

            // Boot Launch
            if (Input.GetButtonUp(pCon.controls.launch))
            {
                SetAttackStats(0, 0, 35);
                pCon.animator.SetTrigger("kick");
                StartCoroutine(AttackCoolDown(pCon.GetAnimTime()));
            }
        }
    }

    IEnumerator AttackCoolDown(float _time)
    {
        canAttack = false;
        pCon.CanFlipSprite();

        yield return new WaitForSeconds(_time);

        pCon.CanFlipSprite();
        canAttack = true;

    }

    void SetAttackStats(float _dmg, float _knkBk, float _knkUp)
    {
        damage = _dmg;
        knockback = _knkBk;
        knockUp = _knkUp;
    }

    public void CanAttack() // Called from the animator
    {
        canAttack = true;
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
                    hitObj.GetComponent<Knockback>().IsKnocked(pCon.accessibleDirection, damage, knockback, knockUp);
                }
                else
                {
                    Debug.Log("No Knockback component on enemy!");
                }           
            }
        }
    }
}
