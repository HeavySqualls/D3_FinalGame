using System.Collections;
using System;
using UnityEngine;

public class EquipmentPanel : MonoBehaviour
{
    [SerializeField] Transform equipmentSlotsParent;
    [SerializeField] EquipmentSlot[] equipmentSlots;

    public event Action<sItem> OnItemRightClickedEvent;

    private void Start()
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            // catches the event from EquipSlot.cs and says "the player right clicked this item"... (see InventoryManager.cs for next step)
            equipmentSlots[i].OnRightClickEvent += OnItemRightClickedEvent;
        }
    }

    private void OnValidate()
    {
        equipmentSlots = equipmentSlotsParent.GetComponentsInChildren<EquipmentSlot>();
    }

    public bool AddItem(sEquippableItem _item, out sEquippableItem _previousItem) // out sends a reference out of the method
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].equipType == _item.equipType)
            {
                _previousItem = (sEquippableItem)equipmentSlots[i].Item;
                equipmentSlots[i].Item = _item;
                return true;
            }
        }
        Debug.Log("Item can not be equiped!");
        _previousItem = null;
        return false;
    }

    public bool RemoveItem(sEquippableItem _item)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (equipmentSlots[i].equipType == _item.equipType)
            {
                equipmentSlots[i].Item = null;
                return true;
            }
        }
        Debug.Log("No item to be removed!");
        return false;
    }
}
