using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Line
{
    [Tooltip("Which character will be speaking this line? Drag in the relative sCharacter.")]
    public sCharacter character;

    [TextArea(2, 5)]
    public string text;
}


[CreateAssetMenu(fileName = "Scriptable Objects", menuName = "DialogueSystem/Narrative", order = 1)]
public class sNarrative : ScriptableObject
{
    [Header("Narrative Settings:")]
    public string narrativeID;
    [Tooltip("Does this Narrative only have one speaker?")]
    public bool isMonologue;
    [Tooltip("Does this Narrative contain a tutorial?")]
    public bool hasTutorial;
    [Tooltip("Does this Narrative have a delay before beginning?")]
    public bool hasDelay;
    [Tooltip("If this Narrative has a delay, how long will that delay be?")]
    public float narrativeStartDelayTime;
    [Tooltip("Drag in the relative sTutorial for this narrative.")]
    public sTutorial[] tutorials;
    [Tooltip("After which line of dialogue will this tutorial be presented?")]
    public int[] tutorialLines;


    [Tooltip("Who will be the speaker on the left?")]
    public sCharacter speakerOnTheLeft;
    [Tooltip("Who will be the speaker on the right?")]
    public sCharacter speakerOnTheRight;

    [Tooltip("Input the number of lines desired and add text.")]
    public Line[] lines;
}
