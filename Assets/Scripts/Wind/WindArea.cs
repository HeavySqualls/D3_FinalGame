using UnityEngine;

public class WindArea : MonoBehaviour
{
    [Tooltip("If direction is 1, 0, 0 = fromLeft true / -1, 0, 0 = fromLeft false")]
    public bool fromLeft;

    [Tooltip("A value of above 1 will not allow the player to move forward at all.")]
    public float strength = 0f;

    [Tooltip("Is the wind intermittent? - false for no")]
    public bool isBorasWind = false;

    [Tooltip("Which direction is the wind blowing? (-1 for to the left, 1 for to the right)")]
    public Vector3 direction;

    [Tooltip("Does the wind affect the players movement (disable if just the in wind animations are desired)")]
    public bool doesAffectMovement = true;
}
