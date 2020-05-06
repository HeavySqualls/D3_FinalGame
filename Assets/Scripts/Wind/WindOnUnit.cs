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
            windZone = _windArea;

            unit.inWindZone = true;
            unit.WindZoneStats(windZone.direction, windZone.strength, windZone.fromLeft, windZone.doesAffectMovement, windZone.isBorasWind);
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        WindArea _windArea = coll.gameObject.GetComponent<WindArea>();

        if (_windArea != null)
        {
            windZone = null;
            unit.inWindZone = false;
        }
    }
}
