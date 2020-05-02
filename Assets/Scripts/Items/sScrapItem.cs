using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Objects", menuName ="Items/Scrap Item", order = 1)]
public class sScrapItem : ScriptableObject
{
    [SerializeField] string id;
    public string ID { get { return id; } }

    public string itemName;
    public string itemCategory;

    public Sprite scrapSprite;
    public Sprite inventoryIcon;

    [TextArea(2, 5)]
    public string itemDescription;

    public float scrapValue;

    public string text;

}
