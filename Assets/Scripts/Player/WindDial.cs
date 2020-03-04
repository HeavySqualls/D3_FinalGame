using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindDial : MonoBehaviour
{
    [SerializeField] Text windDirectionText;
    [SerializeField] Text windStrengthText;
    [SerializeField] Image magBootIcon;
    [SerializeField] Image windMeterFillBar;
    [SerializeField] GameObject windDial;

    [SerializeField] float timeSinceInWindZone = 0;
    [SerializeField] float timeSinceLastMeterUse = 0;
    [SerializeField] float timeSinceMagBootsUsed = 0;
    [SerializeField] float hideWindDialTime = 5f;

    bool bootsSwitchedOn;
    bool inWindZone;

    PlayerController pCon;
    AirTankController airCon;
    Animator animator;

    void Start()
    {       
        pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();
        airCon = Toolbox.GetInstance().GetPlayerManager().GetAirTankController();
        animator = GetComponentInChildren<Animator>();

        windDial.SetActive(false);
    }

    void Update()
    {
        TrackTimeSinceInWindZone();
        TrackTimeSinceLastAttacked();
        TrackTimeSinceDisabledMagBoots();

        MagBootCheck();
        WindStrengthCheck();
        WindMeterFillAmount();
    }

    bool wasInWindZone = false;

    private void TrackTimeSinceInWindZone()
    {
        if (pCon.inWindZone)
        {
            windDial.SetActive(true);
            wasInWindZone = true;
        }
        else if (wasInWindZone && !inWindZone && windDial.activeSelf == true && !bootsSwitchedOn)
        {
            timeSinceInWindZone += Time.deltaTime;

            if (timeSinceInWindZone >= hideWindDialTime)
            {
                timeSinceInWindZone = 0;
                timeSinceMagBootsUsed = 0;
                wasInWindZone = false;

                if (!airCon.attacked && !bootsSwitchedOn)
                {
                    windDial.SetActive(false);
                }
            }
        }
    }


    private void TrackTimeSinceLastAttacked()
    {
        if (airCon.attacked && !inWindZone && !bootsSwitchedOn)
        {
            if (windDial.activeSelf == false)
            {
                windDial.SetActive(true);
            }

            timeSinceLastMeterUse += Time.deltaTime;

            if (timeSinceLastMeterUse >= hideWindDialTime)
            {
                timeSinceInWindZone = 0;
                timeSinceLastMeterUse = 0;
                timeSinceMagBootsUsed = 0;
                airCon.attacked = false;

                windDial.SetActive(false);
            }
        }
    }

    bool wasMagBootsOn = false;

    private void TrackTimeSinceDisabledMagBoots()
    {
        if (wasMagBootsOn && !bootsSwitchedOn && !inWindZone)
        {
            timeSinceMagBootsUsed += Time.deltaTime;

            if (timeSinceMagBootsUsed >= hideWindDialTime)
            {
                timeSinceInWindZone = 0;
                timeSinceLastMeterUse = 0;
                timeSinceMagBootsUsed = 0;
                windDial.SetActive(false);
                wasMagBootsOn = false;
            }
        }
    }

    private void WindMeterFillAmount()
    {
        if (windMeterFillBar.fillAmount != airCon.airInCanPercent)
        {
            windMeterFillBar.fillAmount = airCon.airInCanPercent;
        }
    }

    private void WindStrengthCheck()
    {
        inWindZone = pCon.inWindZone;

        if (inWindZone)
        {
            if (pCon.windDir == Vector2.right)
                windDirectionText.text = " > ";
            else if (pCon.windDir == Vector2.left)
                windDirectionText.text = " < ";

            windStrengthText.text = (pCon.windPwr * 100f).ToString() + " KM";
        }
        else
        {
            windDirectionText.text = " - ";
            windStrengthText.text = "0 KM";
        }

        animator.SetBool("inWind", inWindZone);
    }

    private void MagBootCheck()
    {
        bootsSwitchedOn = pCon.magBootsOn;

        // TODO: Add in an audio queue
        if (bootsSwitchedOn)
        {
            magBootIcon.enabled = true;

            if (windDial.activeSelf == false)
            {
                windDial.SetActive(true);
                wasMagBootsOn = true;
            }
        }
        else if (!bootsSwitchedOn && magBootIcon.enabled == true)
        {
            magBootIcon.enabled = false;
        }
    }
}
