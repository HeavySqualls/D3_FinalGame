using UnityEngine;
using Kryz.CharacterStats;

public enum EquipmentType
{
    Head, 
    Chest, 
    Hand1,
    Hand2,
    Boots, 
}

[CreateAssetMenu(fileName = "Scriptable Objects", menuName = "Items/Equippable Item", order = 1)]
public class sEquippableItem : sItem
{
    [Header("Stat Bonuses:")]
    public int strengthBonus;
    public int agilityBonus;
    public int intellectBonus;
    public int vitalityBonus;
    [Space]
    public float strengthPercentBonus;
    public float agilityPercentBonus;
    public float intellectPercentBonus;
    public float vitalityPercentBonus;
    [Space]
    [Header("Equipment Type")]
    public EquipmentType equipType;

    public void Equip(InventoryManager _inventoryMan)
    {
        if (strengthBonus != 0)
            _inventoryMan.strength.AddModifier(new StatModifier(strengthBonus, StatModType.Flat, this));
        if (agilityBonus != 0)
            _inventoryMan.agility.AddModifier(new StatModifier(agilityBonus, StatModType.Flat, this));
        if(intellectBonus != 0)
            _inventoryMan.intellect.AddModifier(new StatModifier(intellectBonus, StatModType.Flat, this));
        if(vitalityBonus != 0)
            _inventoryMan.vitality.AddModifier(new StatModifier(vitalityBonus, StatModType.Flat, this));

        if(strengthPercentBonus != 0)
            _inventoryMan.strength.AddModifier(new StatModifier(strengthPercentBonus, StatModType.PercentMult, this));
        if(agilityPercentBonus != 0)
            _inventoryMan.agility.AddModifier(new StatModifier(agilityPercentBonus, StatModType.PercentMult, this));
        if(intellectPercentBonus != 0)
            _inventoryMan.intellect.AddModifier(new StatModifier(intellectPercentBonus, StatModType.PercentMult, this));
        if(vitalityPercentBonus != 0)
            _inventoryMan.vitality.AddModifier(new StatModifier(vitalityPercentBonus, StatModType.PercentMult, this));
    }

    public void Unequip(InventoryManager _inventoryMan)
    {
        _inventoryMan.strength.RemoveAllModifiersFromSource(this);
        _inventoryMan.agility.RemoveAllModifiersFromSource(this);
        _inventoryMan.intellect.RemoveAllModifiersFromSource(this);
        _inventoryMan.vitality.RemoveAllModifiersFromSource(this);
    }
}
