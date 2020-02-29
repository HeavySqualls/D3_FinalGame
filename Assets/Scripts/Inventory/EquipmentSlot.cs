
public class EquipmentSlot : ItemSlot
{
    public EquipmentType equipType;

    protected override void OnValidate()
    {
        base.OnValidate();

        gameObject.name = equipType.ToString() + "Slot";
    }

    public override bool CanRecieveItem(sItem _item)
    {
        if (_item == null)
        {
            return true;
        }

        // Check if the item we are trying to equip is an equippable item
        sEquippableItem equippableItem = _item as sEquippableItem;

        // And if it is an equippable item, check if the items equipment type corresponds to the slot equipment type 
        return equippableItem != null && equippableItem.equipType == equipType;
    }
}
