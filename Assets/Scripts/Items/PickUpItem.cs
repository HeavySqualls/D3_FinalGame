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
    public bool isEmpty;

    [Tooltip("What scriptable object item this item be? - NOT FOR LOOT BOX. FOR SINGLE PIKC UP ITEMS ONLY!")]
    public sItem item;

    [Tooltip("This field gets auto-assigned and holds the reference to the player while they are in the trigger zone.")]
    [SerializeField] PlayerHandleInteract pInteract;
    [Tooltip("Assign the normal material assigned to the asset.")]
    [SerializeField] Material normalMat;
    [Tooltip("Assign the related highlighed 'Outline' shader for when the player is in the trigger zone.")]
    [SerializeField] Material highlightMat;


    [SerializeField] AudioClip pickUpSound;
    [SerializeField] AudioClip openBoxSound;
    [SerializeField] AudioClip errorSound;
    [SerializeField] float pickUpVolume = 0.15f;

    public LootBoxPanel lootBoxPanel;

    AudioSource audioSource;
    SpriteRenderer spriteRenderer;
    CircleCollider2D circleCollider;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = normalMat;
        circleCollider = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = pickUpVolume;
        //Toolbox.GetInstance().GetLevelManager().AddPickups(this);
        //lootBoxPanel = GetComponentInChildren<LootBoxPanel>();
    }

    //private void OnDestroy()
    //{
    //    Toolbox.GetInstance().GetLevelManager().RemovePickups(this);
    //}

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
            if (isLootBox && isOpen)
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
            audioSource.PlayOneShot(pickUpSound);
            spriteRenderer.enabled = false;
            pInteract.UnAssignPickUpItemReferences();
            pInteract = null;
            spriteRenderer.material = normalMat;
            circleCollider.enabled = false;
        }
    }

    public void OpenLootBox()
    {
        audioSource.PlayOneShot(openBoxSound);
        lootBoxPanel.ViewItems();
        isOpen = true;
        ShowMouseCursor();
    }

    public void CloseLootBox()
    {
        //TODO: Figure out how to play a track backwards
        audioSource.PlayOneShot(openBoxSound);
        lootBoxPanel.HideItems();
        isOpen = false;
        HideMouseCursor();
    }

    public void QuickLoot()
    {
        if (!lootBoxPanel.CheckIfEmpty())
        {
            audioSource.PlayOneShot(pickUpSound);
            lootBoxPanel.QuickLoot();
        }
        else
        {
            audioSource.PlayOneShot(errorSound);
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
