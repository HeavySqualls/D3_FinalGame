using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Kryz.CharacterStats;

public class StatToolTip : MonoBehaviour
{
    [SerializeField] Text statNameText;
    [SerializeField] Text statModifiersLabelText;
    [SerializeField] Text statModifiersText;

    // Allows us to join several things together in to one string, without taking the penalty for allocating a new string each time
    private StringBuilder sb = new StringBuilder();

    public void ShowToolTip(CharacterStat _stat, string _statName)
    {
        statNameText.text = GetStatTopText(_stat, _statName);

        statModifiersText.text = GetStatModifiersText(_stat);

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }

    private string GetStatTopText(CharacterStat _stat, string _statName)
    {
        sb.Length = 0;
        sb.Append(_statName);
        sb.Append(" ");
        sb.Append(_stat.Value);

        // If the stat has not changed, do not show parenthesis and different values
        if (_stat.Value != _stat.BaseValue)
        {
            sb.Append(" (");
            sb.Append(_stat.BaseValue);

            if (_stat.Value > _stat.BaseValue)
                sb.Append("+");

            sb.Append(System.Math.Round(_stat.Value - _stat.BaseValue, 2)); // round number to the second decimal
            sb.Append(")");
        }

        return sb.ToString();
    }

    private string GetStatModifiersText(CharacterStat _stat)
    {
        sb.Length = 0;

        foreach (StatModifier mod in _stat.StatModifiers)
        {
            if (sb.Length > 0)
                sb.AppendLine();

            if (mod.Value > 0)
                sb.Append("+");

            if (mod.Type == StatModType.Flat)
            {
                sb.Append(mod.Value);
            }
            else
            {
                sb.Append(mod.Value * 100);
                sb.Append("%");
            }
            

            // Access the source variable
            // "as" keyword checks if the source variable is of type sEquippableItem,
            // if source IS an equippable item, convert it and assign it to the item variable,
            // if source is NOT an equippable item, it will assign a null to the item variable    
            sEquippableItem item = mod.Source as sEquippableItem;

            if (item != null) // if item is of type sEquippableItem
            {
                sb.Append(" ");
                sb.Append(item.itemName);
            }
            else
            {
                Debug.LogError("Modifier is not an equippable item!");
            }
        }

        return sb.ToString();
    }
}
