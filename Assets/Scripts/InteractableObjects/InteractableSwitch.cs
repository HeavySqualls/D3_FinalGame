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
    [Header("Additional Objects this switch controls:")]
    [Tooltip("Assign any relative game objects you want to disable with this switch.")]
    [SerializeField] GameObject[] additionalGameObjects;
    [Tooltip("Does this trigger have a CinematicTriggerController attached to it? " +
    "(cinematic will only play at the end of the current narrative event)")]
    public bool hasCinematic = false;
    [SerializeField] CinematicTriggerController cinCon;

    [Space]
    [Header("Material References:")]
    [Tooltip("Assign the normal material assigned to the asset.")]
    [SerializeField] Material normalMat;
    [Tooltip("Assign the related highlighed 'Outline' shader for when the player is in the trigger zone.")]
    [SerializeField] Material highlightMat;
    [Tooltip("Assign the relative door to actuate with this switch.")]

    [Space]
    [Header("Audio:")]
    [SerializeField] AudioClip valveOpenSound;
    [SerializeField] float valveOpenVolume;
    [SerializeField] AudioClip switchOpenSound;
    [SerializeField] float switchOpenVolume;
    AudioManager AM;

    bool isOpen;
    CircleCollider2D trigger;
    PlayerHandleInteract pInteract;
    SpriteRenderer spriteRenderer;
    Animator animator;

    private void Start()
    {
        trigger = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = normalMat;
        AM = Toolbox.GetInstance().GetAudioManager();
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
                AM.PlayConsistentOneShot(switchOpenSound, switchOpenVolume);
                door.OpenCloseDoor();
                isOpen = true;

                if (hasCinematic && cinCon != null)
                {
                    cinCon.PlayCinematic();
                }

                trigger.enabled = false;
            }
            else
                Debug.LogWarning("Door is currently in use... wait for movement to finish.");
        }

        if (isSpillSwitch)
        {
            print("spill switch is actuated!");
            AM.PlayConsistentOneShot(valveOpenSound, valveOpenVolume);

            foreach (ToxicSpill ts in spillPipes)
            {
                ts.TurnOffSpillPipe();
            }

            isOpen = true;
        }

        if (additionalGameObjects.Length > 0)
        {
            foreach(GameObject go in additionalGameObjects)
            {
                go.SetActive(false);
                Debug.Log(gameObject.name + " has disabled " + go.name);
            }
        }

        animator.SetBool("isOpen", isOpen);
    }
}
