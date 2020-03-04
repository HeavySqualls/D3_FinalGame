using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Space]
    [Header("Scene Management:")]
    private int currentLevelIndex;
    private bool isLastLevel = false;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGameManager();
    }

    void Start()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
    }


    // ------ SCENE MANAGEMENT ------ //


    public void LevelComplete()
    {
        if (isLastLevel)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(currentLevelIndex + 1);
        }
    }

    public void RestartLevel(bool _ismanualReset)
    {
        SceneManager.LoadScene(currentLevelIndex);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }


    // ------ TOOLBOX REFERENCE RESET ------ //

    private void ResetGameManager()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
    }
}
