using UnityEngine;
using UnityEngine.Playables;

public class CinematicTriggerController : MonoBehaviour
{
    [Tooltip("Does this cinematic contain a camera change?")]
    public bool isCameraChange = true;
    [Tooltip("Is this a cutscene? (black bars on top & bottom)")]
    public bool isCutscene = true;
    [Tooltip("Is this a flow cutscene?")]
    public bool isFlowCutscene;

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
        if (isCameraChange)
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
            if (isCutscene)
                Toolbox.GetInstance().GetCanvasManager().GetCinematicCanvasController().PlayCutSceneSlideIn(this);

            if (isCameraChange)
                cinematicCam.SetActive(true);

            timeLine.Play();

            if (!isFlowCutscene)
            {
                pCon.DisablePlayerController();
            }
        }
    }
}
