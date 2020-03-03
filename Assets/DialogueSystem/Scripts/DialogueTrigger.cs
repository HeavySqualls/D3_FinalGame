using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] ConversationController conversationCon;
    //TODO: is there simple way for this to automatially find this?

    [SerializeField] sNarrative thisConversation;
    CircleCollider2D circleCollider;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
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
