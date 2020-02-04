using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandleInteract : MonoBehaviour
{
    public GameObject interactableItem;

    void Start()
    {
        
    }

    void Update()
    {
        Interaction();
    }

    private void Interaction()
    {
        if (interactableItem != null)
        {
            if (interactableItem.GetComponent<>())
            {

            }
        }
    }
}
