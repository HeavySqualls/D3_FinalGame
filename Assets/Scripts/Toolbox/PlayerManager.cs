using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Store a references to the player related components in scene for other classes to access it. 

    InventoryManager inventoryManager;
    PlayerInventoryHandler pInventoryHandler;
    PlayerController pCon;
    PlayerFeedback pFeedback;
    PlayerHealthSystem pHealth;
    AirTankController airTankCon;

    bool isControllerActivated;

    public void SetControlType(bool _isController)
    {
        isControllerActivated = _isController;

        if (pCon != null)
            pCon.isController = _isController;
    }

    public bool GetControlType()
    {
        return isControllerActivated;
    }

    // <<--------------------------------------------- SET UP PLAYER CONTROLLER ------------------------------------------- >> //

    public void SetPlayerHealthSystem(PlayerHealthSystem _pHealth)
    {
        pHealth = _pHealth;
    }

    public PlayerHealthSystem GetPlayerHealthSystem()
    {
        return pHealth;
    }

    // <<--------------------------------------------- SET UP PLAYER CONTROLLER ------------------------------------------- >> //

    public void SetPlayerController(PlayerController _pCon)
    {
        pCon = _pCon;
    }

    public PlayerController GetPlayerController()
    {
        return pCon;
    }


    // <<--------------------------------------------- SET UP PLAYER FEEDBACK ------------------------------------------- >> //

    public void SetPlayerFeedback(PlayerFeedback _pFeedback)
    {
        pFeedback = _pFeedback;
    }

    public PlayerFeedback GetPlayerFeedback()
    {
        return pFeedback;
    }


    // <<--------------------------------------------- SET UP INVENTORY MANAGER ------------------------------------------- >> //

    public void SetInventoryManager(InventoryManager _inMan)
    {
        inventoryManager = _inMan;
    }

    public InventoryManager GetInventoryManager()
    {
        return inventoryManager;
    }


    // <<--------------------------------------------- SET UP PLAYER HANDLE INVENTORY ------------------------------------------- >> //

    public void SetPlayerInventoryHandler(PlayerInventoryHandler _pInHand)
    {
        pInventoryHandler = _pInHand;
    }

    public PlayerInventoryHandler GetPlayerInventoryHandler()
    {
        return pInventoryHandler;
    }


    // <<--------------------------------------------- SET UP AIR TANK CONTROLLER ------------------------------------------- >> //

    public void SetAirTankController(AirTankController _airTankCon)
    {
        airTankCon = _airTankCon;
    }

    public AirTankController GetAirTankController()
    {
        return airTankCon;
    }

}
