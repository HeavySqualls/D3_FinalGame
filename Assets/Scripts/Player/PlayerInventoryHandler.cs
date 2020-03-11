using UnityEngine;

public class PlayerInventoryHandler : MonoBehaviour
{
    public GameObject lootBoxPanel;

    [SerializeField] GameObject inventory;
    [SerializeField] GameObject equipmentPanel;
    [SerializeField] GameObject statsPanel;
    [SerializeField] GameObject itemTooltip;
    [SerializeField] GameObject statTooltip;

    bool isInventoryOpen = false;
    PlayerController pCon;

    private void Awake()
    {
        Toolbox.GetInstance().GetPlayerManager().SetPlayerInventoryHandler(this);
    }

    private void Start()
    {
        pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();
        equipmentPanel.SetActive(!equipmentPanel.activeSelf);
    }

    void Update()
    {
        if (Input.GetButtonDown(pCon.controls.inventory) || Controls.IsUp)
        {
            if (isInventoryOpen)
            {
                HideMouseCursor();
                DisableInventory();
            }
            else if (!isInventoryOpen)
            {
                ShowMouseCursor();
                EnableInventory();
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
        statTooltip.SetActive(false);
    }

    private void EnableInventory()
    {
        inventory.SetActive(true);
        equipmentPanel.SetActive(true);
        statsPanel.SetActive(true);
        isInventoryOpen = true;
    }

    private void DisableInventory()
    {
        inventory.SetActive(false);
        equipmentPanel.SetActive(false);
        statsPanel.SetActive(false);
        isInventoryOpen = false;

        if (lootBoxPanel != null && lootBoxPanel.activeSelf == true)
        {
            lootBoxPanel.SetActive(false);
        }
    }
}
