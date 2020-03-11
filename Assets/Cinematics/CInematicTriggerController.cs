using UnityEngine;
using UnityEngine.Playables;

public class CInematicTriggerController : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.F) && cinematicCam.activeSelf == true)
        {
            cinematicCam.SetActive(false);

            pCon.EnablePlayerController();

            circColl.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController pRef = other.gameObject.GetComponent<PlayerController>();
        RollingObject ro = other.GetComponent<RollingObject>();

        if (pRef != null || ro != null)
        {
            timeLine.Play();
            cinematicCam.SetActive(true);

            pCon.DisablePlayerController();
        }
    }
}
