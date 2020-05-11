using System.Collections;
using UnityEngine;

public class SandOstrich : Enemy_Turret_Base
{
    [Space]
    [Header("--- SAND OSTRICH ---")]

    [Space]
    [Header("States: ")]
    public bool isDamageDelay = false;
    public bool isStartInHole = false;
    public bool isInHole = false;
    private bool isPreBurp = false;
    private bool isTracking = false;

    [Space]
    [Header("Combat: ")]
    [Tooltip("The transform of the empty game object used to position the units hit box.")]
    public Transform hitBoxPos;
    [Tooltip("The time in between getting hit, which the player can get hurt again by this units attack.")]
    [SerializeField] float damageDelay = 1f;
    [Tooltip("The current distance the player is to the unit.")]
    [SerializeField] float distanceToTarget;
    [Tooltip("The distanceToTarget distance before the unit will enter its burp animation phase.")]
    [SerializeField] float burpDistance;
    [Tooltip("The distanceToTarget distance before the unit will enter its flame attack animation phase.")]
    [SerializeField] float flameDistance;

    [Space]
    [Header("Audio:")]
    [SerializeField] AudioClip[] idleSounds;
    [SerializeField] float idleVolume = 0.3f;
    float currentNoiseInterval;
    [SerializeField] AudioClip enterExitSound;
    [SerializeField] float enterExitVolume = 0.3f;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] float hurtVolume = 0.3f;
    [SerializeField] AudioClip deathSound;
    [SerializeField] float deathVolume = 0.3f;
    [SerializeField] AudioClip preBurpSound;
    [SerializeField] float preBurpVolume = 0.3f;
    [SerializeField] AudioClip flameBurpSound;
    [SerializeField] float flameBurpVolume = 0.3f;
    [SerializeField] AudioSource localAudioSource;
    [SerializeField] AudioSource oneShotSource;

    [Space]
    [Header("References: ")]
    public Vector3 partOffsetPos;
    GameObject disposablePartSyst;
    RecieveDamage pRecieveDamage;
    ParticleSystem partSyst;

    protected override void Start()
    {
        base.Start();

        currentState = State.Idle;
        partSyst = GetComponentInChildren<ParticleSystem>();

        if (isStartInHole)
        {
            animator.SetTrigger("isGoingInHole");
            isInHole = true;
        }
        else
            isInHole = false;

        Toolbox.GetInstance().GetAudioManager().AddAudioSources(localAudioSource);
        Toolbox.GetInstance().GetAudioManager().AddAudioSources(oneShotSource);
    }

    private void PlayOneShotAudio(AudioClip _clip, float _volume)
    {
        oneShotSource.volume = _volume;
        oneShotSource.PlayOneShot(_clip);
    }

    private void PlayRandomNoise(float _volume, params AudioClip[] _audioClips)
    {
        int rand = Random.Range(0, _audioClips.Length);

        PlayOneShotAudio(_audioClips[rand], _volume);
    }

    // USED BY CINEMATIC CONTROLLER FOR SCENE 1 INTRO 
    public void SetToIdle() // TODO: Find a better way of implementing this
    {
        Destroy(gameObject);
    }

    protected override void Update()
    {
        base.Update();

        switch (this.currentState)
        {
            case State.Idle:
                this.Idle();
                break;
            case State.Prone:
                this.Prone();
                break;
            case State.Attacking:
                this.Attacking();
                break;
            case State.Hurt:
                this.Hurt();
                break;
            case State.Dead:
                this.Dead();
                break;
        }

        if (isTracking && !isDead)
        {
            TrackTargetDistance();
        }

        Animations();
    }


    // << --------------------------------------- ANIMATIONS -------------------------------- >> //

    private void Animations()
    {
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isPreBurp", isPreBurp);
        animator.SetBool("isDead", isDead);
    }


    // << --------------------------------------- STATES -------------------------------- >> //

    private void Idle()
    {
        currentNoiseInterval -= Time.deltaTime;

        if (currentNoiseInterval <= 0 && !isInHole)
        {
            float randInt = Random.Range(1, 3);
            currentNoiseInterval = randInt;
            PlayRandomNoise(idleVolume, idleSounds);
            print("play random sound");
        }

        if (localAudioSource.isPlaying)
        {
            localAudioSource.Stop();
        }

        isIdle = true;
        isPreBurp = false;
    }

    private void Prone()
    {
        // Play pre-burping noise
        if (!localAudioSource.isPlaying)
        {
            localAudioSource.clip = preBurpSound;
            localAudioSource.volume = preBurpVolume;
            localAudioSource.Play();
        }

        isIdle = false;
        isPreBurp = true;
        FaceTarget();
        DetectPlayerForFireAttack();
    }

    private void Attacking()
    {
        FaceTarget();

        // ** Animation is called when the attack state is switched on
    }

    private void Hurt()
    {
        // Do nothing and wait for state change
    }

    private void Dead()
    {
        // Do nothing and wait to be destroyed
    }


    // << ------------------------------------- CHECK FOR PROXIMITY -------------------------------- >> //

    // Check to see if the player has entered the trigger zone to get the sand ostrich out of its hole
    // Set the player as the target to be tracked
    private void OnTriggerEnter2D(Collider2D other) 
    {
        pRecieveDamage = other.GetComponent<RecieveDamage>();

        if (pRecieveDamage != null)
        {
            if (isInHole)
            {
                target = pRecieveDamage.gameObject;
                isInHole = false;
                PlayOneShotAudio(enterExitSound, enterExitVolume);
                animator.SetTrigger("isExitingHole");
                isTracking = true;
            }
        }
    }

    // Check to see if the player has left the trigger zone to get the sand ostrich back in to its hole
    // Set the target as null
    private void OnTriggerExit2D(Collider2D other)
    {
        pRecieveDamage = other.GetComponent<RecieveDamage>();

        if (pRecieveDamage != null)
        {
            print("go in hole");
            isTracking = false;
            isInHole = true;
            pRecieveDamage = null;
            target = null;
            PlayOneShotAudio(enterExitSound, enterExitVolume);
            animator.SetTrigger("isGoingInHole");
        }
    }

    // When the Sand Ostrch is out of its hole, track the distance to the player to decide which state to be in
    private void TrackTargetDistance()
    {
        if (!isDead)
        {
            distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distanceToTarget <= burpDistance)
                currentState = State.Prone;
            else
                currentState = State.Idle;
        }
    }


    // << ------------------------------------- RAYCAST CHECKS -------------------------------- >> //

    private void FaceTarget()
    {
        Vector2 targetDirection;

        if (target != null)
        {
            targetDirection = (transform.position - target.transform.position);
            targetDirection.x = Mathf.Clamp(targetDirection.x, -1f, 1f);

            if (targetDirection.x > 0 && direction == Vector2.right || targetDirection.x < 0 && direction == Vector2.left)
            {
                FlipSprite();
            }
        }
    }

    // Detect the players proximity with another raycast, if player is touched by this raycast, trigger flameburp animation + attack
    void DetectPlayerForFireAttack()
    {
        RaycastHit2D huntingInfo = Physics2D.Raycast(eyeRange.position, direction, eyeRangeDistance, playerLayerMask);
        Debug.DrawRay(eyeRange.position, direction * eyeRangeDistance, Color.yellow);

        // If we have detected the player and are not currently attacking them
        if (huntingInfo.collider != null && !huntingInfo.collider.gameObject.GetComponent<PlayerHealthSystem>().isDead)
        {
            if (localAudioSource.isPlaying)
                localAudioSource.Stop();

            currentState = State.Attacking;
            animator.SetTrigger("isFlameBurp");
        }
    }


    // << --------------------------------------- COMBAT -------------------------------- >> //

    // Called from inside the animator during the "SandOstrich_Hit" animation 
    // Cast happens when the sand ostrich's head swings back after being punched.
    public void PeckAtPlayerCast() 
    {
        if (!objectHit)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(hitBoxPos.position, 0.5f, direction, 0.5f, playerLayerMask);
            foreach (RaycastHit2D hit in hits)
            {
                RecieveDamage hitObj = hit.collider.gameObject.GetComponent<RecieveDamage>();

                if (hitObj != null)
                {
                    print("hit: " + hitObj.name);

                    hitObj.GetComponent<RecieveDamage>().GetHit(direction, damageOutput, knockBack, knockUp, stunTime);
                    objectHit = true; // prevents enemy attacking the player every frame - gets reset in AfterAttackCooldown()
                    AttackCoolDown();
                }
            }
        }
    }

    // Called from inside the animator during the "SandOstrich_FlameBurp" animation
    public void FlameBurp()
    {
        if (!isTurretPaused)
        {
            PlayOneShotAudio(flameBurpSound, flameBurpVolume);
            partSyst.Play();
        }

        if (!isCooldown)
        {
            StartCoroutine(AttackCoolDown());
            isCooldown = true;
        }
    }

    // Called from the sand ostrich's flame burp particle system
    public void DealFlameParticleDamage()
    {
        if (pRecieveDamage != null)
            pRecieveDamage.GetHit(direction, damageOutput, knockBack, knockUp, stunTime);
    }

    // What happens after the sand ostrich has been attacked
    protected override void AfterThisUnitWasAttacked()
    {
        if (!isDead)
        {
            base.AfterThisUnitWasAttacked();
            partSyst.Stop();
            PlayOneShotAudio(hurtSound, hurtVolume);

            disposablePartSyst = Instantiate(Resources.Load("OstrichFeathersParticles", typeof(GameObject))) as GameObject;
            disposablePartSyst.transform.position = gameObject.transform.position + partOffsetPos;
            Destroy(disposablePartSyst, 0.8f);

            if (currentHP <= 0)
                KillUnit();
            else
            {
                currentState = State.Hurt;
                animator.SetTrigger("isHit");
                StartCoroutine(AttackCoolDown());
            }
        }
    }

    // What  happens after this unit attacks
    private IEnumerator AttackCoolDown()
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

    protected override void KillUnit()
    {
        base.KillUnit();

        PlayOneShotAudio(deathSound, deathVolume);
        isIdle = false;
        isPreBurp = false;
        isDead = true;

        currentState = State.Dead;
    } 
}
