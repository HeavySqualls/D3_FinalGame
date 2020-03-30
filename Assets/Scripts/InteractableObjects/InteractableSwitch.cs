using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSwitch : MonoBehaviour
{
    [Space]
    [Header("Type of switch:")]
    [Tooltip("Does this switch control a door?")]
    public bool isDoorSwitch = false;
    [Tooltip("Does this switch control a toxic spill pipe?")]
    public bool isSpillSwitch = false;

    [Space]
    [Header("Object this switch controls:")]
    [SerializeField] BlockedDoor door;
    [Tooltip("Assign the relative toxic spill pipes to actuate with this switch.")]
    [SerializeField] ToxicSpill[] spillPipes;

    [Space]
    [Header("Material References:")]
    [Tooltip("Assign the normal material assigned to the asset.")]
    [SerializeField] Material normalMat;
    [Tooltip("Assign the related highlighed 'Outline' shader for when the player is in the trigger zone.")]
    [SerializeField] Material highlightMat;
    [Tooltip("Assign the relative door to actuate with this switch.")]

    bool isOpen;
    PlayerHandleInteract pInteract;
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

    public void ActuateSwitch()
    {
        if (isDoorSwitch)
        {
            if (!door.isInputBlocked)
            {
                print("door switch is actuated!");
                door.OpenCloseDoor();
                isOpen = !isOpen;
            }
            else
                Debug.LogWarning("Door is currently in use... wait for movement to finish.");
        }

        if (isSpillSwitch)
        {
            print("spill switch is actuated!");

            isOpen = !isOpen;

            foreach (ToxicSpill ts in spillPipes)
            {
                ts.TurnOffSpillPipe();
            }
        }

        animator.SetBool("isOpen", isOpen);
    }
}
