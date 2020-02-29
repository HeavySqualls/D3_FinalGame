using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OstrichFlame : MonoBehaviour
{
    SandOstrich so;

    void Start()
    {
        so = GetComponentInParent<SandOstrich>();
    }

    void OnParticleCollision(GameObject other)
    {
        if (other == so.target)
        {
            //pRecieveDamage = target.GetComponent<RecieveDamage>();

            if (!so.isDamageDelay)
            {
                so.DealDamage();
            }
        }
    }
}
