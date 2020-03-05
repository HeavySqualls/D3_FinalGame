using UnityEngine;

[System.Serializable]

public struct FlowLine
{
    [TextArea(2, 5)]
    public string text;
}

[CreateAssetMenu(fileName = "Scriptable Objects", menuName = "DialogueSystem/FlowNarrative", order = 1)]
public class sFlowNarrative : ScriptableObject
{
    [Tooltip("Input the desired ID number of this flow narrative.")]
    public string flowNarrativeID;

    [Tooltip("Input the number of lines desired and add text.")]
    public FlowLine[] lines;

}
