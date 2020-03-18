using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Objects", menuName = "DialogueSystem/Tutorial", order = 1)]
public class sTutorial : ScriptableObject
{
    [Tooltip("Input the desired sTutorial ID number.")]
    public string tutorialID;

    [Tooltip("Used to track if this scriptable object has been seen or not.")]
    public bool hasBeenPlayed;

    [Tooltip("What is the name of this tutorial?")]
    public string tutorialTitle;

    [Tooltip("Add in the relative quote for this tutorial.")]
    [TextArea(2, 6)]
    public string tutorialQuote;

    [Tooltip("Input the number of lines desired and add text.")]
    [TextArea(2, 6)]
    public string tutorialInfo;

    [Tooltip("Add in a relative image for the tutorial.")]
    public Sprite tutorialImage;
}
