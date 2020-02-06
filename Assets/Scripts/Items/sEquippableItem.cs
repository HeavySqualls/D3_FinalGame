using UnityEngine;

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
    public int intelligenceBonus;
    public int vitalityBonus;
    [Space]
    public float strengthPecentBonus;
    public float agilityPercentBonus;
    public float intelligencePercentBonus;
    public float vitalityPercentBonus;
    [Space]
    [Header("Equipment Type")]
    public EquipmentType equipType;
}
