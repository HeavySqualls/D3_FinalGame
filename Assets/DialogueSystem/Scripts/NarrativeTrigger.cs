using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class NarrativeTrigger : MonoBehaviour
{
    [Tooltip("Drag in a Scriptable Object conversation.")]
    [SerializeField] sNarrative thisConversation;

    [Tooltip("Does this trigger have a CinematicTriggerController attached to it? " +
        "(cinematic will only play at the end of the current narrative event)")]
    public bool hasCinematic = false;
    [SerializeField] CinematicTriggerController cinCon;

    [Tooltip("Is there a camera change that happens during this narrative event?" +  
        "(CamerChangeTriggerController component required)")]
    public bool isCameraChange = false;
    [SerializeField] CameraChangeTriggerController camChangeTrigCon;

    [Tooltip("Will the music and scene background sound be turned down during the cutscene?")]
    public bool isDampenMusic = true;

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
            //pCon.SetPlayerVelocityToZero();
            narrativeController.SetUpNarrativeController(thisConversation, this);
            boxColl.enabled = false;
        }
    }

    public void DisableTrigger()
    {
        if (isCameraChange)
        {
            if (camChangeTrigCon != null)
                camChangeTrigCon.DisableCamera();
            else
                Debug.LogError("Attach a CamerChangeTriggerController component");
        }

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
