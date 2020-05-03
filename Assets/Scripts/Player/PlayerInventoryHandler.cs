using UnityEngine;

public class PlayerInventoryHandler : MonoBehaviour
{
    public GameObject lootBoxPanel;

    [SerializeField] GameObject inventory;
    //[SerializeField] GameObject equipmentPanel;
    //[SerializeField] GameObject statsPanel;
    [SerializeField] GameObject itemTooltip;
    //[SerializeField] GameObject statTooltip;

    bool isInventoryOpen = false;
    [SerializeField] AudioClip openSound;
    [SerializeField] float openSoundVolume = 0.15f;
    AudioManager AM;
    PlayerController pCon;

    private void Awake()
    {
        Toolbox.GetInstance().GetPlayerManager().SetPlayerInventoryHandler(this);
    }

    private void Start()
    {
        pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();
        //equipmentPanel.SetActive(!equipmentPanel.activeSelf);
        AM = Toolbox.GetInstance().GetAudioManager();
    }

    void Update()
    {
        if (!pCon.isDisabled)
        {
            if (Input.GetButtonDown(pCon.controls.inventory) || Controls.IsUp)
            {
                if (isInventoryOpen)
                {
                    AM.PlayConsistentOneShot(openSound, openSoundVolume);

                    HideMouseCursor();
                    DisableInventory();
                }
                else if (!isInventoryOpen)
                {
                    AM.PlayConsistentOneShot(openSound, openSoundVolume);

                    ShowMouseCursor();
                    EnableInventory();
                }
            }
        }
    }

    private void ShowMouseCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HideMouseCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        itemTooltip.SetActive(false);
        //statTooltip.SetActive(false);
    }

    private void EnableInventory()
    {
        inventory.SetActive(true);
        inventory.GetComponent<Inventory>().OpenCloseInventory(true);
        isInventoryOpen = true;

        //equipmentPanel.SetActive(true);
        //statsPanel.SetActive(true);
    }

    private void DisableInventory()
    {
        inventory.GetComponent<Inventory>().OpenCloseInventory(false);
        //inventory.SetActive(false);
        //equipmentPanel.SetActive(false);
        //statsPanel.SetActive(false);
        isInventoryOpen = false;

        if (lootBoxPanel != null && lootBoxPanel.activeSelf == true)
        {
            lootBoxPanel.SetActive(false);
        }
    }
}
