using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField] Text itemNameText;
    [SerializeField] Text itemSlotText;
    [SerializeField] Text itemStatsText;

    // Allows us to join several things together in to one string, without taking the penalty for allocating a new string each time
    private StringBuilder sb = new StringBuilder();

    public void ShowToolTip(sEquippableItem _item)
    {
        itemNameText.text = _item.itemName;
        itemSlotText.text = _item.equipType.ToString();
        sb.Length = 0;

        // Stat normal
        AddStat(_item.strengthBonus, "Strength", false);
        AddStat(_item.agilityBonus, "Agility", false);
        AddStat(_item.intellectBonus, "Intellect", false);
        AddStat(_item.vitalityBonus, "Vitality", false);

        // Stat percentages
        AddStat(_item.strengthPercentBonus, "Strength", true);
        AddStat(_item.agilityPercentBonus, "Agility", true);
        AddStat(_item.intellectPercentBonus, "Intellect", true);
        AddStat(_item.vitalityPercentBonus, "Vitality", true);

        itemStatsText.text = sb.ToString();

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }

    private void AddStat(float _value, string _statName, bool _isPercent)
    {
        if (_value != 0) // only add stat if the value is not nothing
        {
            if (sb.Length > 0) // if this is not the first stat, start writing on a new line
                sb.AppendLine();

            if (_value > 0) // if the value is positive, add in a plus sign 
                sb.Append("+");

            if (_isPercent) // if the stat is a percent, multiply it by 100 and put a % sign beside it 
            {
                sb.Append(_value * 100);
                sb.Append("% ");
            }
            else // else just display the stat
            {
                sb.Append(_value);
                sb.Append(" ");
            }

            sb.Append(_statName);
        }
    }
}
