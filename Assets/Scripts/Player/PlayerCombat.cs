using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float damage;

    public float knockback;
    public float knockUp;

    [SerializeField] private bool canAttack = true;
    [SerializeField] private bool comboAttacking = false;

    public float maxComboTime = 0.5f;
    [SerializeField] private float timeBetweenCombos = 0;
    [SerializeField] private int comboNum = 1;

    PlayerController pCon;

    void Start() 
    {
        pCon = GetComponent<PlayerController>();
    }

    void Update()
    {
        Attacks();
        AttackTimer();
    }

    private void AttackTimer()
    {
        if (comboAttacking)
        {
            timeBetweenCombos += Time.deltaTime;

            if (timeBetweenCombos > maxComboTime)
            {
                timeBetweenCombos = 0;
                comboNum = 1;
                comboAttacking = false;
            }
        }
    }

    private void Attacks()
    {
        if (canAttack)
        {
            // Punch
            if (Input.GetButtonUp(pCon.controls.punch) && comboAttacking && comboNum == 2)
            {
                timeBetweenCombos = 0;
                SetAttackStats(2, 2, 8);
                pCon.animator.SetTrigger("punch2");
                StartCoroutine(AttackCoolDown(pCon.GetAnimTime() / 4));

                comboNum = 1;
            }
            else if (Input.GetButtonUp(pCon.controls.punch) && comboNum == 1)
            {
                comboAttacking = true;
                timeBetweenCombos = 0;

                SetAttackStats(2, 2, 8);
                pCon.animator.SetTrigger("punch");
                StartCoroutine(AttackCoolDown(pCon.GetAnimTime() / 4));

                comboNum = 2;
            }

            // Boot Launch
            if (Input.GetButtonUp(pCon.controls.launch))
            {
                SetAttackStats(0, 0, 35);
                pCon.animator.SetTrigger("kick");
                StartCoroutine(AttackCoolDown(pCon.GetAnimTime()/ 4));
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
