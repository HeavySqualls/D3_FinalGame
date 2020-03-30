using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicSpill : Interact_Base
{
    public bool isTurnedOn = true;
    public float damageDelay;
    private bool isDamageDelay = false;

    public float maxEmmisionPause = 3f;
    private float emmisionPause = 0f;

    [SerializeField] ParticleSystem partSyst;

    void Start()
    {
        partSyst = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (!partSyst.isEmitting && isTurnedOn)
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
        //print(other.name);

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

    public void TurnOffSpillPipe()
    {
        isTurnedOn = false;

        var main = partSyst.main;
        main.loop = false;
    }
}
