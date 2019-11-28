using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindOnUnit : MonoBehaviour
{
    private bool affectUnit = true;
    public WindArea windZone;
    public Transform respawnZone;

    private PlayerController pCon;

    void Start()
    {
        pCon = GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "WindZone")
        {
            print("Entered wind zone");
            windZone = coll.gameObject.GetComponent<WindArea>();
            pCon.inWindZone = true;
            pCon.WindZoneStats(windZone.direction, windZone.strength);
        }

        if (coll.gameObject.tag == "DeathZone")
        {
            print("Dead");
            pCon.gameObject.transform.position = respawnZone.position;
            StartCoroutine(pCon.IFlashRed(this.GetComponent<SpriteRenderer>()));
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "WindZone")
        {
            pCon.rb2d.velocity = Vector3.zero;
            windZone = null;
            pCon.inWindZone = false;
        }
    }
}
