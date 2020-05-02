using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour, IItemContainer
{
    [SerializeField] sScrapItem[] startingItems;
    [SerializeField] Transform itemsParent;
    [SerializeField] ItemSlot[] itemSlots;

    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnRightClickEvent;
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    public event Action<sScrapItem> OnItemRightClickedEvent;

    [SerializeField] AudioClip pickUpSound;
    [SerializeField] AudioClip errorSound;
    [SerializeField] AudioClip trashSound;
    [SerializeField] float soundVolume = 0.15f;
    AudioManager AM;

    private void Start()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            // catches the events from ItemSlot.cs and says "the player right clicked this item"... (see InventoryManager.cs for next step)
            itemSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            itemSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            itemSlots[i].OnRightClickEvent += OnRightClickEvent;
            itemSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            itemSlots[i].OnEndDragEvent += OnEndDragEvent;
            itemSlots[i].OnDragEvent += OnDragEvent;
            itemSlots[i].OnDropEvent += OnDropEvent;
        }

        SetStartingItems();

        gameObject.SetActive(!gameObject.activeSelf);

        AM = Toolbox.GetInstance().GetAudioManager();
    }

    private void OnValidate()
    {
        if (itemsParent != null)
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();

        SetStartingItems();
    }

    private void SetStartingItems()
    {
        int i = 0;
        // For each item we have, assign it to an item slot,
        for (; i < startingItems.Length && i < itemSlots.Length; i++)
        {
            if (startingItems[i] != null)
                itemSlots[i].Item = Instantiate(startingItems[i]);
        }

        // for each remaining slot with no item, set slot to null
        for (; i < itemSlots.Length; i++)
        {
            itemSlots[i].Item = null;
        }
    }

    public bool AddItem(sScrapItem item) 
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Item == null)
            {
                AM.PlayConsistentOneShot(pickUpSound, soundVolume);
                itemSlots[i].Item = item;
                return true;
            }
        }

        Debug.Log("No room left in inventory!");
        return false;
    }

    public bool RemoveItem(sScrapItem _item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Item == _item)
            {
                AM.PlayConsistentOneShot(trashSound, soundVolume);
                itemSlots[i].Item = null;
                return true;
            }
        }
        return false;
    }

    public sScrapItem RemoveItem(string _itemID)
    {
        // loop through the item slots 
        for (int i = 0; i < itemSlots.Length; i++)
        {
            sScrapItem item = itemSlots[i].Item;

            // check to see if there is an item in the slot, and if so, does it have the same item ID as the one we are looking for
            if (item != null && item.ID == _itemID)
            {
                // if we find the item we are looking for, remove it from that slot, and return the item reference
                itemSlots[i].Item = null;
                return item;
            }
        }
        // if we didnt find anything, return null
        return null;
    }

    public bool IsFull()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Item == null)
            {
                AM.PlayConsistentOneShot(errorSound, soundVolume);
                return false;
            }
        }
        return true;
    }

    public int ItemCount(string _itemID) // look for an item ID instead of a reference
    {
        int number = 0;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Item.ID == _itemID)
            {
                number++;
            }
        }
        return number;
    }

    public bool ContainsItem(sScrapItem _item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].Item == _item)
            {
                return true;
            }
        }

        AM.PlayConsistentOneShot(errorSound, soundVolume);
        return false;
    }
}
