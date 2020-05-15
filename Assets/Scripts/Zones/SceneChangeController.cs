using UnityEngine;

public class SceneChangeController : MonoBehaviour
{
    [SerializeField] AudioClip winSound;
    [SerializeField] float winVolume = 0.6f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Change Scene");
        PlayerController pCon = collision.gameObject.GetComponent<PlayerController>();

        if (pCon != null)
            ChangeScene();
    }

    public void ChangeScene()
    {
        Toolbox.GetInstance().GetGameManager().LoadNextScene();
    }

    public void PlayWinSound()
    {
        if (winSound != null)
        {
            Toolbox.GetInstance().GetAudioManager().PlayJingle(winSound, true);
        }
    }
}
