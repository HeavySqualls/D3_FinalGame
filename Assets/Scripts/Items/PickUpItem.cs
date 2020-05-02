using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [Tooltip("Is this object a loot box?")]
    public bool isLootBox;
    [Tooltip("Is this object open?")]
    public bool isOpen;
    [Tooltip("Is this object empty?")]
    public bool isEmpty;

    [Tooltip("What scriptable object item this item be? - NOT FOR LOOT BOX. FOR SINGLE PICK UP ITEMS ONLY!")]
    public sScrapItem item;

    [Tooltip("This field gets auto-assigned and holds the reference to the player while they are in the trigger zone.")]
    [SerializeField] PlayerHandleInteract pInteract;
    [Tooltip("Assign the normal material assigned to the asset.")]
    [SerializeField] Material normalMat;
    [Tooltip("Assign the related highlighed 'Outline' shader for when the player is in the trigger zone.")]
    [SerializeField] Material highlightMat;

    [Space]
    [Header("Audio:")]
    [SerializeField] AudioClip pickUpSound;
    [SerializeField] float pickUpVolume = 0.15f;
    [SerializeField] AudioClip openBoxSound;
    [SerializeField] float openVolume = 0.15f;
    [SerializeField] AudioClip errorSound;
    [SerializeField] float errorVolume = 0.15f;
    AudioManager AM;

    public LootBoxPanel lootBoxPanel;

    SpriteRenderer spriteRenderer;
    CircleCollider2D circleCollider;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = normalMat;
        spriteRenderer.sprite = item.scrapSprite;
        circleCollider = GetComponent<CircleCollider2D>();
        AM = Toolbox.GetInstance().GetAudioManager();
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
            AM.PlayConsistentOneShot(pickUpSound, pickUpVolume);
            spriteRenderer.enabled = false;
            pInteract.UnAssignPickUpItemReferences();
            pInteract = null;
            spriteRenderer.material = normalMat;
            circleCollider.enabled = false;
        }
    }

    public void OpenLootBox()
    {
        AM.PlayConsistentOneShot(openBoxSound, openVolume);
        lootBoxPanel.ViewItems();
        isOpen = true;
        ShowMouseCursor();
    }

    public void CloseLootBox()
    {
        //TODO: Figure out how to play a track backwards
        AM.PlayConsistentOneShot(openBoxSound, openVolume);
        lootBoxPanel.HideItems();
        isOpen = false;
        HideMouseCursor();
    }

    public void QuickLoot()
    {
        if (!lootBoxPanel.CheckIfEmpty())
        {
            AM.PlayConsistentOneShot(pickUpSound, pickUpVolume);
            lootBoxPanel.QuickLoot();
        }
        else
        {
            AM.PlayConsistentOneShot(errorSound, errorVolume);
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
