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
        // WIND

        if (coll.gameObject.tag == "WindZone")
        {
            print("Entered wind zone");
            windZone = coll.gameObject.GetComponent<WindArea>();

            unit.inWindZone = true;
            unit.WindZoneStats(windZone.direction, windZone.strength, windZone.fromLeft);
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "WindZone")
        {
            unit.velocity = Vector3.zero;
            windZone = null;
            unit.inWindZone = false;
        }
    }
}
