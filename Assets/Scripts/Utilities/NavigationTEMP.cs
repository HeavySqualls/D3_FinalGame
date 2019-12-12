using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavigationTEMP : MonoBehaviour
{
    // HANDLE ALL SCENE CHANGES
    [Space]
    [Header("SCENE MANAGEMENT:")]
    private int currentLevelIndex;

    void Start()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadLevelGym()
    {
        SceneManager.LoadScene("TestLevel");
    }
    public void LoadLevelMechanic()
    {
        SceneManager.LoadScene("gym_PlayerController");
    }
    public void LoadLevelPatterns()
    {
        SceneManager.LoadScene("gym_CrumblingBoras");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(currentLevelIndex);
    }
}
