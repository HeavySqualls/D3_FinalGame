using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] Image slotImage;

    public event Action<ItemSlot> OnPointerEnterEvent;
    public event Action<ItemSlot> OnPointerExitEvent;
    public event Action<ItemSlot> OnRightClickEvent; // Item has been selected
    public event Action<ItemSlot> OnBeginDragEvent;
    public event Action<ItemSlot> OnEndDragEvent;
    public event Action<ItemSlot> OnDragEvent;
    public event Action<ItemSlot> OnDropEvent;

    private Color enabledColor = Color.white;
    private Color disabledColor = new Color (1, 1, 1, 0);

    private sItem _item;

    public sItem Item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item == null)
            {
                slotImage.color = disabledColor;
            }
            else
            {
                slotImage.sprite = _item.icon;
                slotImage.color = enabledColor;
            }
        }
    }

    protected virtual void OnValidate() // triggers when the script is loaded or a value is changed in the inspector
    {
        if (slotImage == null)
        {
            Image[] images = GetComponentsInChildren<Image>();

            foreach (Image image in images)
            {
                if (image.gameObject.name == "SlotImage")
                {
                    slotImage = image;
                    return;
                }
            }
        }
    }

    public virtual bool CanRecieveItem(sItem _item)
    {
        return true; // tells us whether we can put this item inside a slot

        // As this is in the inventory slot, it will always return true as there are no current restrictions on what items 
        // we can hold in our inventory. 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            // If we right click on an item slot...
            if (OnRightClickEvent != null)
            {
                // Event says "I've been clicked, and this is my item"... (see Inventory.cs for next step)
                OnRightClickEvent(this);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnPointerEnterEvent != null)
        {
            OnPointerEnterEvent(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnPointerExitEvent != null)
        {
            OnPointerExitEvent(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (OnBeginDragEvent != null)
        {
            OnBeginDragEvent(this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragEvent != null)
        {
            OnDragEvent(this);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (OnDragEvent != null)
        {
            OnEndDragEvent(this);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (OnDropEvent != null)
        {
            OnDropEvent(this);
        }
    }
}
