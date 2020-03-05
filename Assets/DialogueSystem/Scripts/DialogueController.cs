using System.Collections;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public sNarrative conversation;

    public GameObject speakerLeft;
    public GameObject speakerRight;
    public TutorialController tutorialController;

    [SerializeField] float textDelayTime;

    private bool hasDisplayedTutorial = false;

    private SpeakerUIController speakerUILeft;
    private SpeakerUIController speakerUIRight;

    private int activeLineIndex = 0;
    private PlayerController pCon;

    private void Awake()
    {
        Toolbox.GetInstance().GetDialogueManager().SetConversationController(this);
    }

    private void Start()
    {
        pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && conversation != null && !tutorialController.isOpen)
        {       
            AdvanceConversation();
        }
    }

    public void GetNewConversation (sNarrative _convo)
    {
        conversation = _convo;
        StartConversation();
    }

    private void StartConversation()
    {
        speakerUILeft = speakerLeft.GetComponent<SpeakerUIController>();
        speakerUIRight = speakerRight.GetComponent<SpeakerUIController>();

        speakerUILeft.Speaker = conversation.speakerOnTheLeft;
        speakerUIRight.Speaker = conversation.speakerOnTheRight;

        speakerUILeft.ShowSprite();
        speakerUIRight.ShowSprite();
        AdvanceConversation();
        pCon.canMove = false;
    }

    // If the current line that we are interested in is less lines that the number of lines in the conversation, 
    // display it and increase the line index. Otherwise end the conversation. 
    public void AdvanceConversation()
    {
        if (activeLineIndex < conversation.lines.Length)
        {
            if (conversation.hasTutorial && conversation.tutorialLine == activeLineIndex && !hasDisplayedTutorial)
            {
                tutorialController.DisplayTutorial(conversation.tutorial);
                speakerUIRight.Hide();
                speakerUILeft.Hide();
                hasDisplayedTutorial = true;
            }
            else
            {
                DisplayNextLine();
                activeLineIndex += 1;
            }
        }
        else
        {
            EndResetController();
        }
    }

    // Seach the lines in the conversation and return the active one, get the character and set the active versus
    // inactive by providing the correct characters in the correct order to SetDialogue, which tracks the active
    // and inactive speaker, and displays the lines accordingly.
    private void DisplayNextLine()
    {
        Line line = conversation.lines[activeLineIndex];
        sCharacter character = line.character;

        if (speakerUILeft.SpeakerIs(character))
        {
            SetDialogue(speakerUILeft, speakerUIRight, line.text);
        }
        else
        {
            SetDialogue(speakerUIRight, speakerUILeft, line.text);
        }
    }

    private void SetDialogue(SpeakerUIController _activeSpeakerUI, SpeakerUIController _inactiveSpeakerUI, string _text)
    {
        _activeSpeakerUI.Dialogue = _text;
        _activeSpeakerUI.Show();
        _inactiveSpeakerUI.Hide();

        _activeSpeakerUI.Dialogue = "";
        StopAllCoroutines();
        StartCoroutine(TypeWritterEffect(_text, _activeSpeakerUI));
    }

    private IEnumerator TypeWritterEffect(string _text, SpeakerUIController _speakerUI)
    {
        yield return new WaitForSeconds(0.3f);

        foreach (char letter in _text.ToCharArray())
        {
            _speakerUI.Dialogue += letter;
            yield return new WaitForSeconds(textDelayTime);
        }
    }

    public bool CheckIfConversationIsFinished()
    {
        if (conversation != null)
        {
            if (activeLineIndex < conversation.lines.Length)
            {
                return false;
            }
        }

        return true;
    }

    public void EndResetController()
    {
        speakerUILeft.Hide();
        speakerUIRight.Hide();
        speakerUILeft.HideSprite();
        speakerUIRight.HideSprite();
        activeLineIndex = 0;
        pCon.move.x = 0;
        pCon.canMove = true;
        hasDisplayedTutorial = false;
        conversation = null;
    }
}
