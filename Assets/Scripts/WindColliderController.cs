using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindColliderController : MonoBehaviour
{
    public bool isParticleCollision = false;

    private void LateUpdate()
    {
        print("false");
        isParticleCollision = false;
    }

    void OnParticleCollision(GameObject other)
    {
        isParticleCollision = true;
    }
}
