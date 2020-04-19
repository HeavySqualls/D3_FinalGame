using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Variables:")]
    [Tooltip("The time between attacks required to continue the combo chain.")]
    [SerializeField] float maxComboTime = 0.5f;
    [Tooltip("The minumum time between the player is able to register a second attack.")]
    [SerializeField] float attackSpacing = 0.2f;
    [Tooltip("The radius of the circle cast to determine size of the hitbox.")]
    [SerializeField] float circleCastRadius = 0.5f;
    [Tooltip("The distance from the center of the sprite that the circle cast will appear.")]
    [SerializeField] float castDistance = 0.85f;
    [SerializeField] float airDrainAmount = 2f;
    float timeBetweenCombos = 0;
    int comboNum = 1;
    int currentAttackNum = 1;

    [Space]
    [Header("States:")]
    private bool canAttack = true;
    private bool canCast = true;
    private bool comboAttacking = false;
    private float damage;
    private float knockback;
    private float knockup;
    private float stunTime;

    [Space]
    [Header("Punch 1:")]
    public float p1_damage;
    public float p1_knockback;
    public float p1_knockUp;
    public float p1_stunTime;

    [Space]
    [Header("Punch 2:")]
    public float p2_damage;
    public float p2_knockback;
    public float p2_knockUp;
    public float p2_stunTime;

    [Space]
    [Header("Punch 3:")]
    public float p3_damage;
    public float p3_knockback;
    public float p3_knockUp;
    public float p3_stunTime;

    [Space]
    [Header("Boot Kick:")]
    public float b_damage;
    public float b_knockback;
    public float b_knockUp;
    public float b_stunTime;

    [Space]
    [Header("Audio:")]


    [Space]
    [Header("References:")]
    private AirTankController pUI;
    private Animator animator;
    private PlayerController pCon;
    private PlayerFeedback pFeedback;
    private PlayerAudioController pAudio;
    LayerMask interactableLayerMask;
    int enemyLayer = 13;
    int interactablesLayer = 15;

    void Start() 
    {
        interactableLayerMask = ((1 << enemyLayer) | (1 << interactablesLayer));
        pUI = GetComponent<AirTankController>();
        pCon = GetComponent<PlayerController>();
        pFeedback = GetComponent<PlayerFeedback>();
        pAudio = GetComponent<PlayerAudioController>();
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
                pFeedback.AttackParticles("AttackParticleSystem-Heavy");
                pAudio.PlayPunchSound(3);
                timeBetweenCombos = 0;
                SetAttackStats(p3_damage, p3_knockback, p3_knockUp, p2_stunTime);
                pCon.animator.SetTrigger("punch3");
                StartCoroutine(AttackCoolDown(attackSpacing));
                StartCoroutine(CastForHit());
                currentAttackNum = 3;
                comboNum = 1;
            }
            else if (Input.GetButtonDown(pCon.controls.punch) && comboAttacking && comboNum == 2)
            {
                pFeedback.AttackParticles("AttackParticleSystem-Medium");
                pAudio.PlayPunchSound(2);
                timeBetweenCombos = 0;
                SetAttackStats(p2_damage, p2_knockback, p2_knockUp, p2_stunTime);
                pCon.animator.SetTrigger("punch2");
                StartCoroutine(AttackCoolDown(attackSpacing));
                StartCoroutine(CastForHit());
                currentAttackNum = 2;
                comboNum = 3;
            }
            else if (Input.GetButtonDown(pCon.controls.punch) && comboNum == 1)
            {
                pAudio.PlayPunchSound(1);
                comboAttacking = true;
                timeBetweenCombos = 0;
                SetAttackStats(p1_damage, p1_knockback, p1_knockUp, p1_stunTime);
                pCon.animator.SetTrigger("punch1");
                StartCoroutine(AttackCoolDown(attackSpacing));
                StartCoroutine(CastForHit());
                currentAttackNum = 1;
                comboNum = 2;
            }

            // Boot Launch
            if (Input.GetButton(pCon.controls.ability_1))
            {
                if (pUI.airInCan >= airDrainAmount)
                {
                    SetAttackStats(b_damage, b_knockback, b_knockUp, b_stunTime);
                    pCon.animator.SetTrigger("kick");
                    pUI.UseAirInTank(airDrainAmount);
                    StartCoroutine(AttackCoolDown(pCon.GetAnimTime()));
                    StartCoroutine(CastForHit());
                }
                else
                {
                    pUI.UseAirInTank(0);
                    print("No air left in tank!");
                }
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

    float castTime = 0.2f;

    IEnumerator CastForHit()
    {
        yield return new WaitForSeconds(0.01f);

        float timer = 0;

        while (timer < castTime && canCast)
        {
            CastForEnemies();
            timer += Time.deltaTime;
            yield return null;
        }

        canCast = false;
    }

    void SetAttackStats(float _dmg, float _knkBk, float _knkUp, float _stunTime)
    {
        damage = _dmg;
        knockback = _knkBk;
        knockup = _knkUp;
        stunTime = _stunTime;
    }

    //RaycastHit2D[] hits = Physics2D.OverlapBoxAll(gameObject.transform.position, Vector2(1, 2), pCon.accessibleDirection, circleCastDistance, interactableLayerMask);
    public Vector2 boxCast = new Vector2(1, 3);
    //private GameObject hitBox;
    //private bool isCasting = false;
    //private Vector3 spawnPos;
    public void CastForEnemies()
    {
        //spawnPos = gameObject.transform.position + gameObject.transform.right * circleCastDistance;

        if (canCast)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(gameObject.transform.position, boxCast, 0, pCon.accessibleDirection, castDistance, interactableLayerMask);

            //if (!isCasting)
            //{
            //    hitBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    hitBox.transform.localScale = boxCast;
            //    hitBox.transform.position = spawnPos;
            //    hitBox.transform.SetParent(gameObject.transform);

            //    isCasting = true;
            //    DestroyHitBox();
            //}

            foreach (RaycastHit2D hit in hits)
            {
                RecieveDamage recieveDamage = hit.collider.gameObject.GetComponent<RecieveDamage>();

                if (recieveDamage != null && !hit.collider.isTrigger)
                {
                    //print("hit: " + recieveDamage.name);
                    pAudio.PlayConnectSound(currentAttackNum);
                    recieveDamage.GetHit(pCon.accessibleDirection, damage, knockback, knockup, stunTime);
                    
                    canCast = false;
                    //pCon.PlayerKnocked(-pCon.accessibleDirection, 20, 0f, 0.2f);
                }
            }
        }
    }

    //IEnumerator DestroyHitBox()
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    Destroy(hitBox);
    //    isCasting = false;
    //}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
