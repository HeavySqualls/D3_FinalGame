using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public delegate void OnPlayerKilled(); // Declare the delegate
    public static event OnPlayerKilled onPlayerKilled; // Create an event

    public delegate void OnResetLevelObjects(); // Declare the delegate
    public static event OnResetLevelObjects onResetLevelObjects; // Create an event

    public static void PlayerWasKilled()
    {
        if (onPlayerKilled != null)
        {
            onPlayerKilled(); // Invoke an Event
        }
    }

    public static void ResetLevelObjects()
    {
        if (onResetLevelObjects != null)
        {
            onResetLevelObjects(); // Invoke an Event
        }
    }
}
