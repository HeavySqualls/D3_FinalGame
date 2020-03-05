using UnityEngine;

public class NarrativeTrigger : MonoBehaviour
{
    [Tooltip("Drag in a Scriptable Object conversation.")]
    [SerializeField] sNarrative thisConversation;

    NarrativeController conversationCon;
    CircleCollider2D circleCollider;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        conversationCon = Toolbox.GetInstance().GetDialogueSystemManager().GetConversationController();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHandleInteract playerHandleInteract = collision.gameObject.GetComponent<PlayerHandleInteract>();

        if (playerHandleInteract != null)
        {
            conversationCon.GetNewNarrative(thisConversation);
            circleCollider.enabled = false;
        }
    }
}
