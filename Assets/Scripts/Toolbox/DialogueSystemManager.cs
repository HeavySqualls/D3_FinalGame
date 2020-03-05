using UnityEngine;

public class DialogueSystemManager : MonoBehaviour
{
    // Store important information regarding narrative.... stuff...


    // < ------------------------------------- NARRATIVE ------------------------------------- >> //

    NarrativeController N_Controller;

    public NarrativeController GetConversationController()
    {
        return N_Controller;
    }

    public void SetNarrativeController(NarrativeController _nCon)
    {
        N_Controller = _nCon;
    }


    // < ------------------------------------- FLOW NARRATIVE ------------------------------------- >> //

    FlowNarrativeController FN_Controller;

    public FlowNarrativeController GetFlowNarrativeController()
    {
        return FN_Controller;
    }

    public void SetFlowNarrativeController(FlowNarrativeController _fnCon)
    {
        FN_Controller = _fnCon;
    }
}
