using UnityEngine;

public class EventHandler : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            EventMan.RaiseOnReset(); // Function call to invoke event
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            EventMan.RaiseOnReStart(); // Function call to invoke event
        }
    }
}
