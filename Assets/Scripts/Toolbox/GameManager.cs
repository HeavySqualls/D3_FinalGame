using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Space]
    [Header("Scene Management:")]
    private int currentLevelIndex;

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

    public void LoadCustomScene(int _sceneToLoad)
    {
        SceneManager.LoadScene(_sceneToLoad);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(currentLevelIndex + 1);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(currentLevelIndex);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    // ------ TOOLBOX REFERENCE RESET ------ //

    private void ResetGameManager()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
    }
}
