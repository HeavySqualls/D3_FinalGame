using UnityEngine;

public class Controls : MonoBehaviour
{
    public string xMoveKeys;
    public string xMoveController;
    public string jump;
    public string punch;
    public string ability_1;
    public string ability_2;
    public string ability_3;
    public string magBoots;
    public string crouch;
    public string interact;
    public string inventory;
    public string quickLoot;
    public string pauseMenu;

    public static bool IsLeft, IsRight, IsUp, IsDown;
    private float _LastX, _LastY;

    private void Start()
    {
        ControllerMovement();
        KeyboardMovement();
        jump = "Jump";
        punch = "Punch";
        ability_1 = "Ability_1";
        ability_2 = "Ability_2";
        ability_3 = "Ability_3";
        magBoots = "MagBoots";
        crouch = "Crouch";
        interact = "Interact";
        inventory = "Inventory";
        quickLoot = "QuickLoot";
        pauseMenu = "PauseMenu";
    }

    // ---- FOR DPAD INPUT ----- //
    private void Update()
    {
        // Reference DPad X & Y axis' float number. These axis's only return y.1, y.-1, x.1, x.-1, and from there we are able 
        // to determine which direction was the last input, and whether than input has been changed or not. We use this information
        // to set booleans to result in the same return value as a OnButtonDown input. 
        float x = Input.GetAxis("DPad X");
        float y = Input.GetAxis("DPad Y");

        IsLeft = false;
        IsRight = false;
        IsUp = false;
        IsDown = false;

        // If the last input on the x axis is NOT the same as the new float from the X axis, set the proper direction to true
        if (_LastX != x)
        {
            if (x == -1)
                IsLeft = true;
            else if (x == 1)
                IsRight = true;
        }

        // If the last input on the y axis is NOT the same as the new float from the Y axis, set the proper direction to true
        if (_LastY != y)
        {
            if (y == -1)
                IsDown = true;
            else if (y == 1)
                IsUp = true;
        }

        _LastX = x;
        _LastY = y;
    }

    public void KeyboardMovement()
    {
        xMoveKeys = "Horizontal";
    }

    public void ControllerMovement()
    {
        xMoveController = "Cont_Hor";
    }
}
