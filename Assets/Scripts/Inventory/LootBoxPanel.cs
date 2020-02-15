using System;
using UnityEngine;

public class LootBoxPanel : MonoBehaviour
{
    [SerializeField] Transform lootBoxSlotsParent;
    [SerializeField] LootBoxSlot[] lootBoxSlots;
    [SerializeField] GameObject lootBoxPanel;

    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnRightClickEvent; // Item has been selected
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    public event Action<sItem> OnItemRightClickedEvent;

    private void OnValidate()
    {
        lootBoxSlots = lootBoxSlotsParent.GetComponentsInChildren<LootBoxSlot>();
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
    }

    public bool ViewItems(sItem item)
    {

        lootBoxPanel.SetActive(true);

        for (int i = 0; i < lootBoxSlots.Length; i++)
        {
            if (lootBoxSlots[i].Item == null && !lootBoxSlots[i].isLooted)
            {
                lootBoxSlots[i].Item = item;
                return true;
            }
        }

        Debug.Log("No room left in the chest!");
        return false;
    }

    public void HideItems()
    {
        // set all loot box game objects to false
        lootBoxPanel.SetActive(false);
    }
}
