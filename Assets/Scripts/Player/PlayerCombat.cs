﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("STATES:")]
    [Tooltip("The time between attacks required to continue the combo chain.")]
    public float maxComboTime = 0.5f;
    [Tooltip("The minumum time between the player is able to register a second attack.")]
    public float attackSpacing = 0.2f;
    private bool canAttack = true;
    private bool comboAttacking = false;
    private float timeBetweenCombos = 0;
    private int comboNum = 1;
    private float damage;
    private float knockback;
    private float knockup;

    [Space]
    [Header("PUNCH 1:")]
    public float p1_damage;
    public float p1_knockback;
    public float p1_knockUp;

    [Space]
    [Header("PUNCH 2:")]
    public float p2_damage;
    public float p2_knockback;
    public float p2_knockUp;

    [Space]
    [Header("PUNCH 3:")]
    public float p3_damage;
    public float p3_knockback;
    public float p3_knockUp;

    [Space]
    [Header("BOOT KICK:")]
    public float b_damage;
    public float b_knockback;
    public float b_knockUp;

    [Space]
    [Header("REFERENCES:")]
    private Animator animator;
    private PlayerController pCon;

    void Start() 
    {
        pCon = GetComponent<PlayerController>();
        animator = pCon.animator;
    }

    void Update()
    {
        Attacks();
        ComboTimer();
    }

    public float GetAnimTime()
    {
        if (animator != null)
        {
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

            if (clipInfo.Length > 0 && clipInfo != null)
            {
                return clipInfo[0].clip.length;
            }
        }

        return 0;
    }

    private void ComboTimer()
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
            if (Input.GetButtonDown(pCon.controls.punch) && comboAttacking && comboNum == 3)
            {
                timeBetweenCombos = 0;
                SetAttackStats(p3_damage, p3_knockback, p3_knockUp);
                pCon.animator.SetTrigger("punch3");
                StartCoroutine(AttackCoolDown(attackSpacing));

                comboNum = 1;
            }
            else if (Input.GetButtonDown(pCon.controls.punch) && comboAttacking && comboNum == 2)
            {
                timeBetweenCombos = 0;
                SetAttackStats(p2_damage, p2_knockback, p2_knockUp);
                pCon.animator.SetTrigger("punch2");
                StartCoroutine(AttackCoolDown(attackSpacing));

                comboNum = 3;
            }
            else if (Input.GetButtonDown(pCon.controls.punch) && comboNum == 1)
            {
                comboAttacking = true;
                timeBetweenCombos = 0;

                SetAttackStats(p1_damage, p1_knockback, p1_knockUp);
                pCon.animator.SetTrigger("punch1");
                StartCoroutine(AttackCoolDown(attackSpacing));

                comboNum = 2;
            }

            // Boot Launch
            if (Input.GetButton(pCon.controls.launch))
            {
                SetAttackStats(b_damage, b_knockback, b_knockUp);
                pCon.animator.SetTrigger("kick");
                StartCoroutine(AttackCoolDown(pCon.GetAnimTime()));
            }
        }
    }

    IEnumerator AttackCoolDown(float _time)
    {
        canAttack = false;
        //pCon.CanFlipSprite();

        yield return new WaitForSeconds(_time);

        //pCon.CanFlipSprite();
        canAttack = true;

    }

    void SetAttackStats(float _dmg, float _knkBk, float _knkUp)
    {
        damage = _dmg;
        knockback = _knkBk;
        knockup = _knkUp;
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
                    hitObj.GetComponent<Knockback>().IsKnocked(pCon.accessibleDirection, damage, knockback, knockup);
                }
                else
                {
                    Debug.Log("No Knockback component on enemy!");
                }           
            }
        }
    }
}
