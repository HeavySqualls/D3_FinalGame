using UnityEngine;

public class Controls : MonoBehaviour
{
    public string xMove;
    public string jump;
    public string punch;
    public string launch;
    public string magBoots;

    public void KeyboardControls()
    {
        xMove = "Horizontal";
        jump = "Jump";
        punch = "Punch";
        launch = "Launch";
        magBoots = "MagBoots";
    }

    public void ControllerControls()
    {
        xMove = "Cont_Hor";
        jump = "Jump";
        punch = "Punch";
        launch = "Launch";
        magBoots = "MagBoots";
    }
}
