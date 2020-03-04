using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirTankController : MonoBehaviour
{
    public bool attacked = false; // to determine when the player attacks - for the wind dial to know when to enable/disable
    public float airInCan;
    public float airInCanPercent;
    [SerializeField] float airInCanStart;

    bool isCharging = false;

    ParticleSystem partSyst;
    PlayerController pCon;

    private void Awake()
    {
        Toolbox.GetInstance().GetPlayerManager().SetAirTankController(this);
    }

    void Start()
    {
        partSyst = GetComponentInChildren<ParticleSystem>();
        pCon = GetComponent<PlayerController>();
        airInCan = airInCanStart;
        UpdateAirPercent();
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
            UpdateAirPercent();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            airInCan -= 1;
        }
    }

    public void UseAirInTank(float _airUsage)
    {
        attacked = true;

        airInCan -= _airUsage;

        UpdateAirPercent();
    }

    private void UpdateAirPercent()
    {
        airInCanPercent = airInCan / (airInCanStart);
    }
}
