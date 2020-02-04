using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Base : MonoBehaviour
{
    public bool isInteractable = true;
    public bool isInstantPickup = false;

    // Class attached to the player
    [SerializeField] protected PlayerHandleInteract pThatHitMe;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        pThatHitMe = other.GetComponent<PlayerHandleInteract>();

        if (pThatHitMe != null)
        {
            pThatHitMe.Interaction(this);
        }
        else
        {
            return;
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        pThatHitMe = other.GetComponent<PlayerHandleInteract>();

        if (pThatHitMe != null)
        {
            pThatHitMe = null;
        }
        else
        {
            return;
        }
    }

    public virtual void OnInteracted() { }
}
