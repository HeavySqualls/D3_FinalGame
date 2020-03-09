using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindOnUnit : MonoBehaviour
{
    public WindArea windZone;

    private PhysicsObject unit;

    void Start()
    {
        unit = GetComponent<PhysicsObject>();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        WindArea _windArea = coll.gameObject.GetComponent<WindArea>();

        if (_windArea != null)
        {
            print("Entered wind zone");
            windZone = _windArea;

            unit.inWindZone = true;
            unit.WindZoneStats(windZone.direction, windZone.strength, windZone.fromLeft);
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        WindArea _windArea = coll.gameObject.GetComponent<WindArea>();

        if (_windArea != null)
        {
            print("Exit Wind Zone");

            //unit.velocity = Vector3.zero;
            windZone = null;
            unit.inWindZone = false;
        }
    }
}
