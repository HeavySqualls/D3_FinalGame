using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSystem : MonoBehaviour
{
    [Header("Health Variables:")]
    [SerializeField] float damageCooldownTime = 1f;
    public bool isHurt = false;
    bool isDamageCooldown = false;

    [Space]
    [Header("Red Flash:")]
    [Tooltip("The length of time the object will flash.")]
    public float flashDuration = 5f;
    [SerializeField] float currentFlashDelay;
    [SerializeField] float phase1FlashDelay = 1f;
    [SerializeField] float phase2FlashDelay = 0.5f;
    [SerializeField] float phase3FlashDelay = 0.35f;
    private IEnumerator flashCoroutine;

    [Space]
    [Header("Injury Phases:")]
    [SerializeField] int currentPhase;
    int hurtPhase0 = 0;
    int hurtPhase1 = 1;
    int hurtPhase2 = 2;
    int hurtPhase3 = 3;

    [Space]
    [Header("Respawn:")]
    public SpawnZone spawnZone;
    public float respawnTime;
    public bool isDead = false;

    [Space]
    [Header("References:")]
    WindDial windDial;
    PlayerController pCon;
    PlayerFeedback pFeedback;
    PlayerAudioController pAudio;
    Animator animator;

    private void OnEnable()
    {
        SpawnManager.onResetLevelObjects += RespawnPlayer;
    }

    private void OnDisable()
    {
        SpawnManager.onResetLevelObjects -= RespawnPlayer;
    }

    void Start()
    {
        pCon = GetComponent<PlayerController>();
        pAudio = GetComponent<PlayerAudioController>();
        pFeedback = GetComponent<PlayerFeedback>();
        windDial = Toolbox.GetInstance().GetPlayerManager().GetWindDial();

        animator = GetComponent<Animator>();

        currentPhase = hurtPhase0;

        flashCoroutine = IInjuredFlashRed();
    }

    private void Update()
    {
        windDial.isHurt = isHurt;
    }

    // ---- HANDLE DAMAGE ---- //

    public void TakeFallDamage()
    {
        if (currentPhase < hurtPhase2)
        {
            currentPhase = hurtPhase2;
        }

        TakeDamage(Vector2.zero, 0, 0, 0, 0, true);
    }

    public void TakeDamage(Vector2 _hitDirection, float _damage, float _knockBack, float _knockUp, float _stunTime, bool _isFallDamage)
    {
        if (!isDead && !isDamageCooldown)
        {
            pAudio.PlayHurtSound();
            pFeedback.HurtShake();
            StopCoroutine("IInjuredFlashRed");

            if (currentPhase == hurtPhase0)
            {
                print("Phase 1");
                currentPhase = hurtPhase1;
                currentFlashDelay = phase1FlashDelay;
            }
            else if (currentPhase == hurtPhase1)
            {
                print("Phase 2");
                currentPhase = hurtPhase2;
                currentFlashDelay = phase2FlashDelay;
            }
            else if (currentPhase == hurtPhase2)
            {
                print("Phase 3");
                currentPhase = hurtPhase3;
                currentFlashDelay = phase3FlashDelay;
            }
            else if (currentPhase == hurtPhase3)
            {
                print("PLAYER IS DEAD!");
                KillPlayer();
                return;
            }

            if (!_isFallDamage)
            {
                animator.SetTrigger("isHurt");
            }

            StartCoroutine("IInjuredFlashRed");

            if (!pCon.isHit)
            {
                StartCoroutine(pCon.PlayerKnocked(_hitDirection, _knockBack, _knockUp, _stunTime));
            }

            StartCoroutine(DamageCooldownTimer());
        }    
    }

    IEnumerator DamageCooldownTimer()
    {
        isDamageCooldown = true;

        yield return new WaitForSeconds(damageCooldownTime);

        isDamageCooldown = false;

        yield break;
    }

    public void KillPlayer()
    {
        print("player killed");
        isDead = true;
        pAudio.PlayDeathSound();
        pCon.animator.SetBool("isDead", isDead);
        pCon.DisablePlayerController();

        StopCoroutine("IInjuredFlashRed");

        windDial.SetNodesBackToIdle();
        SpawnManager.PlayerWasKilled();
    }

    public void RespawnPlayer()
    { 
        isDead = false;
        pCon.animator.SetBool("isDead", isDead);

        if (spawnZone != null)
        {
            print("spawn player");
            spawnZone.RespawnObject(gameObject);
        }
        else
            Debug.LogError("No respawn location assigned!");

        pCon.EnablePlayerController();
        currentPhase = hurtPhase0;
    }

    public IEnumerator IInjuredFlashRed()
    {
        print("Flash");
        isHurt = true;
        windDial.isHurt = isHurt;

        if (currentPhase == hurtPhase3)
        {
            print("Hurt Phase 3");
            currentFlashDelay = phase3FlashDelay;

            for (float i = 0; i < flashDuration; i += currentFlashDelay)
            {
                foreach (Image node in windDial.healthNodes)
                {
                    if (node.color == windDial.idleColor)
                        node.color = windDial.flashColor;
                    else if (node.color == windDial.flashColor)
                        node.color = windDial.idleColor;
                }

                yield return new WaitForSeconds(currentFlashDelay);
            }

            windDial.SetNodesBackToIdle();
            currentPhase = hurtPhase2;
        }
        
        if (currentPhase == hurtPhase2)
        {
            print("Hurt Phase 2");

            currentFlashDelay = phase2FlashDelay;

            for (float i = 0; i < flashDuration; i += currentFlashDelay)
            {
                for (int node = 0; node < windDial.healthNodes.Length -1; node++)
                {
                    if (windDial.healthNodes[node].color == windDial.idleColor)
                        windDial.healthNodes[node].color = windDial.flashColor;
                    else if (windDial.healthNodes[node].color == windDial.flashColor)
                        windDial.healthNodes[node].color = windDial.idleColor;
                }

                yield return new WaitForSeconds(currentFlashDelay);
            }

            windDial.SetNodesBackToIdle();
            currentPhase = hurtPhase1;
        }
        
        if (currentPhase == hurtPhase1)
        {
            print("Phase 1");

            currentFlashDelay = phase1FlashDelay;

            for (float i = 0; i < flashDuration; i += currentFlashDelay)
            {
                for (int node = 0; node < windDial.healthNodes.Length - 2; node++)
                {
                    if (windDial.healthNodes[node].color == windDial.idleColor)
                        windDial.healthNodes[node].color = windDial.flashColor;
                    else if (windDial.healthNodes[node].color == windDial.flashColor)
                        windDial.healthNodes[node].color = windDial.idleColor;
                }

                yield return new WaitForSeconds(currentFlashDelay);
            }
        }

        currentPhase = 0;

        windDial.SetNodesBackToIdle();

        isHurt = false;
        yield break;
    }
}

