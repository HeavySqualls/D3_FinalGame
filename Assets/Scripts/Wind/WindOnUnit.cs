using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindOnUnit : MonoBehaviour
{
    public WindArea windZone;
    public Transform respawnZone;

    public PhysicsObject unit;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "WindZone")
        {
            print("Entered wind zone");
            windZone = coll.gameObject.GetComponent<WindArea>();

            unit.inWindZone = true;
            unit.WindZoneStats(windZone.direction, windZone.strength, windZone.fromLeft);
        }

        if (coll.gameObject.tag == "DeathZone")
        {
            print("Dead");

            if (unit.GetComponent<PlayerController>())
            {
                unit.gameObject.transform.position = respawnZone.position;
                StartCoroutine(unit.IFlashRed(this.GetComponent<SpriteRenderer>()));
            }

            if (unit.GetComponent<EnemyController>())
            {
                unit.GetComponent<EnemyController>().Killed();
            }
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
