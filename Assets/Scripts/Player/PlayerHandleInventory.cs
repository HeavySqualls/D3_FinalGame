using UnityEngine;

public class PlayerHandleInventory : MonoBehaviour
{
    [SerializeField] GameObject inventoryGO;
    [SerializeField] KeyCode[] toggleInventoryKeys;

    void Update()
    {
        for (int i = 0; i < toggleInventoryKeys.Length; i++)
        {
            if (Input.GetKeyDown(toggleInventoryKeys[i]))
            {
                inventoryGO.SetActive(!inventoryGO.activeSelf);

                if (inventoryGO.activeSelf)
                    ShowCursor();
                else
                    HideMouseCursor();

                break;
            }
        }
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideMouseCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
