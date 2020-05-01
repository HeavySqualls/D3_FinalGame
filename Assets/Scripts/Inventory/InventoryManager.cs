using UnityEngine;
using UnityEngine.UI;
using Kryz.CharacterStats;

public class InventoryManager : MonoBehaviour
{
    public CharacterStat strength;
    public CharacterStat agility;
    public CharacterStat intellect;
    public CharacterStat vitality;

    public Inventory inventory;
    [SerializeField] EquipmentPanel equipPanel;
    [Tooltip("Set all the panels from the loot boxes in the level here.")]
    [SerializeField] LootBoxPanel[] lootBoxPanels;
    [SerializeField] StatPanel statPanel;
    [SerializeField] ItemToolTip itemTooltip;
    [SerializeField] Image draggableItem;

    private ItemSlot draggedSlot;

    private void Awake()
    {
        Toolbox.GetInstance().GetPlayerManager().SetInventoryManager(this);

        lootBoxPanels = FindObjectsOfType<LootBoxPanel>();
        
        // TODO: enable this again when we decide to start working with stats
        //statPanel.SetStats(strength, agility, intellect, vitality);
        //statPanel.UpdateStatValues();

        // ---- SET UP EVENTS: ---- //

        // Right click:
        inventory.OnRightClickEvent += Equip;
        equipPanel.OnRightClickEvent += Unequip;
        //lootPanel.OnRightClickEvent += TakeLoot;

        // Pointer Enter:
        inventory.OnPointerEnterEvent += ShowTooltip;
        equipPanel.OnPointerEnterEvent += ShowTooltip;
        //lootPanel.OnPointerEnterEvent += ShowTooltip;
        
        // Pointer Exit:
        inventory.OnPointerExitEvent += HideTooltip;
        equipPanel.OnPointerExitEvent += HideTooltip;
        //lootPanel.OnPointerExitEvent += HideTooltip;
        
        // Begin Drag
        inventory.OnBeginDragEvent += BeginDrag;
        equipPanel.OnBeginDragEvent += BeginDrag;
        //lootPanel.OnBeginDragEvent += BeginDrag;
        
        // End Drag:
        inventory.OnEndDragEvent += EndDrag;
        equipPanel.OnEndDragEvent += EndDrag;
        //lootPanel.OnEndDragEvent += EndDrag;
        
        // Drag 
        inventory.OnDragEvent += Drag;
        equipPanel.OnDragEvent += Drag;
        //lootPanel.OnDragEvent += Drag;
        
        // Drop
        inventory.OnDropEvent += Drop;
        equipPanel.OnDropEvent += Drop;
        //lootPanel.OnDropEvent += Drop;

        //Loot Box Panels
        foreach (LootBoxPanel lbp in lootBoxPanels)
        {
            lbp.OnRightClickEvent += TakeLoot;
            lbp.OnPointerEnterEvent += ShowTooltip;
            lbp.OnPointerExitEvent += HideTooltip;
            lbp.OnBeginDragEvent += BeginDrag;
            lbp.OnEndDragEvent += EndDrag;
            lbp.OnDragEvent += Drag;
            lbp.OnDropEvent += Drop;
        }
    }

    private void Equip(ItemSlot _itemSlot)
    {
        // if the item in the selected equipment slot is an equippable item, equip it!
        sEquippableItem equippableItem = _itemSlot.Item as sEquippableItem;
        if (equippableItem != null)
        {
            Equip(equippableItem);
            itemTooltip.HideToolTip();
        }
    }

    private void Unequip(ItemSlot _itemSlot)
    {
        // if the item in the selected equipment slot is an equippable item, unequip it!
        sEquippableItem equippableItem = _itemSlot.Item as sEquippableItem;
        
        if (equippableItem != null)
        {
            Unequip(equippableItem);

            itemTooltip.HideToolTip();

            // if the item is from a loot box, remove the item from inside it 
            if (_itemSlot as LootBoxSlot)
            {
                _itemSlot.Item = null;
            }
        }
    }

