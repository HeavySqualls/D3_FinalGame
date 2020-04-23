using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class NarrativeTrigger : MonoBehaviour
{
    [Tooltip("Drag in a Scriptable Object conversation.")]
    [SerializeField] sNarrative thisConversation;

    //[Tooltip("Drag in any Timeline cutscenes that will be played during this narrative event.")]
    //[SerializeField] PlayableDirector[] timeLines;

    [Tooltip("Does this trigger have a CinematicTriggerController attached to it? " +
        "(cinematic will only play at the end of the current narrative event)")]
    public bool hasCinematic = false;
    [SerializeField] CinematicTriggerController cinCon;

    NarrativeController narrativeController;
    BoxCollider2D boxColl;

    private void Start()
    {
        boxColl = GetComponent<BoxCollider2D>();
        narrativeController = Toolbox.GetInstance().GetDialogueSystemManager().GetNarrativeController();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //PlayerHandleInteract playerHandleInteract = collision.gameObject.GetComponent<PlayerHandleInteract>();
        PlayerController pCon = collision.gameObject.GetComponent<PlayerController>();

        if (pCon != null)
        {
            pCon.SetPlayerVelocityToZero();
            narrativeController.SetUpNarrativeController(thisConversation, this);
            boxColl.enabled = false;
        }
    }

    public void DisableTrigger()
    {
        boxColl.enabled = false;
    }

    public void PlayNarrative()
    {
        narrativeController.SetUpNarrativeController(thisConversation, this);
    }

    public void PlayPostNarrativeCinematic()
    {
        if (cinCon != null)
        {
            cinCon.PlayCinematic();
        }
    }
}
