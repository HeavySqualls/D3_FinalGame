using UnityEngine;

public class Controls : MonoBehaviour
{
    public string xMove;
    public string jump;
    public string punch;
    public string launch;
    public string magBoots;

    // Input Actions
    PlayerInputActions inputActions;

    // Movement
    Vector2 movementInput;

    // Jump


    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.PlayerControls.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
    }

    public void KeyboardControls()
    {
        //xMove = movementInput.x;
        xMove = "Horizontal";
        jump = "Jump";
        punch = "Punch";
        launch = "Launch";
        magBoots = "MagBoots";
    }

    public void ControllerControls()
    {
        //xMove = "Cont_Hor";
        jump = "Jump";
        punch = "Punch";
        launch = "Launch";
        magBoots = "MagBoots";
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
