using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractInteractable : MonoBehaviour
{
    public abstract void OnInteracted(Collider2D hit, Interact_Base interactable);
}
