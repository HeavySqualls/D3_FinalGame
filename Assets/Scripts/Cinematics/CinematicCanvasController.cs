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
            if (cinematicTrigger.hasContinueButton)
            {
                // start tracking whether the cutscene has finished, and if so, display the continue prompt
                WaitAndShowContinueButton();

                // if we have recieved the correct button input, and if the cinemtic timeline has stopped playing, and if the cinematic camera
                // is still active
                if ((Input.GetButtonDown(Toolbox.GetInstance().GetPlayerManager().GetPlayerController().controls.interact) || 
                    Controls.IsDown || Input.GetButtonDown(Toolbox.GetInstance().GetPlayerManager().GetPlayerController().controls.jump))
                   && cinematicTrigger.timeLine.state != PlayState.Playing && cinematicTrigger.cinematicCam.activeSelf == true)
                {
                    // if the cinematic trigger is set up to remove the black bars after the cinematic is over, 
                    // slide out cinematic bars
                    if (cinematicTrigger.removeBlackBarsEnd)
                        PlayCutSceneSlideOut();

                    // hide the continue prompt
                    HideContinueButton();

                    // end the cutscene from the trigger
                    cinematicTrigger.EndCutScene();

                    // nullify the cinematicTrigger variable
                    cinematicTrigger = null;
                }
            }
            else if (cinematicTrigger.timeLine.state != PlayState.Playing)
            {
                // if the cinematic trigger is set up to remove the black bars after the cinematic is over, 
                // slide out cinematic bars
                if (cinematicTrigger.removeBlackBarsEnd)
                    PlayCutSceneSlideOut();

                // end the cutscene from the trigger
                cinematicTrigger.EndCutScene();

                // nullify the cinematicTrigger variable
                cinematicTrigger = null;
            }
        }
    }

    public void EndCutScene()
    {
        // if the cinematic trigger is set up to remove the black bars after the cinematic is over, 
        // slide out cinematic bars

        if (cinematicTrigger.removeBlackBarsEnd)
            PlayCutSceneSlideOut();

        // end the cutscene from the trigger
        cinematicTrigger.EndCutScene();

        // nullify the cinematicTrigger variable
        cinematicTrigger = null;
    }


    // < ------------------------------------- CUTSCENES ------------------------------------- >> //

    public void PlayCutSceneSlideIn(CinematicTriggerController _cinematicTrigger)
    {
        animator.SetBool("isCutSceneSlideIn", true);
        cinematicTrigger = _cinematicTrigger;
    }

    public void PlayCutSceneSlideOut()
    {
        print("cutscene slide out ");
        animator.SetBool("isCutSceneSlideIn", false);
    }

    public void WaitAndShowContinueButton()
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
