using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandleInteract : MonoBehaviour
{
    [SerializeField] Inventory inventory;

    //[Space]
    //[Header("PLAYER STATS:")]
    //public float hpStart;
    //public float hpCurrent;
    [SerializeField] KeyCode inputKey;
    [SerializeField] PickUpItem pickupItem;
    private PlayerController pCon;
    private PlayerFeedback pFeedBack;

    void Start()
    {
        pFeedBack = GetComponent<PlayerFeedback>();
        pCon = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(inputKey))
        {
            if (pickupItem != null)
            {
                inventory.AddItem(pickupItem.item);
                pickupItem.OnItemPickedUp();
            }
            else
            {
                Debug.Log("Nothing to pick up here!");
            }
        }
    }

    public void HitDangerousObstacle(Interact_Base _interactableItem)
    {
        if (_interactableItem != null)
        {
            if (_interactableItem.GetComponent<DangerousObstacle>() || _interactableItem.GetComponent<ToxicSpill>())
            {
                print("flash");
                StartCoroutine(pFeedBack.IFlashRed());
            }
        }
    }

    public void TakeDamage(Vector2 _hitDirection, float _damage, float _knockBack, float _knockUp, float _stunTime)
    {
        //hpCurrent -= _damage;
        StartCoroutine(pCon.PlayerKnocked(_hitDirection, _knockBack, _knockUp, _stunTime));
    }


    // Assign Pick up item to player for pick up
    public void AssignPickUpItem (PickUpItem _pickup)
    {
        pickupItem = _pickup;
    }

    // Remove pick up item from player pick up
    public void UnAssignPickUpItem()
    {
        pickupItem = null;
    }

}
