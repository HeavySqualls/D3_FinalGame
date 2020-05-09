using UnityEngine;

public class CameraChangeTriggerController : MonoBehaviour
{
    public GameObject cinematicCamera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController pCon = collision.GetComponent<PlayerController>();

        if (pCon != null)
        {
            cinematicCamera.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController pCon = collision.GetComponent<PlayerController>();

        if (pCon != null)
        {
            cinematicCamera.SetActive(false);
        }
    }

    public void DisableCamera()
    {
        cinematicCamera.SetActive(false);
    }
}
