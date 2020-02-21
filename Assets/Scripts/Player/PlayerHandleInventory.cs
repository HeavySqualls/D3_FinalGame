using UnityEngine;

public class PlayerHandleInventory : MonoBehaviour
{
    bool isInventoryOpen = false;

    [SerializeField] GameObject inventory;
    [SerializeField] GameObject equipmentPanel;
    [SerializeField] GameObject statsPanel;
    [SerializeField] GameObject itemTooltip;
    [SerializeField] GameObject statTooltip;
    [SerializeField] GameObject lootBoxPanel;
    [SerializeField] KeyCode inventoryKey;

    private void Start()
    {
        // TODO: find out why this blocks item from being picked up until the inventory has been enabled - call order issue?
        // EnableDisableInventory();
        equipmentPanel.SetActive(!equipmentPanel.activeSelf);
    }

    void Update()
    {
        if (Input.GetKeyDown(inventoryKey))
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

        if (lootBoxPanel.activeSelf == true)
        {
            lootBoxPanel.SetActive(false);
        }
    }
}
