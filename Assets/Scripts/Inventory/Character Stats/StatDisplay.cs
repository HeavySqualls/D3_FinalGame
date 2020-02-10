using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Kryz.CharacterStats;

public class StatDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CharacterStat _stat;
    public CharacterStat Stat {
        get { return _stat; }
        set {
            _stat = value;
            UpdateStatValue();
        } 
    }

    private string _name;
    public string Name {
        get { return _name; }
        set { 
            _name = value;
            nameText.text = _name;
        } 
    }

    [SerializeField] Text nameText;
    [SerializeField] Text valueText;

    [SerializeField] StatToolTip toolTip;

    private void OnValidate()
    {
        Text[] texts = GetComponentsInChildren<Text>();
        nameText = texts[0];
        valueText = texts[1];

        if (toolTip == null)
        {
            toolTip = FindObjectOfType<StatToolTip>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.ShowToolTip(Stat, Name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.HideToolTip();
    }

    public void UpdateStatValue()
    {
        valueText.text = _stat.Value.ToString();
    }
}