    public void QuickLoot(ItemSlot _quickLootItemSlot)
    {
        TakeLoot(_quickLootItemSlot);
    }

    private void TakeLoot(ItemSlot _lootSlot)
    {
        LootBoxSlot lootSlot = _lootSlot as LootBoxSlot;
        if (inventory.AddItem(_lootSlot.Item))
        {
            Debug.Log(_lootSlot.Item.name + " has been added to your inventory!");
            lootSlot.IsLooted();
            itemTooltip.HideToolTip();
        }
        else
        {
            Debug.Log("No room in inventory!");
        }
    }

    private void ShowTooltip(ItemSlot _itemSlot)
    {
        // if the item in the selected equipment slot is an equippable item, display the tooltip!
        sEquippableItem equippableItem = _itemSlot.Item as sEquippableItem;
        if (equippableItem != null)
        {
            itemTooltip.ShowToolTip(equippableItem);
        }
    }

    private void HideTooltip(ItemSlot _itemSlot)
    {
        // if the item in the selected equipment slot is an equippable item, hide the tooltip!
        sEquippableItem equippableItem = _itemSlot.Item as sEquippableItem;
        if (equippableItem != null)
        {
            itemTooltip.HideToolTip();
        }
    }

    private void BeginDrag (ItemSlot _itemSlot)
    {
        if (_itemSlot.Item != null)
        {
            draggedSlot = _itemSlot;
            draggableItem.sprite = _itemSlot.Item.icon;
            draggableItem.transform.position = Input.mousePosition;
            draggableItem.enabled = true;
        }
    }

    private void EndDrag(ItemSlot _itemSlot)
    {
        print("End Drag");
        draggedSlot = null;
        draggableItem.enabled = false;
    }

    private void Drag(ItemSlot _itemSlot)
    {
        if (draggableItem.enabled)
        {
            draggableItem.transform.position = Input.mousePosition;
        }
    }

    private void Drop(ItemSlot _dropItemSlot)
    {
        if (draggedSlot == null) return;

        // Can the slot that we are dropping the item, recieve the item from the slot that started the drag 
        // AND 
        // Can the slot that started the drag recieve the item from the slot that we are dropping the item 

        if (_dropItemSlot.CanRecieveItem(draggedSlot.Item) && draggedSlot.CanRecieveItem(_dropItemSlot.Item))
        {
            sEquippableItem dragItem = draggedSlot.Item as sEquippableItem;
            sEquippableItem dropItem = _dropItemSlot.Item as sEquippableItem;

            // If we are dragging an item out of an equipment slot 
            if (draggedSlot is EquipmentSlot)
            {
                // Unequip the drag item and equip the item in the drop slot
                if (dragItem != null) dragItem.Unequip(this);
                if (dropItem != null) dropItem.Equip(this);
            }
            if (_dropItemSlot is EquipmentSlot)
            {
                // Unequip the drop item and equip the item in the drag item
                if (dragItem != null) dragItem.Equip(this);
                if (dropItem != null) dropItem.Unequip(this);
            }
            
            // Update the stat panel
            //statPanel.UpdateStatValues();

            sItem draggedItem = draggedSlot.Item;
            draggedSlot.Item = _dropItemSlot.Item;

            if (draggedSlot as LootBoxSlot)
            {
                print("slot looted!");
            }

            _dropItemSlot.Item = draggedItem;
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
                    previousItem.Unequip(this);
                    statPanel.UpdateStatValues();
                }
                _item.Equip(this);

                statPanel.UpdateStatValues();
            }
            else
            {
                Debug.Log("Can not equip this item here!");
                inventory.AddItem(_item); // if we can't add the item to an equipment slot, return it to the inventory 
            }
        }
    }

    public void Unequip(sEquippableItem _item)
    {
        if (!inventory.IsFull() && equipPanel.RemoveItem(_item))
        {
            _item.Unequip(this);
            statPanel.UpdateStatValues();
            inventory.AddItem(_item);
            print("unequipped");
        }
        else
            Debug.Log("Can not unequip item! - inventory full!");
    }
}
