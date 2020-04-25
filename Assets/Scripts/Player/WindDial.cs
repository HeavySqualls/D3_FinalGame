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
    bool wasInWindZone = false;
    bool inWindZone;

    [SerializeField] float timeSinceWasAttacked = 0;
    bool wasAttacked = false;

    [SerializeField] float timeSinceMagBootsUsed = 0;
    bool wasMagBootsOn = false;
    bool bootsSwitchedOn;

    [SerializeField] float timeSinceLastMeterUse = 0;

    [SerializeField] float hideWindDialTime = 5f;
    [SerializeField] Image windDialImage;
    PlayerController pCon;
    PlayerHealthSystem pHealth;
    AirTankController airCon;
    Animator animator;

    void Start()
    {       
        pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();
        airCon = Toolbox.GetInstance().GetPlayerManager().GetAirTankController();
        pHealth = Toolbox.GetInstance().GetPlayerManager().GetPlayerHealthSystem();
        pHealth.AssignWindDialSprite(windDialImage);
        animator = GetComponentInChildren<Animator>();

        windDial.SetActive(false);
    }

    void Update()
    {

        TrackTimeSinceHealed();
        TrackTimeSinceInWindZone();
        TrackTimeSinceLastUsedMeter();
        TrackTimeSinceDisabledMagBoots();

        MagBootCheck();
        WindStrengthCheck();
        WindMeterFillAmount();
    }

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

    private void TrackTimeSinceHealed()
    {
        if (pHealth.isHurt)
        {
            windDial.SetActive(true);
            wasAttacked = true;
        }
        else if (wasAttacked && !pHealth.isHurt && windDial.activeSelf == true && !bootsSwitchedOn && !inWindZone)
        {
            timeSinceWasAttacked += Time.deltaTime;

            if (timeSinceWasAttacked >= hideWindDialTime)
            {
                timeSinceInWindZone = 0;
                timeSinceMagBootsUsed = 0;
                wasAttacked = false;

                if (!airCon.attacked && !bootsSwitchedOn && !inWindZone)
                {
                    windDial.SetActive(false);
                }
            }
        }
    }

    private void TrackTimeSinceLastUsedMeter()
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

            //windStrengthText.text = (pCon.windPwr * 100f).ToString() + " KM";
            if (pCon.windPwr == 0.7f)
                windStrengthText.text = "30 KM";
            else if (pCon.windPwr == 0.6f)
                windStrengthText.text = "40 KM";
            else if (pCon.windPwr == 0.5f)
                windStrengthText.text = "50 KM";
            else if (pCon.windPwr == 0.4f)
                windStrengthText.text = "60 KM";
            else if (pCon.windPwr == 0.3f)
                windStrengthText.text = "80 KM";
            else if (pCon.windPwr == 0.2f)
                windStrengthText.text = "90 KM";
            else if (pCon.windPwr == 0.1f)
                windStrengthText.text = "100 KM";
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
