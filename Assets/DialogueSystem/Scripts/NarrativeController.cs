using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeController : MonoBehaviour
{
    [Header("Assigned Narrative Scriptable Object")]
    [Tooltip("The sNarrative will automatically be assigned here by the narrative trigger.")]
    public sNarrative N;

    [Space]
    [Header("Controller Status:")]
    [Tooltip("Is there a narrative playing right now?")]
    public bool isNarrativeEventRunning = false;
    [Tooltip("The time between displaying individual letters in a dialogue.")]
    [SerializeField] float textDelayTime;
    bool isTyping = false;

    [Space]
    [Header("Speaker References:")]
    [Tooltip("Who is the speaker on the left?")]
    public GameObject speakerLeft;
    [Tooltip("Who is the speaker on the right?")]
    public GameObject speakerRight;
    private SpeakerUIController speakerUILeft;
    private SpeakerUIController speakerUIRight;

    [Space]
    [Header("Tutorial References:")]
    public TutorialController tutorialController;
    public bool hasDisplayedTutorial = false;
    [SerializeField] private Image backgroundImage;

    public int activeTutorialLineIndex = 0;
    private int activeLineIndex = 0;
    private PlayerController pCon;

    private void Awake()
    {
        Toolbox.GetInstance().GetDialogueSystemManager().SetNarrativeController(this);
    }

    private void Start()
    {
        pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();
    }

    private void Update()
    {
        // Check if the correct input was registered, if the narrative is not null, and if the tutorial panel is not open 
        // (to prevent the narrative from continuing at the same time as the tutorial panel is closed).
        if ((Input.GetButtonDown(pCon.controls.interact) || Controls.IsRight) && N != null && !tutorialController.isOpen)       
        {
            if (isTyping)
            {
                StopAllCoroutines();
                isTyping = false;
                activeSpeakerUI.Dialogue = line.text;
            }
            else
            {
                AdvanceConversation();
            }
        }
    }

    public void GetNewNarrative (sNarrative _convo)
    {
        N = _convo;
        StartNarrative();
    }

    private void StartNarrative()
    {
        isNarrativeEventRunning = true;

        speakerUILeft = speakerLeft.GetComponent<SpeakerUIController>();
        speakerUIRight = speakerRight.GetComponent<SpeakerUIController>();

        speakerUILeft.Speaker = N.speakerOnTheLeft;
        speakerUIRight.Speaker = N.speakerOnTheRight;

        speakerUILeft.ShowSprite();
        speakerUIRight.ShowSprite();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        backgroundImage.enabled = true;
        AdvanceConversation();
        pCon.canMove = false;
    }

    bool tutorialsRemaining = true;
    int tutorialsPlayed;

    // If the current line that we are interested in is less lines that the number of lines in the conversation, 
    // display it and increase the line index. Otherwise end the conversation. 
    public void AdvanceConversation()
    {
        // Check if both the speaker UI's have finished animating (to prevent UI elements from being enabled/disabled
        // before they are supposed to be)
        if (!speakerUILeft.isAnimating && !speakerUIRight.isAnimating)
        {
            if (activeLineIndex < N.lines.Length)
            {
                // If this narrative has tutorials, and there are tutorials remaining to be played, the tutorial line index matches the current line index, 
                // AND we are not currently displaying a tutorial
                if (N.hasTutorial && tutorialsRemaining && N.tutorialLines[activeTutorialLineIndex] == activeLineIndex && !hasDisplayedTutorial)
                {
                    // Hide speaker UIs
                    speakerUIRight.Hide();
                    speakerUILeft.Hide();

                    // Display tutorial that matches the index number of the activeTutorialLineIndex
                    tutorialController.DisplayTutorial(N.tutorials[activeTutorialLineIndex]);

                    // Increase the line index to select the next index line number to display the following tutorial next time (if applicable)
                    activeTutorialLineIndex++;
                    // Increase the number of tutorials we have played so we can track how many are left
                    tutorialsPlayed++;

                    // Check if we have played all the tutorials the array
                    if (tutorialsPlayed == N.tutorialLines.Length)
                    {
                        tutorialsRemaining = false;
                    }

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
    }

    // Seach the lines in the conversation and return the active one, get the character and set the active versus
    // inactive by providing the correct characters in the correct order to SetDialogue, which tracks the active
    // and inactive speaker, and displays the lines accordingly.
    Line line;
    SpeakerUIController activeSpeakerUI;

    private void DisplayNextLine()
    {
        line = N.lines[activeLineIndex];
        sCharacter character = line.character;

        if (speakerUILeft.SpeakerIs(character))
        {
            SetNarrativeDialogue(speakerUILeft, speakerUIRight, line.text);
        }
        else
        {
            SetNarrativeDialogue(speakerUIRight, speakerUILeft, line.text);
        }
    }

    private void SetNarrativeDialogue(SpeakerUIController _activeSpeakerUI, SpeakerUIController _inactiveSpeakerUI, string _text)
    {
        activeSpeakerUI = _activeSpeakerUI;
        _activeSpeakerUI.Dialogue = _text;
        _activeSpeakerUI.Show();
        _inactiveSpeakerUI.Hide();

        _activeSpeakerUI.Dialogue = "";
        StopAllCoroutines();
        StartCoroutine(TypeWritterEffect_N(_text, _activeSpeakerUI));
    }

    private IEnumerator TypeWritterEffect_N(string _text, SpeakerUIController _speakerUI)
    {
        isTyping = true;

        yield return new WaitForSeconds(0.3f);

        foreach (char letter in _text.ToCharArray())
        {
            _speakerUI.Dialogue += letter;
            yield return new WaitForSeconds(textDelayTime);
        }

        isTyping = false;
    }

    public bool CheckIfConversationIsFinished()
    {
        if (N != null)
        {
            if (activeLineIndex < N.lines.Length)
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
        backgroundImage.enabled = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        pCon.move.x = 0;
        pCon.canMove = true;
        N = null;

        hasDisplayedTutorial = false;
        tutorialsPlayed = 0;
        tutorialsRemaining = true;
        activeTutorialLineIndex = 0;

        activeLineIndex = 0;
        isNarrativeEventRunning = false;
    }
}
