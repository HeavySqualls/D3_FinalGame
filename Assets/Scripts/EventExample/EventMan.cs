using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMan : MonoBehaviour
{
    public delegate void OnRest(); // Declare a Delegate
    public static event OnRest onReset; // Create an event
    public delegate void OnReStart(); // Declare a Delegate
    public static event OnReStart onRestart; // Declare an event
    
    public static void RaiseOnReset()
    {
        if (onReset != null)
        {
            onReset(); // Invoke an Event
        }
    }

    public static void RaiseOnReStart()
    {
        if (onRestart != null)
        {
            onRestart(); // Invoke an Event
        }
    }
}
