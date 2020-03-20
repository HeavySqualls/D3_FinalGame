﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public bool isPauseMenu = false;

    [SerializeField] Button startGameButton;
    [SerializeField] Button quitGameButton;

    [SerializeField] Button optionsButton;

    [SerializeField] Button continueButton;
    [SerializeField] Button resetLevelButton;
    [SerializeField] Button quitToMainMenu;

    [SerializeField] GameObject OptionsMenu;
    [SerializeField] GameObject PauseMenu;

    [SerializeField] PlayableDirector timeLine; //TODO: move this to some sort of scene manager

    GameManager gm;
    PlayerController pCon;

    private void Start()
    {
        gm = Toolbox.GetInstance().GetGameManager();

        if (isPauseMenu)
        {
            pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();
        }

        OptionsMenu.SetActive(false);

        if (PauseMenu != null)
        {
            PauseMenu.SetActive(false);
        }

        if (timeLine != null)
        {
            timeLine.Play();
            print("play");
        }
    }

    private void Update()
    {
        if (isPauseMenu)
        {
            if (Input.GetButtonDown(pCon.controls.pauseMenu))
            {
                OpenClosePauseMenu();
            }
        }
    }

    public void StartGame()
    {
        gm.LoadNextScene();
    }

    public void OpenClosePauseMenu()
    {
        PauseMenu.SetActive(!PauseMenu.activeSelf);

        if (PauseMenu.activeSelf && !OptionsMenu.activeSelf)
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1;

            if (!Toolbox.GetInstance().GetDialogueSystemManager().GetNarrativeController().isNarrativeEventRunning)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void ResetLevel()
    {
        Time.timeScale = 1;
        gm.RestartLevel();
    }

    public void OpenCloseOptions()
    {
        OptionsMenu.SetActive(!OptionsMenu.activeSelf);

        if (PauseMenu != null)
        {
            PauseMenu.SetActive(!PauseMenu.activeSelf);
        }
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1;
        gm.LoadCustomScene(0);
    }

    public void Quit()
    {
        gm.QuitGame();
    }
}
