using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockedDoor : MonoBehaviour
{
    [Tooltip("This field gets auto-assigned and holds the reference to the player while they are in the trigger zone.")]
    [SerializeField] PlayerHandleInteract pInteract;
    [Tooltip("Assign the normal material assigned to the asset.")]
    [SerializeField] Material normalMat;
    [Tooltip("Assign the related highlighed 'Outline' shader for when the player is in the trigger zone.")]
    [SerializeField] Material highlightMat;

    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = normalMat;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        pInteract = other.gameObject.GetComponent<PlayerHandleInteract>();

        if (pInteract != null)
        {
            spriteRenderer.material = highlightMat;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (pInteract != null)
        {
            spriteRenderer.material = normalMat;
        }
    }
}
