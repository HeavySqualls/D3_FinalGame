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
    [Tooltip("The time between when the player hits the trigger and before the dialogue begins.")]
    [SerializeField] float delayTime = 0.5f;
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
    bool tutorialsRemaining = true;
    int tutorialsPlayed;
    public int activeTutorialLineIndex = 0;

    [SerializeField] CinematicCanvasController cinematicController;
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
                AdvanceNarrative();
            }
        }
    }

    public void GetNewNarrative (sNarrative _convo)
    {
        N = _convo;

        StartCoroutine(NarrativeDelay());
    }

    IEnumerator NarrativeDelay()
    {
        if (N.hasDelay)
        {
            yield return new WaitForSeconds(N.narrativeStartDelayTime);
        }

        cinematicController.PlayNarrativeSlideIn();

        yield return new WaitForSeconds(delayTime);

        StartNarrative();

        yield break;
    }

    private void StartNarrative()
    {
        isNarrativeEventRunning = true;
        
        // Disable both the player input and the enemy movements
        pCon.canMove = false;
        Toolbox.GetInstance().GetLevelManager().PauseAllEnemies();

        // Check if this narrative is a monologue, and if so, only enable the speaker on the left side
        if (N.isMonologue)
        {
            speakerUILeft = speakerLeft.GetComponent<SpeakerUIController>();
            speakerUILeft.Speaker = N.speakerOnTheLeft;
            speakerUILeft.ShowSprite();
        }
        // Else enable both speakers for the conversation
        else
        {
            speakerUILeft = speakerLeft.GetComponent<SpeakerUIController>();
            speakerUILeft.Speaker = N.speakerOnTheLeft;
            speakerUILeft.ShowSprite();

            speakerUIRight = speakerRight.GetComponent<SpeakerUIController>();
            speakerUIRight.Speaker = N.speakerOnTheRight;
            speakerUIRight.ShowSprite();
        }

        if (!N.hasTutorial)
        {
            tutorialsRemaining = false;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        AdvanceNarrative();
    }

    // If the current line that we are interested in is less lines that the number of lines in the conversation, 
    // display it and increase the line index. Otherwise end the conversation. 
    public void AdvanceNarrative()
    {
        // Check if this narrative is a Monologue, OR if both the speaker UI's have finished animating (to prevent UI elements from being enabled/disabled
        // before they are supposed to be)
        if (N.isMonologue && !speakerUILeft.isAnimating || !N.isMonologue && !speakerUILeft.isAnimating && !speakerUIRight.isAnimating)
        {
            // If there are still lines left in the dialogue OR if there are any tutorials remaining in the narrative event that have not yet been displayed 
            // (typically located after the last line of dialogue)
            if (activeLineIndex < N.lines.Length || tutorialsRemaining)
            {
                // If this narrative has tutorials, and there are tutorials remaining to be played, the tutorial line index matches the current line index, 
                // AND we are not currently displaying a tutorial
                if (N.hasTutorial && tutorialsRemaining && N.tutorialLines[activeTutorialLineIndex] == activeLineIndex && !hasDisplayedTutorial)
                {
                    // Hide speaker UIs
                    if (N.isMonologue)
                    {
                        speakerUILeft.Hide();
                    }
                    else
                    {
                        speakerUIRight.Hide();
                        speakerUILeft.Hide();
                    }

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
                EndResetNarrativeController();
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

        if (N.isMonologue)
        {
            SetNarrativeMonologueDialogue(speakerUILeft, line.text);
        }
        else
        {
            if (speakerUILeft.SpeakerIs(character))
            {
                SetNarrativeDialogue(speakerUILeft, speakerUIRight, line.text);
            }
            else
            {
                SetNarrativeDialogue(speakerUIRight, speakerUILeft, line.text);
            }
        }
    }

    private void SetNarrativeMonologueDialogue(SpeakerUIController _activeSpeakerUI, string _text)
    {
        activeSpeakerUI = _activeSpeakerUI;
        _activeSpeakerUI.Dialogue = _text;
        _activeSpeakerUI.Show();

        _activeSpeakerUI.Dialogue = "";
        StopAllCoroutines();
        StartCoroutine(TypeWritterEffect_N(_text, _activeSpeakerUI));
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

    public void EndResetNarrativeController()
    {
        // Hide UI
        speakerUILeft.Hide();
        speakerUILeft.HideSprite();

        if (!N.isMonologue)
        {
            speakerUIRight.Hide();
            speakerUIRight.HideSprite();
        }

        cinematicController.PlayNarrativeSlideOut();

        // Hide Cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Re-Enable player 
        pCon.move.x = 0;
        pCon.canMove = true;

        // Re-emable enemies
        Toolbox.GetInstance().GetLevelManager().UnPauseAllEnemies();

        // Reset tutorial tracking vars 
        hasDisplayedTutorial = false;
        tutorialsPlayed = 0;
        tutorialsRemaining = true;
        activeTutorialLineIndex = 0;

        // Reset controller vars 
        N = null;
        activeLineIndex = 0;
        isNarrativeEventRunning = false;
    }
}
