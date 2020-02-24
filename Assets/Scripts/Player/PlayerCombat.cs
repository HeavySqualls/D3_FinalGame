﻿using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("STATES:")]
    [Tooltip("The time between attacks required to continue the combo chain.")]
    public float maxComboTime = 0.5f;
    [Tooltip("The minumum time between the player is able to register a second attack.")]
    public float attackSpacing = 0.2f;

    [SerializeField] Transform hitboxPos;
    private bool canAttack = true;
    private bool canCast = true;
    private bool comboAttacking = false;
    private float timeBetweenCombos = 0;
    private int comboNum = 1;
    private float damage;
    private float knockback;
    private float knockup;
    private float stunTime;

    [Space]
    [Header("PUNCH 1:")]
    public float p1_damage;
    public float p1_knockback;
    public float p1_knockUp;
    public float p1_stunTime;

    [Space]
    [Header("PUNCH 2:")]
    public float p2_damage;
    public float p2_knockback;
    public float p2_knockUp;
    public float p2_stunTime;

    [Space]
    [Header("PUNCH 3:")]
    public float p3_damage;
    public float p3_knockback;
    public float p3_knockUp;
    public float p3_stunTime;

    [Space]
    [Header("BOOT KICK:")]
    public float b_damage;
    public float b_knockback;
    public float b_knockUp;
    public float b_stunTime;

    [Space]
    [Header("REFERENCES:")]
    private Animator animator;
    private PlayerController pCon;
    LayerMask interactableLayerMask;
    int enemyLayer = 13;
    int interactablesLayer = 15;

    void Start() 
    {
        interactableLayerMask = ((1 << enemyLayer) | (1 << interactablesLayer));
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
                SetAttackStats(p3_damage, p3_knockback, p3_knockUp, p2_stunTime);
                pCon.animator.SetTrigger("punch3");
                StartCoroutine(AttackCoolDown(attackSpacing));
                StartCoroutine(CastForHit());
                comboNum = 1;
            }
            else if (Input.GetButtonDown(pCon.controls.punch) && comboAttacking && comboNum == 2)
            {
                timeBetweenCombos = 0;
                SetAttackStats(p2_damage, p2_knockback, p2_knockUp, p2_stunTime);
                pCon.animator.SetTrigger("punch2");
                StartCoroutine(AttackCoolDown(attackSpacing));
                StartCoroutine(CastForHit());
                comboNum = 3;
            }
            else if (Input.GetButtonDown(pCon.controls.punch) && comboNum == 1)
            {
                comboAttacking = true;
                timeBetweenCombos = 0;

                SetAttackStats(p1_damage, p1_knockback, p1_knockUp, p1_stunTime);
                pCon.animator.SetTrigger("punch1");
                StartCoroutine(AttackCoolDown(attackSpacing));
                StartCoroutine(CastForHit());
                comboNum = 2;
            }

            // Boot Launch
            if (Input.GetButton(pCon.controls.ability_1))
            {
                SetAttackStats(b_damage, b_knockback, b_knockUp, b_stunTime);
                pCon.animator.SetTrigger("kick");
                StartCoroutine(AttackCoolDown(pCon.GetAnimTime()));
                StartCoroutine(CastForHit());
            }
        }
    }

    IEnumerator AttackCoolDown(float _time)
    {
        canAttack = false;
        pCon.canMove = false;

        yield return new WaitForSeconds(_time);

        pCon.canMove = true;
        canAttack = true;
        canCast = true;
    }

    IEnumerator CastForHit()
    {
        yield return new WaitForSeconds(0.01f);
        CastForEnemies();
        canCast = false;
    }

    void SetAttackStats(float _dmg, float _knkBk, float _knkUp, float _stunTime)
    {
        damage = _dmg;
        knockback = _knkBk;
        knockup = _knkUp;
        stunTime = _stunTime;
    }

    public void CastForEnemies()
    {
        if (canCast)
        {
            print("cast");

            RaycastHit2D[] hits = Physics2D.CircleCastAll(gameObject.transform.position, 0.95f, pCon.accessibleDirection, 0.95f, interactableLayerMask);

            foreach (RaycastHit2D hit in hits)
            {
                RecieveDamage recieveDamage = hit.collider.gameObject.GetComponent<RecieveDamage>();

                if (recieveDamage != null)
                {
                    print("hit: " + recieveDamage.name);
                    recieveDamage.GetHit(pCon.accessibleDirection, damage, knockback, knockup, stunTime);
                    //pCon.PlayerKnocked(pCon.accessibleDirection, 20, 6f, 0.2f);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.85f);
    }
}
