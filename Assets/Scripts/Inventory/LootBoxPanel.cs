﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LootBoxPanel : MonoBehaviour
{
    [Tooltip("How many slots will this loot box have?")]
    [SerializeField] LootBoxSlot[] lootBoxSlots;
    [Tooltip("What scriptable object items will be inside this loot box? - Must be the same number as loot box slots!")]
    [SerializeField] sItem[] startingItems;
    GameObject lootBoxPanel;
    Transform lootBoxSlotsParent;

    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnRightClickEvent; // Item has been selected
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    public event Action<sItem> OnItemRightClickedEvent;

    [SerializeField] InventoryManager inventoryManager;

    private void OnValidate()
    {
        lootBoxSlots = lootBoxSlotsParent.GetComponentsInChildren<LootBoxSlot>();
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    private void Awake()
    {
        for (int i = 0; i < lootBoxSlots.Length; i++)
        {
            // catches the events from EquipSlot.cs and says "the player right clicked this item"... (see InventoryManager.cs for next step)
            lootBoxSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            lootBoxSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            lootBoxSlots[i].OnRightClickEvent += OnRightClickEvent;
            lootBoxSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            lootBoxSlots[i].OnEndDragEvent += OnEndDragEvent;
            lootBoxSlots[i].OnDragEvent += OnDragEvent;
            lootBoxSlots[i].OnDropEvent += OnDropEvent;
        }       
    }

    private void Start()
    {
        lootBoxPanel.SetActive(false);
        SetStartingItems();
    }

    private void SetStartingItems()
    {
        int i = 0;
        // For each item we have, assign it to an item slot,
        for (; i < startingItems.Length && i < lootBoxSlots.Length; i++)
        {
            lootBoxSlots[i].Item = startingItems[i];
        }

        // for each remaining slot with no item, set slot to null
        for (; i < lootBoxSlots.Length; i++)
        {
            lootBoxSlots[i].Item = null;
        }

        if (startingItems.Length > lootBoxSlots.Length)
        {
            Debug.LogWarning("More items assigned to loot box " + gameObject.GetComponentInParent<SpriteRenderer>().name + " than there are slots! Not all items will appear.");
        }
    }

    public void QuickLoot()
    {
        for (int i = 0; i < lootBoxSlots.Length; i++)
        {
            if (lootBoxSlots[i].Item != null)
            {
                inventoryManager.QuickLoot(lootBoxSlots[i]);
            }
        }
    }

    // Trying to assign an array of items to an array of item slots in a loot crate UI:
    public void ViewItems() // The array of items passed in to the loot crate UI 
    {
        lootBoxPanel.SetActive(true); // Make the UI visible to the player
    }

    public void HideItems()
    {
        // set all loot box game objects to false
        lootBoxPanel.SetActive(false);
    }
}
