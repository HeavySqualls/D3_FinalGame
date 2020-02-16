using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSwitch : MonoBehaviour
{
    [Tooltip("Is this door open?")]
    public bool isOpen;

    [Tooltip("This field gets auto-assigned and holds the reference to the player while they are in the trigger zone.")]
    [SerializeField] PlayerHandleInteract pInteract;
    [Tooltip("Assign the normal material assigned to the asset.")]
    [SerializeField] Material normalMat;
    [Tooltip("Assign the related highlighed 'Outline' shader for when the player is in the trigger zone.")]
    [SerializeField] Material highlightMat;
    [Tooltip("Assign the relative door to actuate with this switch.")]
    [SerializeField] BlockedDoor door;

    SpriteRenderer spriteRenderer;
    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = normalMat;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        pInteract = other.gameObject.GetComponent<PlayerHandleInteract>();

        if (pInteract != null)
        {
            pInteract.RecieveSwitchReference(this);
            spriteRenderer.material = highlightMat;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (pInteract != null)
        {
            pInteract.UnAssignSwitchReference();
            pInteract = null;
            spriteRenderer.material = normalMat;
        }
    }

    public void OpenCloseDoor()
    {
        if (!door.isInputBlocked)
        {
            print("door is actuated!");
            door.OpenCloseDoor();
            isOpen = !isOpen;
            animator.SetBool("isOpen", isOpen);
        }
        else
            Debug.LogWarning("Door is currently in use... wait for movement to finish.");
    }
}
