using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousObstacle : Interact_Base
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

    [Tooltip("The delay until the interacted object will be attacked again if still standing on the spikes")]
    public float damageDelay = 0.5f;
    private bool isDamageDelay = false;

    [Tooltip("Spikes and debris that will become bloodied")]
    public HashSet<MeshRenderer> spikeMeshes = new HashSet<MeshRenderer>();

    void Start()
    {
        FindEveryChild(gameObject.transform);
    }

    private void FindEveryChild(Transform parent)
    {
        int count = parent.childCount;

        for (int i = 0; i < count; i++)
        {
            spikeMeshes.Add(parent.GetChild(i).transform.GetComponent<MeshRenderer>());
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (pThatHitMe != null)
        {
            pThatHitMe.GetComponent<RecieveDamage>().GetHit(hitDirection, damage, knockBack, knockUp, stunTime);

            foreach (MeshRenderer spikeMR in spikeMeshes)
            {
                spikeMR.material.SetColor("_Color", Color.red);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        pThatHitMe = other.GetComponent<PlayerHandleInteract>();

        if (pThatHitMe != null && !isDamageDelay)
        {
            StartCoroutine(DamageDelay());
        }
    }

    IEnumerator DamageDelay()
    {
        isDamageDelay = true;

        yield return new WaitForSeconds(damageDelay);

        if (pThatHitMe != null)
        {
            pThatHitMe.GetComponent<RecieveDamage>().GetHit(hitDirection, damage, knockBack, knockUp, stunTime);
        }

        isDamageDelay = false;
    }
}
