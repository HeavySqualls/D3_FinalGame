using UnityEngine;
using UnityEngine.Playables;

public class CinematicTriggerController : MonoBehaviour
{
    public PlayableDirector timeLine;
    public GameObject cinematicCam;

    PlayerController pCon;
    CircleCollider2D circColl;

    void Start()
    {
        circColl = GetComponent<CircleCollider2D>();
        pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();
    }

    void Update()
    {
        //Toolbox.GetInstance().GetCanvasManager().GetCinematicCanvasController().ShowContinueButton();

        if ((Input.GetButtonDown(pCon.controls.interact) || Controls.IsDown) && cinematicCam.activeSelf == true)
        {
            EndCutScene();
        }
    }

    public void EndCutScene()
    {
        Toolbox.GetInstance().GetCanvasManager().GetCinematicCanvasController().PlayCutSceneSlideOut();
        cinematicCam.SetActive(false);

        pCon.EnablePlayerController();

        circColl.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController pRef = other.gameObject.GetComponent<PlayerController>();
        RollingObject ro = other.GetComponent<RollingObject>();

        if (pRef != null || ro != null)
        {
            Toolbox.GetInstance().GetCanvasManager().GetCinematicCanvasController().PlayCutSceneSlideIn(this);
            timeLine.Play();
            cinematicCam.SetActive(true);

            pCon.DisablePlayerController();
        }
    }

    public void DisplayContinuePrompt()
    {

    }
}
