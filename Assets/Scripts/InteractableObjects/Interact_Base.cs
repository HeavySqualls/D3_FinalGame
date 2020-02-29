using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Base : MonoBehaviour
{
    [Tooltip("How much damage will this object cause? (put 0 if none)")]
    [SerializeField] protected float damage;
    [Tooltip("How far will this object knock another object back? (put 0 if none)")]
    [SerializeField] protected float knockBack;
    [Tooltip("How far will this object knock another object up in the air? (put 0 if none)")]
    [SerializeField] protected float knockUp;
    [Tooltip("How long will this stun the enemy? (put 0 if none)")]
    [SerializeField] protected float stunTime;
    [Tooltip("What direction will this push the interacted object to?")]
    [SerializeField] protected Vector2 hitDirection;

    public bool isInteractable = true;
    public bool isInstantPickup = false;

    // Class attached to the player
    [SerializeField] protected RecieveDamage pRecieveDamage;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            pRecieveDamage = other.GetComponent<RecieveDamage>();

            if (pRecieveDamage == null)
            {
                Debug.LogWarning("No RecieveDamage component attached to player!");
            }
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        pRecieveDamage = other.GetComponent<RecieveDamage>();

        if (pRecieveDamage != null)
        {
            pRecieveDamage = null;
        }
    }
}
