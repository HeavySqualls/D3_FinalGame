﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeakerUIController : MonoBehaviour
{
    public Image portrait;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogue;

    public float animationTime = 0.5f;
    public bool isReadyForText = false;

    public GameObject dialoguePanelGO;
    public GameObject portraitGO;

    public Animator portraitAnimator;
    public Animator dialogueBoxAnimator;
    
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

    public void Show()
    {
        dialoguePanelGO.SetActive(true);
        dialogue.enabled = true;
        StartCoroutine(WaitForNameDisplay());
        dialogueBoxAnimator.SetBool("isOpen", gameObject.activeSelf);
    }

    public void Hide()
    {
        nameText.enabled = false;
        dialogue.enabled = false;
        dialogueBoxAnimator.SetBool("isOpen", false);
    }

    IEnumerator WaitForNameDisplay()
    {
        yield return new WaitForSeconds(0.3f);

        nameText.enabled = !nameText.IsActive();
    }

    public void ShowSprite()
    {
        portraitAnimator.SetBool("showSprite", true);
    }

    public void HideSprite()
    {
        portraitAnimator.SetBool("showSprite", false);
    }
}