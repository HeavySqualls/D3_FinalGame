using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Objects", menuName = "DialogueSystem/Tutorial", order = 1)]
public class sTutorial : ScriptableObject
{
    public string tutorialID;
    public bool hasBeenPlayed;

    public string tutorialTitle;
    public Sprite tutorialImage;

    [TextArea(2, 6)]
    public string tutorialInfo;
}
