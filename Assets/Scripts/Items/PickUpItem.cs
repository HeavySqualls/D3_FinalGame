using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public sItem item;
    [SerializeField] PlayerHandleInteract pInteract;
    [SerializeField] Material normalMat;
    [SerializeField] Material highlightMat;

    private SpriteRenderer spriteRenderer;
    private bool isEmpty;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = normalMat;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        pInteract = other.gameObject.GetComponent<PlayerHandleInteract>();

        if (pInteract != null && !isEmpty)
        {
            pInteract.AssignPickUpItem(this);
            spriteRenderer.material = highlightMat;
        }      
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (pInteract != null)
        {
            pInteract.UnAssignPickUpItem();
            pInteract = null;
            spriteRenderer.material = normalMat;
        }
    }

    public void OnItemPickedUp()
    {
        spriteRenderer.enabled = false;
        isEmpty = true;
        pInteract.UnAssignPickUpItem();
        pInteract = null;
        spriteRenderer.material = normalMat;
    }
}
