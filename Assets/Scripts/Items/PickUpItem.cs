using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [Tooltip("Is this object a loot box?")]
    public bool isLootBox;
    [Tooltip("Is this object open?")]
    public bool isOpen;
    [Tooltip("Is this object empty?")]
    private bool isEmpty;

    [Tooltip("What scriptable object item will be inside this loot box?")]
    public sItem item;

    [Tooltip("This field gets auto-assigned and holds the reference to the player while they are in the trigger zone.")]
    [SerializeField] PlayerHandleInteract pInteract;
    [Tooltip("Assign the normal material assigned to the asset.")]
    [SerializeField] Material normalMat;
    [Tooltip("Assign the related highlighed 'Outline' shader for when the player is in the trigger zone.")]
    [SerializeField] Material highlightMat;

    SpriteRenderer spriteRenderer;
    LootBoxPanel lootBoxPanel;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = normalMat;
        lootBoxPanel = GetComponentInChildren<LootBoxPanel>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        pInteract = other.gameObject.GetComponent<PlayerHandleInteract>();

        if (pInteract != null && !isEmpty)
        {
            pInteract.RecievePickupItemReferences(this, lootBoxPanel);
            spriteRenderer.material = highlightMat;
        }      
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (pInteract != null)
        {
            CloseLootBox();
            pInteract.UnAssignPickUpItemReferences();
            pInteract = null;
            spriteRenderer.material = normalMat;
        }
    }

    public void OnItemPickedUp()
    {
        if (!isLootBox)
        {
            spriteRenderer.enabled = false;
            pInteract.UnAssignPickUpItemReferences();
            pInteract = null;
            spriteRenderer.material = normalMat;
            Destroy(gameObject);
        }
    }

    public void OpenCloseLootBox()
    {
        if (!isOpen)
        {
            lootBoxPanel.ViewItems(item);
            isOpen = true;
            ShowMouseCursor();
        }
        else
        {
            lootBoxPanel.HideItems();
            isOpen = false;
            HideMouseCursor();
        }
    }

    public void CloseLootBox()
    {
        if (isOpen)
        {
            lootBoxPanel.HideItems();
            isOpen = false;
        }
    }

    public void ShowMouseCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideMouseCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
