﻿using UnityEngine;
using UnityEngine.Playables;

public class CinematicTriggerController : MonoBehaviour
{
    [Tooltip("Does this cinematic contain a camera change?")]
    public bool hasCameraChange = true;
    [Tooltip("Does this camera return to it's original position after the cutscene?")]
    public bool camReturnToOriginalPos = true;
    [Tooltip("Will this cutscene wait for an action from the player before ending?")]
    public bool hasContinueButton = true;
    [Tooltip("Will this cutscene have black bars fade in at the beginning?")]
    public bool hasBlackBarsStart = false;
    [Tooltip("Will this cutscene remove the black bars at the end?")]
    public bool removeBlackBarsEnd = true;
    [Tooltip("Is this a flow cutscene? (camera change, with no black bars)")]
    public bool isFlowCutscene = false;

    public PlayableDirector timeLine;
    public GameObject cinematicCam;

    PlayerController pCon;
    CircleCollider2D circColl;

    void Start()
    {
        circColl = GetComponent<CircleCollider2D>();
        pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();
    }

    public void DisableTrigger()
    {
        circColl.enabled = false;
    }

    public void EndCutScene()
    {
        print("End cutscene");
        // If this cutscene has a camera change, and the camera will return to its original position, 
        // Else the camera will stay in its new position. 
        if (camReturnToOriginalPos && cinematicCam.activeSelf)
        {
            print("disable cinematic camera");
            cinematicCam.SetActive(false);
        }

        pCon.EnablePlayerController();

        circColl.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController pRef = other.gameObject.GetComponent<PlayerController>();
        RollingObject ro = other.GetComponent<RollingObject>();

        if (pRef != null || ro != null)
        {
            PlayCinematic();
        }
    }

    public void PlayCinematic()
    {
        if (hasContinueButton || hasBlackBarsStart)
        {
            print("slide in the bars");
            Toolbox.GetInstance().GetCanvasManager().GetCinematicCanvasController().PlayCutSceneSlideIn(this);
        }

        if (hasCameraChange && !cinematicCam.activeSelf)
            cinematicCam.SetActive(true);

        timeLine.Play();

        if (!isFlowCutscene)
        {
            pCon.DisablePlayerController();
        }
    }
}
