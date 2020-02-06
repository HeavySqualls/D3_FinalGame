using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<sItem> items;
    [SerializeField] Transform itemsParent;
    [SerializeField] ItemSlot[] itemSlots;

    public event Action<sItem> OnItemRightClickedEvent;

    private void Start()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            // catches the event from ItemSlot.cs and says "the player right clicked this item"... (see InventoryManager.cs for next step)
            itemSlots[i].OnRightClickEvent += OnItemRightClickedEvent;
        }
    }

    private void OnValidate()
    {
        if (itemsParent != null)
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();

        RefreshUI();
    }

    private void RefreshUI()
    {
        int i = 0;
        // For each item we have, assign it to an item slot,
        for (; i < items.Count && i < itemSlots.Length; i++)
        {
            itemSlots[i].Item = items[i];
        }

        // for each remaining slot with no item, set slot to null
        for (; i < itemSlots.Length; i++)
        {
            itemSlots[i].Item = null;
        }
    }

    public bool AddItem(sItem item) 
    {
        if (IsFull())
        {
            return false;
        }

        items.Add(item);
        RefreshUI();
        return true;
    }

    public bool RemoveItem(sItem item)
    {
        if (items.Remove(item)) // if we are able to remove the item from the list...
        {
            RefreshUI();
            return true;
        }
        return false;
    }

    public bool IsFull()
    {
        return items.Count >= itemSlots.Length;
    }
}
