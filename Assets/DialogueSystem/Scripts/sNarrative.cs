using UnityEngine;


[System.Serializable]
public struct Line
{
    public sCharacter character;

    [TextArea(2, 5)]
    public string text;
}


[CreateAssetMenu(fileName = "Scriptable Objects", menuName = "DialogueSystem/Narrative", order = 1)]
public class sNarrative : ScriptableObject
{
    public string narrativeID;
    public bool hasTutorial;
    public sTutorial tutorial;

    public sCharacter speakerOnTheRight;
    public sCharacter speakerOnTheLeft;

    public Line[] lines;
}
