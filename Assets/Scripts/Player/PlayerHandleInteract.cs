using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandleInteract : MonoBehaviour
{
    //[Space]
    //[Header("PLAYER STATS:")]
    //public float hpStart;
    //public float hpCurrent;

    private PlayerController pCon;
    private PlayerFeedback pFeedBack;

    void Start()
    {
        pFeedBack = GetComponent<PlayerFeedback>();
        pCon = GetComponent<PlayerController>();
    }

    public void Interaction(Interact_Base _interactableItem)
    {
        if (_interactableItem != null)
        {
            if (_interactableItem.GetComponent<Spikes_Interactable>())
            {
                Debug.Log("Spikes damage player.");
                StartCoroutine(pFeedBack.IFlashRed());
            }
        }
    }

    public void TakeDamage(Vector2 _hitDirection, float _damage, float _knockBack, float _knockUp, float _stunTime)
    {
        //hpCurrent -= _damage;
        StartCoroutine(pCon.PlayerKnocked(_hitDirection, _knockBack, _knockUp, _stunTime));
    }
}
