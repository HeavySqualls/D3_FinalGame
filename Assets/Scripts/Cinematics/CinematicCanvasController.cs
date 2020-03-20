using System.Collections;
using UnityEngine;

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
        continueButton.SetActive(true);
    }

    public void ContinueButtonPressed()
    {
        cinematicTrigger.EndCutScene();
        cinematicTrigger = null;
    }
}
