using UnityEngine;

public class PlayerHandleInteract : MonoBehaviour
{
    [SerializeField] Inventory inventory;

    // Gets assigned when in the trigger zone of a pick up object
    [SerializeField] PickUpItem currentPickupItem = null;
    [SerializeField] LootBoxPanel currentLootBoxPanel = null;

    // Gets assigned when in the trigger zone of a swtich object
    [SerializeField] InteractableSwitch currentSwtich = null;
    PlayerInventoryHandler pInventoryHandler;
    PlayerController pCon;


    void Start()
    {
        pInventoryHandler = Toolbox.GetInstance().GetPlayerManager().GetPlayerInventoryHandler();
        inventory = Toolbox.GetInstance().GetPlayerManager().GetInventoryManager().inventory;
        pCon = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetButtonDown(pCon.controls.interact) || Controls.IsDown)
        {
            // If the player is interacting with a pickup object:
            if (currentPickupItem != null)
            {
                if (currentPickupItem.isLootBox)
                {
                    if (!currentPickupItem.isOpen)
                    {
                        currentPickupItem.OpenLootBox();
                        pInventoryHandler.lootBoxPanel = currentPickupItem.lootBoxPanel.gameObject;
                    }
                    else
                    {
                        currentPickupItem.CloseLootBox();
                        pInventoryHandler.lootBoxPanel = null;
                    }
                }
                else
                {
                    if (inventory != null)
                        inventory.AddItem(currentPickupItem.item);
                    else                  
                        Debug.LogError("PlayerHandleInteract does not have a reference to the Inventory.cs component!");                                  
                }

                currentPickupItem.OnItemPickedUp();
            }
            else
            {
                Debug.Log("Nothing to pick up here!");
            }

            // If the player is interacting with a switch object:
            if (currentSwtich != null)
            {
                currentSwtich.OpenCloseDoor();
            }
        }

        if ((Input.GetButtonDown(pCon.controls.quickLoot) || Controls.IsLeft) && currentPickupItem != null && currentPickupItem.isOpen)
        {
            currentPickupItem.QuickLoot();
        }
    }


    // ---- SWTICH OBJECTS ---- //

    public void RecieveSwitchReference(InteractableSwitch _interSwitch)
    {
        currentSwtich = _interSwitch;
    }

    public void UnAssignSwitchReference()
    {
        currentSwtich = null;
    }


    // ---- PICK UP OBJECTS ---- //

    // Assign Pick up item to player for pick up
    public void RecievePickupItemReferences (PickUpItem _pickup, LootBoxPanel _lootPanel)
    {
        currentPickupItem = _pickup;

        if (currentPickupItem.isLootBox)
            currentLootBoxPanel = _lootPanel;
     }

    // Remove pick up item from player pick up
    public void UnAssignPickUpItemReferences()
    {
        currentPickupItem = null;
        currentLootBoxPanel = null;
    }
}
