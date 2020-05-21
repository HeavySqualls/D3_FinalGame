using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Space]
    [Header("Scene Management:")]
    private int currentLevelIndex;

    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGameManager();
        Toolbox.GetInstance().GetLevelManager().ClearLists();
        Toolbox.GetInstance().GetAudioManager().ClearAudioSources();
    }

    void Start()
    {
        currentLevelIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    }


    // ------ SCENE MANAGEMENT ------ //

    public void LoadCustomScene(string _sceneToLoad)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneToLoad);
    }

    public void LoadNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentLevelIndex + 1);
    }

    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentLevelIndex);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    // ------ TOOLBOX REFERENCE RESET ------ //

    private void ResetGameManager()
    {
        currentLevelIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    }
}
