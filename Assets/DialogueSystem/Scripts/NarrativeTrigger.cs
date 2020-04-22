using UnityEngine;

public class NarrativeTrigger : MonoBehaviour
{
    [Tooltip("Drag in a Scriptable Object conversation.")]
    [SerializeField] sNarrative thisConversation;

    NarrativeController narrativeController;
    BoxCollider2D boxColl;

    private void Start()
    {
        boxColl = GetComponent<BoxCollider2D>();
        narrativeController = Toolbox.GetInstance().GetDialogueSystemManager().GetNarrativeController();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHandleInteract playerHandleInteract = collision.gameObject.GetComponent<PlayerHandleInteract>();

        if (playerHandleInteract != null)
        {
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
}
