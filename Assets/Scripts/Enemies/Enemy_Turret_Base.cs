using System.Collections;
using UnityEngine;

public class Enemy_Turret_Base : MonoBehaviour
{
    public enum State { Idle, Prone, Attacking, Hurt, Dead }

    [Space]
    [Header("--- ENEMY TURRET BASE ---")]

    public State startingState;
    protected State currentState;

    public bool isTurretPaused = false;
    protected bool isIdle;
    protected bool isPatrolling;
    protected bool isHunting;
    protected bool isDead;
    protected bool isInAirInWind;
    protected bool isHit = false;
    protected bool isHurt = false;
    protected bool isCooldown = false;
    protected bool objectHit = false;

    [Space]
    [Header("Unit Health:")]

    [Tooltip("The current amount of health on the unit.")]
    [SerializeField] public float currentHP;
    [Tooltip("The starting health of the unit.")]
    [SerializeField] float startHP = 10f;

    [Space]
    [Header("Enemy Combat:")]

    [Tooltip("The damage this enemy will cause to the player.")]
    public int damageOutput = 2;
    [Tooltip("The time between attacks on the player.")]
    public float attackCooldown = 0.5f;
    [Tooltip("The distance the player will be knocked back when hit by the player. (total distance is relative to the duration of the stun time variable)")]
    [SerializeField] protected float knockBack;
    [Tooltip("The distance the player will be knocked in tp the air when hit by this unit. (total distance is relative to the duration of the stun time variable)")]
    [SerializeField] protected float knockUp;
    [Tooltip("The amount of time the player will be stunned when hit by the unit.")]
    [SerializeField] protected float stunTime;

    [Space]
    [Header("Raycasts:")]

    [Tooltip("The location of the units eye range.")]
    [SerializeField] protected Transform eyeRange;
    [Tooltip("The distance of the units eye range.")]
    [SerializeField] protected float eyeRangeDistance = 10f;

    [Space]
    [Header("Enemy References:")]
    protected Vector2 direction;
    protected LayerMask playerLayerMask;
    protected Animator animator;
    public GameObject target;

    private void OnEnable()
    {
        SpawnManager.onResetLevelObjects += ResetTurretUnit;
    }

    private void OnDisable()
    {
        SpawnManager.onResetLevelObjects -= ResetTurretUnit;
    }

    protected virtual void Start()
    {
        Toolbox.GetInstance().GetLevelManager().AddTurretEnemies(this);

        currentState = startingState;

        currentHP = startHP;
        animator = GetComponent<Animator>();

        playerLayerMask = ((1 << 12));

        // Set the enemy in the correct direction at Start
        if (direction == Vector2.zero)
            direction = Vector2.right;
    }

    protected virtual void Update()
    {
        // Dealt with in the relative enemy controller
    }

    public void ResetTurretUnit()
    {
        isDead = false;
        Debug.Log(gameObject.name + " was reset");
        gameObject.SetActive(true);
        currentState = startingState;
        currentHP = startHP;
    }

    // << --------------------------------------- COMBAT -------------------------------- >> //

    protected IEnumerator AttackCoolDown()
    {

        yield return new WaitForSeconds(attackCooldown);

        if (target == null)
            currentState = State.Idle;
        else
            currentState = State.Prone;

        isCooldown = false;
        objectHit = false;
        yield break;
    }

    protected virtual void AfterThisUnitWasAttacked()
    {
        // Behaviour handled in specific enemy controller
    }

    protected void AfterAttackCooldown()
    {

    }

    protected virtual void KillUnit()
    {
        // Behaviour handled in specific enemy controller
        //Toolbox.GetInstance().GetLevelManager().RemoveTurretEnemies(this);
        //currentState = State.Dead;
    }

    public void ThisUnitHit(Vector2 _hitDirection, float _knockBack, float _knockUp, float _stunTime)
    {
        if (currentHP <= 0)
            KillUnit();
        else
            AfterThisUnitWasAttacked();
    }

    // << ------------------------------------- FLIP SPRITE -------------------------------- >> //

    protected void FlipSprite()
    {
        if (direction == Vector2.right)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            direction = Vector2.left;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            direction = Vector2.right;
        }
    }
}
