using UnityEngine;

public class PlayerHandleInventory : MonoBehaviour
{
    [SerializeField] GameObject characterPanelGO;
    [SerializeField] GameObject itemTooltip;
    [SerializeField] GameObject statTooltip;
    [SerializeField] GameObject lootBoxPanel;
    [SerializeField] KeyCode[] toggleInventoryKeys;

    void Update()
    {
        for (int i = 0; i < toggleInventoryKeys.Length; i++)
        {
            if (Input.GetKeyDown(toggleInventoryKeys[i]))
            {
                characterPanelGO.SetActive(!characterPanelGO.activeSelf);

                if (characterPanelGO.activeSelf)
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
}
