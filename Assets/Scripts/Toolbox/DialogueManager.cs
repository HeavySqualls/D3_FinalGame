using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    // Store important information regarding narrative.... stuff...

    DialogueController conversationController;

    public DialogueController GetConversationController()
    {
        return conversationController;
    }

    public void SetConversationController(DialogueController _conCon)
    {
        conversationController = _conCon;
    }
}
