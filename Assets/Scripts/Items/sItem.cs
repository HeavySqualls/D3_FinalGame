using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Objects", menuName ="Items/Basic Item", order = 1)]
public class sItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
}
