using UnityEngine;
using UnityEngine.Playables;

public class CinematicCanvasController : MonoBehaviour
{
    [Tooltip("The prompt that will tell the player that they can close the cutscene and continue playing.")]
    [SerializeField] GameObject continuePrompt;

    [Tooltip("Gets automatically assighed by the trigger itself, and nullified when the continue prompt is closed.")]
    CinematicTriggerController cinematicTrigger;

    Animator animator;

    private void Start()
    {
        Toolbox.GetInstance().GetCanvasManager().SetCinematicCanvasCon(this);
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // if we have a cinematic controller assigned, 
        if (cinematicTrigger != null)
        {
            if (!cinematicTrigger.isFlowCutscene)
            {
                // start tracking whether the cutscene has finished, and if so, display the continue prompt
                ShowContinueButton();

                // if we have recieved the correct button input, and if the cinemtic timeline has stopped playing, and if the cinematic camera
                // is still active
                if ((Input.GetButtonDown(Toolbox.GetInstance().GetPlayerManager().GetPlayerController().controls.interact) || Controls.IsDown)
                   && cinematicTrigger.timeLine.state != PlayState.Playing && cinematicTrigger.cinematicCam.activeSelf == true)
                {
                    // End the cutscene from the trigger
                    cinematicTrigger.EndCutScene();
                    // slide out cinematic bars
                    if (cinematicTrigger.isCutscene)
                        PlayCutSceneSlideOut();

                    // hide the continue prompt
                    HideContinueButton();
                    // nullify the cinematicTrigger variable
                    cinematicTrigger = null;
                }
            }
            else if (cinematicTrigger.isFlowCutscene && cinematicTrigger.timeLine.state != PlayState.Playing)
            {
                cinematicTrigger.EndCutScene();
            }
        }
    }


    // < ------------------------------------- CUTSCENES ------------------------------------- >> //

    public void PlayCutSceneSlideIn(CinematicTriggerController _cinematicTrigger)
    {
        animator.SetBool("isCutSceneSlideIn", true);
        cinematicTrigger = _cinematicTrigger;
    }

    public void PlayCutSceneSlideOut()
    {
        animator.SetBool("isCutSceneSlideIn", false);
    }

    public void ShowContinueButton()
    {
        if (cinematicTrigger.timeLine.state != PlayState.Playing && !continuePrompt.activeSelf)
        {
            print("Show Continue Button");
            continuePrompt.SetActive(true);
        }
    }

    public void HideContinueButton()
    {
        continuePrompt.SetActive(false);
    }


    // < ------------------------------------- NARRATIVE ------------------------------------- >> //

    public void PlayNarrativeSlideIn()
    {
        animator.SetBool("isNarrativeSlideIn", true);
    }

    public void PlayNarrativeSlideOut()
    {
        animator.SetBool("isNarrativeSlideIn", false);
    }
}
