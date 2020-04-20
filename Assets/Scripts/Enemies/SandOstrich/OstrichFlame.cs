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
            so.DealFlameParticleDamage();
        }
    }
}
