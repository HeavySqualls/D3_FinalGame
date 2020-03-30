using System.Collections;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        var collision = other.gameObject.GetComponent<ParticleSystem>().collision;
        collision.lifetimeLoss = 1;      
    }
}
