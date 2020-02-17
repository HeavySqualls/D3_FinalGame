using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Scriptable Objects", menuName ="Items/Basic Item", order = 1)]
public class sItem : ScriptableObject
{
    [SerializeField] string id;
    public string ID { get { return id; } }

    public string itemName;
    public Sprite icon;

    private void OnValidate()
    {
        string path = AssetDatabase.GetAssetPath(this);
        id = AssetDatabase.AssetPathToGUID(path);
    }
}