//print("Flash");
//isHurt = true;

//        for (float i = 0; i<flashDuration; i += currentFlashDelay)
//        {
//            if (healthNode1.color == Color.white)
//            {
//                healthNode1.color = Color.red;
//            }
//            else if (healthNode1.color == Color.red)
//            {
//                healthNode1.color = Color.white;
//            }

//            yield return new WaitForSeconds(currentFlashDelay);
//        }

//        if (currentPhase == hurtPhase3)
//        {
//            print("Phase 3 to 2");
//currentPhase = hurtPhase2;
//            currentFlashDelay = phase2FlashDelay;

//            for (float i = 0; i<flashDuration; i += currentFlashDelay)
//            {
//                if (healthNode1.color == Color.white)
//                {
//                    healthNode1.color = Color.red;
//                }
//                else if (healthNode1.color == Color.red)
//                {
//                    healthNode1.color = Color.white;
//                }

//                yield return new WaitForSeconds(currentFlashDelay);
//            }
//        }

//        if (currentPhase == hurtPhase2)
//        {
//            print("Phase 2 to 1");
//currentPhase = hurtPhase1;
//            currentFlashDelay = phase1FlashDelay;

//            for (float i = 0; i<flashDuration; i += currentFlashDelay)
//            {
//                if (healthNode1.color == Color.white)
//                {
//                    healthNode1.color = Color.red;
//                }
//                else if (healthNode1.color == Color.red)
//                {
//                    healthNode1.color = Color.white;
//                }

//                yield return new WaitForSeconds(currentFlashDelay);
//            }
//        }

//        currentPhase = 0;

//        healthNode1.color = Color.white;

//        isHurt = false;
//        yield break;
