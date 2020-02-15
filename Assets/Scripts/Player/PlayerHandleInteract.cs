using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandleInteract : MonoBehaviour
{
    [SerializeField] Inventory inventory;

    [SerializeField] KeyCode interactKey;

    // Gets assigned when in the trigger zone of a pick up object
    [SerializeField] PickUpItem pickupItem = null;
    [SerializeField] LootBoxPanel lootBoxPanel = null;

    private PlayerController pCon;
    private PlayerFeedback pFeedBack;

    void Start()
    {
        pFeedBack = GetComponent<PlayerFeedback>();
        pCon = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))/*(pCon.controls.interact))*/ //TODO: Why the fuck does this "interact" reference not work??
        {
            if (pickupItem != null)
            {
                if (pickupItem.isLootBox)
                {
                    pickupItem.OpenCloseLootBox();
                }
                else
                {
                    if (inventory != null)
                        inventory.AddItem(pickupItem.item);
                    else                  
                        Debug.LogError("PlayerHandleInteract does not have a reference to the Inventory.cs component!");                                  
                }

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
    public void SendPickupItemReferences (PickUpItem _pickup, LootBoxPanel _lootPanel)
    {
        pickupItem = _pickup;

        if (pickupItem.isLootBox)
            lootBoxPanel = _lootPanel;
     }

    // Remove pick up item from player pick up
    public void UnAssignPickUpItemReferences()
    {
        pickupItem = null;
        lootBoxPanel = null;
    }
}
