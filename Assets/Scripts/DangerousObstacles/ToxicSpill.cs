using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicSpill : Interact_Base
{
    [Tooltip("How much damage will this object cause? (put 0 if none)")]
    public float damage;
    [Tooltip("How far will this object knock another object back? (put 0 if none)")]
    public float knockBack;
    [Tooltip("How far will this object knock another object up in the air? (put 0 if none)")]
    public float knockUp;
    [Tooltip("How long will this stun the enemy? (put 0 if none)")]
    public float stunTime;
    [Tooltip("What direction will this push the interacted object to?")]
    public Vector2 hitDirection;

    public float damageDelay;
    private bool isDamageDelay = false;

    public float maxEmmisionPause = 3f;
    private float emmisionPause = 0f; 

    ParticleSystem partSyst;

    void Start()
    {
        partSyst = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!partSyst.isEmitting)
        {
            emmisionPause += Time.deltaTime;

            if (emmisionPause >= maxEmmisionPause)
            {
                partSyst.Play();
            }
        }
    }

    void OnParticleCollision(GameObject other)
    {
        pThatHitMe = other.GetComponent<PlayerHandleInteract>();

        if (pThatHitMe != null && !isDamageDelay)
        {
            print("oww");
            pThatHitMe.HitDangerousObstacle(this);
            isDamageDelay = true;
            pThatHitMe.GetComponent<RecieveDamage>().GetHit(hitDirection, damage, knockBack, knockUp, stunTime);
            StartCoroutine(DamageDelay());
        }
    }

    IEnumerator DamageDelay()
    {       
        yield return new WaitForSeconds(damageDelay);

        isDamageDelay = false;
    }
}
