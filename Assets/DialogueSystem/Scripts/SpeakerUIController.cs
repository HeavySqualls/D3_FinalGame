using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeakerUIController : MonoBehaviour
{
    [Header("UI Base References:")]
    public Image portrait;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogue;
    public GameObject dialoguePanelGO;
    public GameObject portraitGO;

    [Space]
    [Header("UI Input Buttons:")]
    [SerializeField] GameObject purpleButton;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject closeButton;

    [Space]
    [Header("UI Animators:")]
    [Tooltip("Drag in the animator for the speaker sprite here.")]
    public Animator portraitAnimator;
    [Tooltip("Drag in the animator for the dialogue box here.")]
    public Animator dialogueBoxAnimator;
    [Tooltip("The time to wait for the opening animation to finish before enabling the name and dialogue text.")]
    public float animationTime = 0.3f;

    // Tracks the active speaker + update name & portrait
    private sCharacter speaker;
    public sCharacter Speaker
    {
        get { return speaker; }
        set
        {
            speaker = value;
            portrait.sprite = speaker.characterSprite;
            nameText.text = speaker.characterName;
        }
    }

    // Set the dialogue text based on what is assigned
    public string Dialogue
    {
        get { return dialogue.text; }
        set { dialogue.text = value; }
    }

    // Check if the UI has a speaker
    public bool HasSpeaker()
    {
        return speaker != null;
    }

    // Check if this is the speaker in question
    public bool SpeakerIs(sCharacter _character)
    {
        return speaker == _character;
    }

    public bool isAnimating = false;

    public void Show()
    {
        dialoguePanelGO.SetActive(true);
        dialogue.enabled = true;
        StartCoroutine(WaitForNameDisplay());
        dialogueBoxAnimator.SetBool("isOpen", true);
    }

    public void Hide()
    {
        print("hide");
        HideUIItems();
        dialogueBoxAnimator.SetBool("isOpen", false);
        StartCoroutine(WaitForAnimationFinish());
    }

    IEnumerator WaitForNameDisplay()
    {
        isAnimating = true;

        yield return new WaitForSeconds(animationTime);

        purpleButton.SetActive(true);
        continueButton.SetActive(true);
        closeButton.SetActive(true);
        nameText.enabled = true;

        isAnimating = false;
    }

    IEnumerator WaitForAnimationFinish()
    {
        isAnimating = true;

        yield return new WaitForSeconds(animationTime);

        isAnimating = false;
    }

    public void ShowSprite()
    {
        portraitAnimator.SetBool("showSprite", true);
    }

    public void HideSprite()
    {
        portraitAnimator.SetBool("showSprite", false);
    }

    void HideUIItems()
    {
        purpleButton.SetActive(false);
        continueButton.SetActive(false);
        closeButton.SetActive(false);
        nameText.enabled = false;
        dialogue.enabled = false;
    }
}
