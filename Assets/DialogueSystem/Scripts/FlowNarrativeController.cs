using System.Collections;
using UnityEngine;

public class FlowNarrativeController : MonoBehaviour
{
    [Header("Flow Narrative: ")]
    [Tooltip("Automatically assigned by the trigger when the player enters the zone. Assign specific sFlowNarrative to the triggers only.")]
    public sFlowNarrative FN;

    [Space]
    [Header("Text Variables:")]
    [Tooltip("The time between speech bubbles appearing and disapearing.")]
    [SerializeField] float betweenSentenceTime;
    [Tooltip("The time delay before the speech bubble closes.")]
    [SerializeField] float betweenTextTime;
    [Tooltip("The time between each letter appearing when being typed out.")]
    [SerializeField] float textDelayTime;

    private int activeLineIndex = 0;
    private FlowNarrativeUIController flowNarController;

    private void Awake()
    {
        Toolbox.GetInstance().GetDialogueSystemManager().SetFlowNarrativeController(this);
        flowNarController = GetComponentInChildren<FlowNarrativeUIController>();
    }


    // < ------------------------------------- FLOW NARRATIVE SETUP ------------------------------------- >> //

    public void GetNewFlowNarrative(sFlowNarrative _fn, GameObject _target)
    {
        FN = _fn;
        flowNarController.ShowFNSpeechBubble(_target);
        AdvanceConversation();
    }

    public void AdvanceConversation()
    {
        if (activeLineIndex < FN.lines.Length)
        {
            DisplayNextLine();
            activeLineIndex += 1;
        }
        else
        {
            EndResetController();
        }
    }

    private void DisplayNextLine()
    {
        FlowLine line = FN.lines[activeLineIndex];
        SetFlowNarrativeDialogue(flowNarController, line.text);
    }

    private void SetFlowNarrativeDialogue(FlowNarrativeUIController _fnController, string _text)
    {
        _fnController.Dialogue = _text;

        _fnController.Dialogue = "";
        StopAllCoroutines();
        StartCoroutine(TypeWritterEffect_FN(_text, _fnController));
    }

    private IEnumerator TypeWritterEffect_FN(string _text, FlowNarrativeUIController _fnController)
    {
        yield return new WaitForSeconds(0.3f);

        foreach (char letter in _text.ToCharArray())
        {
            _fnController.Dialogue += letter;
            yield return new WaitForSeconds(textDelayTime);
        }

        yield return new WaitForSeconds(betweenTextTime);

        _fnController.HideFNSpeechBubble();

        yield return new WaitForSeconds(betweenSentenceTime);

        _fnController.ShowSpeechBubble();
        AdvanceConversation();
    }

    public void EndResetController()
    {
        flowNarController.HideFNSpeechBubble();
        flowNarController.RemoveTargetRef();
        FN = null;
        activeLineIndex = 0;
    }
}
