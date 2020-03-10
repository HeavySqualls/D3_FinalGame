using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeController : MonoBehaviour
{
    public sNarrative N;

    public GameObject speakerLeft;
    public GameObject speakerRight;
    private SpeakerUIController speakerUILeft;
    private SpeakerUIController speakerUIRight;
    [SerializeField] private Image backgroundImage;

    public TutorialController tutorialController;
    private bool hasDisplayedTutorial = false;

    [SerializeField] float textDelayTime;
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
        if (Input.GetKeyDown(KeyCode.F) && N != null && !tutorialController.isOpen)
        {       
            AdvanceConversation();
        }
    }

    public void GetNewNarrative (sNarrative _convo)
    {
        N = _convo;
        StartNarrative();
    }

    private void StartNarrative()
    {
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


    // If the current line that we are interested in is less lines that the number of lines in the conversation, 
    // display it and increase the line index. Otherwise end the conversation. 
    public void AdvanceConversation()
    {
        if (activeLineIndex < N.lines.Length)
        {
            if (N.hasTutorial && N.tutorialLine == activeLineIndex && !hasDisplayedTutorial)
            {
                tutorialController.DisplayTutorial(N.tutorial);
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
        Line line = N.lines[activeLineIndex];
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
        _activeSpeakerUI.Dialogue = _text;
        _activeSpeakerUI.Show();
        _inactiveSpeakerUI.Hide();

        _activeSpeakerUI.Dialogue = "";
        StopAllCoroutines();
        StartCoroutine(TypeWritterEffect_N(_text, _activeSpeakerUI));
    }

    private IEnumerator TypeWritterEffect_N(string _text, SpeakerUIController _speakerUI)
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
        hasDisplayedTutorial = false;
        N = null;

        activeLineIndex = 0;

    }
}
