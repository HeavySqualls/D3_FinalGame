using UnityEngine;

public class PlayerHandleInventory : MonoBehaviour
{
    bool isInventoryOpen = true;

    [SerializeField] GameObject inventory;
    [SerializeField] GameObject equipmentPanel;
    [SerializeField] GameObject statsPanel;
    [SerializeField] GameObject itemTooltip;
    [SerializeField] GameObject statTooltip;
    [SerializeField] GameObject lootBoxPanel;
    [SerializeField] KeyCode[] toggleInventoryKeys;

    private void Start()
    {
       // TODO: find out why this blocks item from being picked up until the inventory has been enabled - call order issue?
       // EnableDisableInventory();
    }

    void Update()
    {
        for (int i = 0; i < toggleInventoryKeys.Length; i++)
        {
            if (Input.GetKeyDown(toggleInventoryKeys[i]))
            {
                EnableDisableInventory();

                if (isInventoryOpen)
                {
                    ShowMouseCursor();
                }
                else
                {
                    HideMouseCursor();
                }

                break;
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
        lootBoxPanel.SetActive(false);
    }

    private void EnableDisableInventory()
    {
        inventory.SetActive(!inventory.activeSelf);
        equipmentPanel.SetActive(!equipmentPanel.activeSelf);
        statsPanel.SetActive(!statsPanel.activeSelf);
        isInventoryOpen = !isInventoryOpen;
    }
}
