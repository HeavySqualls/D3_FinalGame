using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    // Store important information regarding narrative.... stuff...

    ConversationController conversationController;

    public ConversationController GetConversationController()
    {
        return conversationController;
    }

    public void SetConversationController(ConversationController _conCon)
    {
        conversationController = _conCon;
    }
}
