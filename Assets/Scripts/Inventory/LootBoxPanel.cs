using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LootBoxPanel : MonoBehaviour
{
    [Tooltip("How many slots will this loot box have?")]
    [SerializeField] LootBoxSlot[] lootBoxSlots = new LootBoxSlot[4];
    [Tooltip("What scriptable object items will be inside this loot box? - Must be the same number as loot box slots!")]
    [SerializeField] sItem[] startingItems = new sItem[4];
    GameObject lootBoxPanel;
    [SerializeField] Transform lootBoxSlotsParent;

    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnRightClickEvent; // Item has been selected
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;
    public event Action<sItem> OnItemRightClickedEvent;

    InventoryManager inventoryManager;

    private void OnValidate()
    {
        lootBoxPanel = GetComponent<LootBoxPanel>().gameObject;
        lootBoxSlots = lootBoxSlotsParent.GetComponentsInChildren<LootBoxSlot>();
        inventoryManager = Toolbox.GetInstance().GetPlayerManager().GetInventoryManager();
    }


    private void Awake()
    {
        OnValidate();      
    }

    private void Start()
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

        lootBoxPanel.SetActive(false);
        SetStartingItems();
    }

    private void SetStartingItems()
    {
        int i = 0;
        // For each item we have, assign it to an item slot,
        for (; i < startingItems.Length && i < lootBoxSlots.Length; i++)
        {
            if (startingItems[i] != null)
            {
                lootBoxSlots[i].Item = Instantiate(startingItems[i]);
            }
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
