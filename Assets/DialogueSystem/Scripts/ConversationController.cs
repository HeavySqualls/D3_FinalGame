using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationController : MonoBehaviour
{
    public sNarrative conversation;

    public GameObject speakerLeft;
    public GameObject speakerRight;
    public GameObject continueButton;
    public TutorialController tutorialController;

    [SerializeField] float textDelayTime;

    private SpeakerUIController speakerUILeft;
    private SpeakerUIController speakerUIRight;

    private int activeLineIndex = 0;
    private PlayerController pCon;

    private void Start()
    {
        pCon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && conversation != null)
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
        continueButton.SetActive(!continueButton.activeSelf);
    }

    // If the current line that we are interested in is less lines that the number of lines in the conversation, 
    // display it and increase the line index. Otherwise end the conversation. 
    public void AdvanceConversation()
    {
        if (activeLineIndex < conversation.lines.Length)
        {
            DisplayNextLine();
            activeLineIndex += 1;
        }
        else
        {
            speakerUILeft.Hide();
            speakerUIRight.Hide();
            speakerUILeft.HideSprite();
            speakerUIRight.HideSprite();
            activeLineIndex = 0;
            pCon.move.x = 0;
            pCon.canMove = true;
            continueButton.SetActive(!continueButton.activeSelf);

            if (conversation.hasTutorial)
            {
                tutorialController.DisplayTutorial(conversation.tutorial);
            }

            conversation = null;
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
}
