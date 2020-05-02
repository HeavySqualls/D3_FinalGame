using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemToolTip : MonoBehaviour
{
    // For Equipment Tooltip (no longer implemented)
    [SerializeField] Text itemNameText;
    [SerializeField] Text itemSlotText;
    [SerializeField] Text itemStatsText;

    // For scrap item Tooltip
    [SerializeField] TextMeshProUGUI scrapItemNameText;
    [SerializeField] TextMeshProUGUI scrapItemDescText;
    [SerializeField] TextMeshProUGUI scrapItemValueText;
    [SerializeField] Image scrapItemIcon;

    // Allows us to join several things together in to one string, without taking the penalty for allocating a new string each time
    private StringBuilder sb = new StringBuilder();

    public void ShowEquipmentToolTip(sEquippableItem _item)
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

    public void ShowScrapToolTip(sScrapItem _item)
    {
        scrapItemNameText.text = _item.itemName;
        scrapItemDescText.text = _item.itemDescription;
        scrapItemValueText.text = _item.scrapValue.ToString();
        scrapItemIcon.sprite = _item.inventoryIcon;

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
