using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicCanvasController : MonoBehaviour
{
    [SerializeField] GameObject continueButton;
    CinematicTriggerController cinematicTrigger;
    Animator animator;

    private void Start()
    {
        Toolbox.GetInstance().GetCanvasManager().SetCinematicCanvasCon(this);
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        ShowContinueButton();
    }

    public void PlayNarrativeSlideIn()
    {
        animator.SetBool("isNarrativeSlideIn", true);
    }

    public void PlayNarrativeSlideOut()
    {
        animator.SetBool("isNarrativeSlideIn", false);
    }

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
        if (cinematicTrigger != null && cinematicTrigger.timeLine.state != PlayState.Playing && !continueButton.activeSelf)
        {
            print("Show Continue Button");
            continueButton.SetActive(true);
        }
    }

    public void HideContinueButton()
    {
        continueButton.SetActive(false);
    }

    public void ContinueButtonPressed()
    {
        HideContinueButton();
        cinematicTrigger.EndCutScene();
        cinematicTrigger = null;
    }
}
