using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TUTORIAL: https://www.youtube.com/watch?v=4JewzU_phTM

public class InventoryManager : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentPanel equipPanel;

    private void Awake()
    {
        // ... ok, some one clicked an item in Inventory.cs, lets equip it! 
        inventory.OnItemRightClickedEvent += EquipFromInventory;

        // ... ok, some one clicked an item in EquipmentPanel.cs, lets un-equip it! 
        equipPanel.OnItemRightClickedEvent += UnequipFromEquipPanel;
    }

    private void EquipFromInventory(sItem _item)
    {
        if (_item is sEquippableItem)
        {
            Equip((sEquippableItem)_item);
        }
    }

    private void UnequipFromEquipPanel(sItem _item)
    {
        print("Da fuq?");
        if (_item is sEquippableItem)
        {
            Unequip((sEquippableItem)_item);
        }
    }

    public void Equip(sEquippableItem _item)
    {
        if (inventory.RemoveItem(_item)) // if the item is able to be removed from the inventory 
        {
            sEquippableItem previousItem; // reference for the previous item 

            if (equipPanel.AddItem(_item, out previousItem)) // if we can add the item to the equipment panel
            {
                if (previousItem != null) // if there was an item in the equipment slot before adding this one
                {
                    inventory.AddItem(previousItem); // add the previous item in the slot back in to the inventory 
                }
            }
            else
            {
                inventory.AddItem(_item); // if we can't add the item to an equipment slot, return it to the inventory 
            }
        }
    }

    public void Unequip(sEquippableItem _item)
    {
        if (!inventory.IsFull() && equipPanel.RemoveItem(_item))
        {
            inventory.AddItem(_item);
            print("unequipped");
        }
        else
            Debug.Log("Can not unequip item! - inventory full!");
    }
}
