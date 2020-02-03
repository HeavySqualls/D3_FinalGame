﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private PhysicsObject unit;

    void OnTriggerEnter2D(Collider2D coll)
    {
        // DEATH 

        if (coll.gameObject.GetComponent<PhysicsObject>())
        {
            print("Unit Dead");

            unit = coll.gameObject.GetComponent<PhysicsObject>();

            if (unit.GetComponent<PlayerController>())
            {
                unit.gameObject.transform.position = unit.gameObject.GetComponent<PlayerController>().respawnZone.position;
                StartCoroutine(unit.gameObject.GetComponent<PlayerFeedback>().IFlashRed());
            }
            else if (unit.GetComponent<EnemyController>())
            {
                unit.GetComponent<EnemyController>().Killed();
            }
        }
    }
}