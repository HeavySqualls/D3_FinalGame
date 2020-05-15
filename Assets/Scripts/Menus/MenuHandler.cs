using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.EventSystems;

public class MenuHandler : MonoBehaviour
{
    [Header("Main Menu:")]
    [SerializeField] Button startGameButton;
    [SerializeField] Button quitGameButton;

    [Space]
    [Header("Pause Menu:")]
    public bool isPauseMenu = false;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] Text menuTitleText;
    [SerializeField] Button quitToMainMenu;
    [SerializeField] Button resetLevelButton;
    [SerializeField] Button continueButton;

    [Space]
    [Header("Options Menu:")]
    [SerializeField] GameObject OptionsMenu;
    [SerializeField] Button optionsButton;
    public GameObject controllerToggle;
    public GameObject keyboardToggle;
    bool isController;

    [Space]
    [Header("Death Menu:")]
    [SerializeField] GameObject DeathMenu;
    [SerializeField] Button respawnButton;

    [Space]
    [Header("Audio:")]
    [SerializeField] AudioClip hoverSound;
    [SerializeField] float hoverVolume = 0.3f;
    [SerializeField] AudioClip clickSound;
    [SerializeField] float clickVolume = 0.3f;
    [SerializeField] AudioClip openMenuSound;
    [SerializeField] float openMenuVolume = 0.3f;
    AudioManager AM;

    [Space]
    [Header("Button Navigation:")]
    [SerializeField] GameObject mainMenuFirstSelected;
    [SerializeField] GameObject pauseFirstSelected;
    [SerializeField] GameObject optionsFirstButton;
    [SerializeField] GameObject optionsClosedButton;
    [SerializeField] GameObject firstDeathMenuButton;


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
        AM = Toolbox.GetInstance().GetAudioManager();

        if (isPauseMenu)
            pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();

        if (OptionsMenu != null)
            OptionsMenu.SetActive(false);

        if (DeathMenu != null)
            DeathMenu.SetActive(false);

        if (PauseMenu != null)
            PauseMenu.SetActive(false);

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
            if (Input.GetButtonDown(pCon.controls.pauseMenu) && !DeathMenu.activeSelf)
            {
                OpenClosePauseMenu();
            }
        }
    }

    public void SetMainMenuFirstSelected() // Called from timeline animations
    {
        // Clear selected object 
        EventSystem.current.SetSelectedGameObject(null);
        // Set a new selected object
        EventSystem.current.SetSelectedGameObject(mainMenuFirstSelected);
    }

    public void ToggleControllerInput()
    {
        isController = true;

        Toolbox.GetInstance().GetPlayerManager().SetControlType(isController);
        print("controller switch toggled");
    }

    public void ToggleKeyboardInput()
    {
        isController = false;

        Toolbox.GetInstance().GetPlayerManager().SetControlType(isController);
        print("keyboard switch toggled");
    }

    public void StartGame()
    {
        AM.PlayConsistentOneShot(clickSound, clickVolume);
        gm.LoadNextScene();
    }

    public void PlayHoverSound()
    {
        AM.PlayConsistentOneShot(hoverSound, hoverVolume);
    }

    public void OpenClosePauseMenu()
    {
        PauseMenu.SetActive(!PauseMenu.activeSelf);
        AM.DampenBGMusic(PauseMenu.activeSelf);
        AM.MuteAudioSources(PauseMenu.activeSelf);

        AM.PlayConsistentOneShot(openMenuSound, openMenuVolume);

        if (PauseMenu.activeSelf && !OptionsMenu.activeSelf)
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            pCon.DisablePlayerController();

            // Clear selected object 
            EventSystem.current.SetSelectedGameObject(null);
            // Set a new selected object
            EventSystem.current.SetSelectedGameObject(pauseFirstSelected);
        }
        else
        {
            Time.timeScale = 1;

            if (!Toolbox.GetInstance().GetDialogueSystemManager().GetNarrativeController().isNarrativeEventRunning)
            {
                if (!Toolbox.GetInstance().GetPlayerManager().isInventoryOpen)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }

                pCon.EnablePlayerController();
            }
        }
    }

    private void EnableDeathMenu()
    {
        DeathMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Clear selected object 
        EventSystem.current.SetSelectedGameObject(null);
        // Set a new selected object
        EventSystem.current.SetSelectedGameObject(firstDeathMenuButton);
    }

    public void ResetToLastCheckpoint()
    {
        AM.PlayConsistentOneShot(clickSound, clickVolume);
        DeathMenu.SetActive(false);
        PauseMenu.SetActive(false);
        SpawnManager.ResetLevelObjects();
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ResetLevel()
    {
        AM.PlayConsistentOneShot(clickSound, clickVolume);

        Time.timeScale = 1;
        gm.RestartLevel();
    }

    public void OpenOptions()
    {
        AM.PlayConsistentOneShot(clickSound, clickVolume);
        OptionsMenu.SetActive(true);

        // Clear selected object 
        EventSystem.current.SetSelectedGameObject(null);
        // Set a new selected object
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);

        if (PauseMenu != null)
        {
            PauseMenu.SetActive(true);
        }
    }

    public void CloseOptions()
    {
        AM.PlayConsistentOneShot(clickSound, clickVolume);
        OptionsMenu.SetActive(false);

        // Clear selected object 
        EventSystem.current.SetSelectedGameObject(null);
        // Set a new selected object
        EventSystem.current.SetSelectedGameObject(optionsClosedButton);
    }

    public void QuitToMainMenu()
    {
        AM.PlayConsistentOneShot(clickSound, clickVolume);
        Time.timeScale = 1;
        gm.LoadCustomScene(1);
    }

    public void Quit()
    {
        AM.PlayConsistentOneShot(clickSound, clickVolume);
        gm.QuitGame();
    }
}
