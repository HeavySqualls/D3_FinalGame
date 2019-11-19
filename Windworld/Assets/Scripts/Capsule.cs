using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    public bool inWindZone = false;
    public WindArea windZone;

    Rigidbody rb;

    public float lifeTime = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                Destruction();
            }
        }

        if (this.transform.position.y <= -20)
        {
            Destruction();
        }
    }

    void FixedUpdate()
    {
        if (inWindZone)
        {
            rb.AddForce(windZone.direction * windZone.strength);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "WindZone")
        {
            windZone = coll.gameObject.GetComponent<WindArea>();
            inWindZone = true;
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "WindZone")
        {
            windZone = null;
            inWindZone = false;
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.name == "DeathZone")
        {
            Destruction();
        }
    }

    void Destruction()
    {
        Destroy(gameObject);
    }
}
