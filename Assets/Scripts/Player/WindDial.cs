using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindDial : MonoBehaviour
{
    [SerializeField] Image windDirectionArrow;
    [SerializeField] TextMeshProUGUI windStrengthText;
    [SerializeField] Image magBootIcon;
    [SerializeField] Image windMeterFillBar;
    [SerializeField] GameObject windDial;
    [SerializeField] Image windTypeSymbol;
    [SerializeField] Sprite borasIcon;

    public Image[] healthNodes_lit;
    public bool isHurt;

    public Material idleMat;
    public Material flashMat;

    [SerializeField] float timeSinceInWindZone = 0;
    bool wasInWindZone = false;
    bool inWindZone;
    bool setDirection = false;

    [SerializeField] float timeSinceWasAttacked = 0;
    bool wasAttacked = false;

    [SerializeField] float timeSinceMagBootsUsed = 0;
    bool wasMagBootsOn = false;
    bool bootsSwitchedOn;

    [SerializeField] float timeSinceLastMeterUse = 0;

    [SerializeField] float hideWindDialTime = 5f;

    PlayerController pCon;
    AirTankController airCon;
    Animator animator;

    private void Awake()
    {
        Toolbox.GetInstance().GetPlayerManager().SetWindDial(this);
    }

    void Start()
    {
        pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();
        airCon = Toolbox.GetInstance().GetPlayerManager().GetAirTankController();

        animator = GetComponentInChildren<Animator>();

        windTypeSymbol.enabled = false;
        windDial.SetActive(false);
        SetNodesBackToIdle();
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

    public void SetNodesBackToIdle()
    {
        foreach (Image node in healthNodes_lit)
        {
            node.enabled = false;
        }
    }

    public void HideWindDial()
    {
        windDial.SetActive(false);
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
        if (isHurt)
        {
            windDial.SetActive(true);
            wasAttacked = true;
        }
        else if (wasAttacked && !isHurt && windDial.activeSelf == true && !bootsSwitchedOn && !inWindZone)
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

    float strengthTarget;
    float currentStrength = 0;

    private void WindStrengthCheck()
    {
        inWindZone = pCon.inWindZone;

        if (inWindZone)
        {
            // Enable the direction arrow, then set the direction the arrow will be facing
            windDirectionArrow.enabled = true;

            if (pCon.inBoras)
            {
                windTypeSymbol.enabled = true;
                windTypeSymbol.sprite = borasIcon;
            }

            if (!setDirection)
            {
                if (pCon.windDir == Vector2.right)
                    windDirectionArrow.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                else if (pCon.windDir == Vector2.left)
                    windDirectionArrow.transform.rotation = Quaternion.Euler(0f, 0f, 180f);

                setDirection = true;
            }

            // Check the strength of the wind, and increase the UI number until it reaches the desired target number
            if (pCon.windPwr == 0.7f)
                strengthTarget = 30f;
            else if (pCon.windPwr == 0.6f)
                strengthTarget = 40f;
            else if (pCon.windPwr == 0.5f)
                strengthTarget = 50f;
            else if (pCon.windPwr == 0.4f)
                strengthTarget = 60f;
            else if (pCon.windPwr == 0.3f)
                strengthTarget = 80f;
            else if (pCon.windPwr == 0.2f)
                strengthTarget = 90f;
            else if (pCon.windPwr == 0.1f)
                strengthTarget = 100f;

            if (currentStrength < strengthTarget)
            {
                currentStrength += 1f;
            }

            windStrengthText.text = currentStrength.ToString("F0");
        }
        else
        {
            // If we have left the wind zone, decrease the UI number until it reaches zero, then hide the wind dial
            if (currentStrength > 0)
            {
                currentStrength -= 1f;
                windStrengthText.text = currentStrength.ToString("F0");
            }
            else
            {
                if (windTypeSymbol.enabled)
                {
                    windTypeSymbol.enabled = false;
                }

                windDirectionArrow.enabled = false;
                windStrengthText.text = "00";
                setDirection = false;
            }
        }

        animator.SetBool("inWind", inWindZone);
    }

    private void MagBootCheck()
    {
        bootsSwitchedOn = pCon.magBootsOn;

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
