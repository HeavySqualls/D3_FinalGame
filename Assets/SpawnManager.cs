using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public delegate void OnResetLevelObjects(); // Declare the delegate
    public static event OnResetLevelObjects onResetLevelObjects; // Create an event

    public static void ResetLevelObjects()
    {
        onResetLevelObjects();
    }
}
