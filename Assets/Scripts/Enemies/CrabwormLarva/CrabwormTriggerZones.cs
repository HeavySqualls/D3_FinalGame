using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabwormTriggerZones : MonoBehaviour
{
    [Tooltip("Drag in the crabworm that will react to this trigger zone here.")]
    [SerializeField] CrabwormLarvaController crabwormCon;

    public bool isActive = true;

    private void Start()
    {
        Toolbox.GetInstance().GetLevelManager().AddcwTriggerZone(this);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive)
        {
            if (other.gameObject.tag == "Player")
            {
                crabwormCon.AttackCall(other.gameObject);
                isActive = false;
            }
        }
    }
}
