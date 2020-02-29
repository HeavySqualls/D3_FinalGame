using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicSpill : Interact_Base
{
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
        pRecieveDamage = other.GetComponent<RecieveDamage>();

        if (pRecieveDamage != null && !isDamageDelay)
        {
            isDamageDelay = true;
            pRecieveDamage.GetHit(hitDirection, damage, knockBack, knockUp, stunTime);
            StartCoroutine(DamageDelay());
        }
    }

    IEnumerator DamageDelay()
    {       
        yield return new WaitForSeconds(damageDelay);

        isDamageDelay = false;
    }
}
