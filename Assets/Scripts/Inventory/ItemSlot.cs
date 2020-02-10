using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image slotImage;
    [SerializeField] ItemToolTip toolTip;

    public event Action<sItem> OnRightClickEvent;

    private sItem _item;

    public sItem Item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item == null)
            {
                slotImage.enabled = false;
            }
            else
            {
                slotImage.sprite = _item.icon;
                slotImage.enabled = true;
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

        if (toolTip == null)
        {
            toolTip = FindObjectOfType<ItemToolTip>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right)
        {
            // If we right click on an item slot...
            if (Item != null && OnRightClickEvent != null)
            {
                // Event says "I've been clicked, and this is my item"... (see Inventory.cs for next step)
                OnRightClickEvent(Item);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_item is sEquippableItem)
        {
            toolTip.ShowToolTip((sEquippableItem)_item);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.HideToolTip();
    }
}
