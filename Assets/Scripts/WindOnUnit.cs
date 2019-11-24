using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindOnUnit : MonoBehaviour
{

    public bool inWindZone = false;
    public WindArea windZone;
    public Transform respawnZone;

    private PlayerController pCon;

    void Start()
    {
        pCon = GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        if (inWindZone && pCon.magBootsOn == false)
        {
            pCon.rb2d.AddForce(windZone.direction * windZone.strength);
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "WindZone")
        {
            print("Entered wind zone");
            windZone = coll.gameObject.GetComponent<WindArea>();
            inWindZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "WindZone")
        {
            pCon.rb2d.velocity = Vector3.zero;
            windZone = null;
            inWindZone = false;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.name == "DeathZone")
        {
            pCon.gameObject.transform.position = respawnZone.position;
        }
    }
}
