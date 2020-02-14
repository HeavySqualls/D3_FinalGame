using UnityEngine;

public class Controls : MonoBehaviour
{
    public string xMove;
    public string jump;
    public string punch;
    public string ability_1;
    public string ability_2;
    public string ability_3;
    public string magBoots;
    public string crouch;
    public string interact;

    private void Start()
    {
        jump = "Jump";
        punch = "Punch";
        ability_1 = "Ability_1";
        ability_2 = "Ability_2";
        ability_3 = "Ability_3";
        magBoots = "MagBoots";
        crouch = "Crouch";
        interact = "Interact";
    }

    public void KeyboardMovement()
    {
        xMove = "Horizontal";
    }

    public void ControllerMovement()
    {
        xMove = "Cont_Hor";
    }
}
