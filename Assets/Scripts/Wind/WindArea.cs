using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    [Tooltip("If direction is 1, 0, 0 = fromLeft true / -1, 0, 0 = fromLeft false")]
    public bool fromLeft;

    [Tooltip("A value of above 1 will not allow the player to move forward at all.")]
    public float strength = 0f;

    [Tooltip("")]
    public float windRatio = 0f;

    public Vector3 direction;
}
