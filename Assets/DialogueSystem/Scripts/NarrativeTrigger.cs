using UnityEngine;

public class NarrativeTrigger : MonoBehaviour
{
    [Tooltip("Drag in a Scriptable Object conversation.")]
    [SerializeField] sNarrative thisConversation;

    NarrativeController conversationCon;
    BoxCollider2D boxColl;

    private void Start()
    {
        boxColl = GetComponent<BoxCollider2D>();
        conversationCon = Toolbox.GetInstance().GetDialogueSystemManager().GetNarrativeController();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHandleInteract playerHandleInteract = collision.gameObject.GetComponent<PlayerHandleInteract>();

        if (playerHandleInteract != null)
        {
            conversationCon.GetNewNarrative(thisConversation);
            boxColl.enabled = false;
        }
    }
}
