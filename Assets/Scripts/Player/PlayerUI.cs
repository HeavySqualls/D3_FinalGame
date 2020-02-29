using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] ParticleSystem partSyst;
    [SerializeField] float airInCanStart;
    public float airInCan;
    [SerializeField] bool isCharging = false;

    PlayerController pCon;

    void Start()
    {
        partSyst = GetComponentInChildren<ParticleSystem>();
        pCon = GetComponent<PlayerController>();
        airInCan = airInCanStart;
    }

    void Update()
    {
        if (airInCan < airInCanStart && pCon.inWindZone && !isCharging)
        {
            partSyst.Play();
            isCharging = true;
        }
        else if (isCharging && airInCan >= airInCanStart)
        {
            airInCan = airInCanStart;
            partSyst.Stop();
            isCharging = false;
        }
        else if (isCharging)
        {
            airInCan += 2f * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            airInCan -= 1;
        }
    }
}
