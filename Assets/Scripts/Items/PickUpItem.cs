using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public bool isLootBox;
    public bool isOpen;
    private bool isEmpty;

    public sItem item;

    [SerializeField] PlayerHandleInteract pInteract;
    [SerializeField] Material normalMat;
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
            pInteract.SendPickupItemReferences(this, lootBoxPanel);
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
