using UnityEngine;

public class SceneChangeController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Change Scene");

        Toolbox.GetInstance().GetGameManager().LoadNextScene();
    }
}
