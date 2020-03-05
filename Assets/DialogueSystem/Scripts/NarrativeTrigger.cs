using UnityEngine;

public class NarrativeTrigger : MonoBehaviour
{
    [Tooltip("Drag in a Scriptable Object conversation.")]
    [SerializeField] sNarrative thisConversation;

    DialogueController conversationCon;
    CircleCollider2D circleCollider;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        conversationCon = Toolbox.GetInstance().GetDialogueManager().GetConversationController();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHandleInteract playerHandleInteract = collision.gameObject.GetComponent<PlayerHandleInteract>();

        if (playerHandleInteract != null)
        {
            conversationCon.GetNewConversation(thisConversation);
            circleCollider.enabled = false;
        }
    }
}
