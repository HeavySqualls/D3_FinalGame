using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_Base : MonoBehaviour
{
    public bool isInteractable = true;
    public bool isInstantPickup = false;


    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHitInteractable pHitI = other.GetComponent<PlayerHitInteractable>();

        if (pHitI != null && isInteractable)
        {
            //pHitI.OnInteracted(other, this);
            //pCon = other.GetComponent<PlayerController>();
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        PlayerHitInteractable pHitI = other.GetComponent<PlayerHitInteractable>();

        if (pHitI != null)
        {
            //interactKey.enabled = false;
            //other.GetComponent<PlayerController>().interactableItem = null;
            //pCon = null;
            //pHitI = null;
        }
    }

    public virtual void OnInteracted() { }
}
