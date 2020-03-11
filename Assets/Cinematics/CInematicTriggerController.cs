using UnityEngine;
using UnityEngine.Timeline;
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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && cinematicCam.activeSelf == true)
        {
            cinematicCam.SetActive(false);

            if (pCon != null)
            {
                pCon.EnablePlayerController();
                pCon = null;
            }

            circColl.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        pCon = other.gameObject.GetComponent<PlayerController>();
        RollingObject ro = other.GetComponent<RollingObject>();

        if (pCon != null || ro != null)
        {
            timeLine.Play();
            cinematicCam.SetActive(true);

            if (pCon != null)
            {
                pCon.DisablePlayerController();
            }
        }
    }
}
