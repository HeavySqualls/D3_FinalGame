using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MenuHandler : MonoBehaviour
{
    [Header("Main Menu:")]
    [SerializeField] Button startGameButton;
    [SerializeField] Button quitGameButton;

    [Space]
    [Header("Pause Menu:")]
    public bool isPauseMenu = false;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] Button quitToMainMenu;
    [SerializeField] Button resetLevelButton;
    [SerializeField] Button continueButton;

    [Space]
    [Header("Options Menu:")]
    [SerializeField] GameObject OptionsMenu;
    [SerializeField] Button optionsButton;

    [Space]
    [Header("Death Menu:")]
    [SerializeField] GameObject DeathMenu;
    [SerializeField] Button respawnButton;

    [Space]
    [Header("References:")]
    [SerializeField] PlayableDirector timeLine; //TODO: move this to some sort of scene manager
    GameManager gm;
    PlayerController pCon;

    private void OnEnable()
    {
        SpawnManager.onPlayerKilled += EnableDeathMenu;
    }

    private void OnDisable()
    {
        SpawnManager.onPlayerKilled -= EnableDeathMenu;
    }

    private void Start()
    {
        gm = Toolbox.GetInstance().GetGameManager();

        if (isPauseMenu)
        {
            pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();
        }

        OptionsMenu.SetActive(false);
        DeathMenu.SetActive(false);

        if (PauseMenu != null)
        {
            PauseMenu.SetActive(false);
        }

        if (timeLine != null)
        {
            timeLine.Play();
            print("play");
        }

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
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

    private void EnableDeathMenu()
    {
        DeathMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResetToLastCheckpoint()
    {
        DeathMenu.SetActive(false);
        SpawnManager.ResetLevelObjects();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ResetLevel()
    {
        //DeathMenu.SetActive(false);
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
