using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerHealthSystem : MonoBehaviour
{
    Tween fadeTween;

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
    private IEnumerator healthNodeFlashRoutine;

    [SerializeField] Image injuredFlashImage;
    [SerializeField] float injured_1Alpha = 0.35f;
    [SerializeField] float injured_2Alpha = 0.65f;
    [SerializeField] float injured_3Alpha = 1f;
    private float currentAlphaTarget;
    private float currentDuration;
    private IEnumerator injuredFlashRoutine;

    [Space]
    [Header("Injury Phases:")]
    [SerializeField] int currentPhase;
    [SerializeField] Color idleColor;
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
        healthNodeFlashRoutine = IInjuredFlashRed();
    }

    private bool isFlashing = false;

    private void Update()
    {
        windDial.isHurt = isHurt;

        //if (isHurt && !isFlashing)
        //{
        //    if (injuredFlashImage.color.a == 0)
        //    {
        //        if (injuredFlashRoutine != null)
        //        {
        //            StopCoroutine(injuredFlashRoutine);
        //            injuredFlashRoutine = null;
        //        }

        //        injuredFlashRoutine = FadeImage(injuredFlashImage, currentAlphaTarget, 1f);
        //        StartCoroutine(injuredFlashRoutine);
        //    }
        //    else if (injuredFlashImage.color.a == currentAlphaTarget)
        //    {
        //        if (injuredFlashRoutine != null)
        //        {
        //            StopCoroutine(injuredFlashRoutine);
        //            injuredFlashRoutine = null;
        //        }

        //        injuredFlashRoutine = FadeImage(injuredFlashImage, 0, 1f);
        //        StartCoroutine(injuredFlashRoutine);
        //    }
        //}
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
            spawnZone.RespawnObject(gameObject);
        }
        else
            Debug.LogError("No respawn location assigned!");

        pCon.EnablePlayerController();
        currentPhase = hurtPhase0;
    }

    private void CheckAndTriggerFadeLoop()
    {
        if (isHurt)
        {
            if (fadeTween != null)
            {
                fadeTween.Kill();
                fadeTween = null;
            }
            FadeInImage(currentAlphaTarget, currentDuration);
        }
    }

    private void FadeOutImage(float target, float duration)
    {
        fadeTween = injuredFlashImage.DOFade(0, duration).OnComplete(CheckAndTriggerFadeLoop);
    }

    private void FadeInImage(float target, float duration)
    {
        fadeTween = injuredFlashImage.DOFade(target, duration).OnComplete(() => FadeOutImage(target, duration));
    }

    //IEnumerator FadeImage(Image _image, float target, float duration)
    //{
        //print("start flashing");
        //isFlashing = true;

        //float totalTime = 0;
        //bool isIncreasing;

        //if (_image.color.a == target)
        //    isIncreasing = true;
        //else
        //    isIncreasing = false;

        //while (isFlashing)
        //{
        //    float totalChange = target - _image.color.a;
        //    float changePerSecond = Mathf.Abs(totalChange) / duration;

        //    if (isIncreasing)
        //    {
        //        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, Mathf.Lerp(0f, target, changePerSecond));
        //    }
        //    else if (!isIncreasing)
        //    {
        //        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, Mathf.Lerp(target, 0f, changePerSecond));
        //    }

        //    yield return null;

        //    totalTime += Time.deltaTime;
        //}

        //_image.color = new Color(_image.color.r, _image.color.g, _image.color.b, target);

        //isFlashing = false;
    //}

    private void Flash(float _flashDelay, int _arraySubtractInt, float _vignetteAlpha)
    {
        for (int node = 0; node < windDial.healthNodes.Length - _arraySubtractInt; node++)
        {
            // Change the health node material 
            if (windDial.healthNodes[node].material == windDial.idleMat)
            {
                windDial.healthNodes[node].material = windDial.flashMat;
            }
            else if (windDial.healthNodes[node].material == windDial.flashMat)
            {
                windDial.healthNodes[node].material = windDial.idleMat;
            }
        }
    }

    public IEnumerator IInjuredFlashRed()
    {
        isFlashing = false;
        isHurt = true;
        windDial.isHurt = isHurt;

        if (currentPhase == hurtPhase3)
        {
            print("Hurt Phase 3");
            currentFlashDelay = phase3FlashDelay;
            currentAlphaTarget = injured_3Alpha;
            currentDuration = 0.6f;
            CheckAndTriggerFadeLoop();

            for (float i = 0; i < flashDuration; i += currentFlashDelay)
            {
                Flash(currentFlashDelay, 0, injured_3Alpha);
                yield return new WaitForSeconds(currentFlashDelay);
                //FadeInImage(injured_3Alpha, 0.7f);
                isFlashing = false;
            }   

            windDial.SetNodesBackToIdle();
            currentPhase = hurtPhase2;
        }
        
        if (currentPhase == hurtPhase2)
        {
            print("Hurt Phase 2");

            currentFlashDelay = phase2FlashDelay;
            currentAlphaTarget = injured_2Alpha;
            currentDuration = 0.7f;
            CheckAndTriggerFadeLoop();

            for (float i = 0; i < flashDuration; i += currentFlashDelay)
            {
                Flash(currentFlashDelay, 1, injured_2Alpha);
                yield return new WaitForSeconds(currentFlashDelay);

                isFlashing = false;
            }

            windDial.SetNodesBackToIdle();
            currentPhase = hurtPhase1;
        }
        
        if (currentPhase == hurtPhase1)
        {
            print("Phase 1");

            currentFlashDelay = phase1FlashDelay;
            currentAlphaTarget = injured_1Alpha;
            currentDuration = 0.8f;
            CheckAndTriggerFadeLoop();

            for (float i = 0; i < flashDuration; i += currentFlashDelay)
            {
                Flash(currentFlashDelay, 2, injured_1Alpha);
                yield return new WaitForSeconds(currentFlashDelay);

                isFlashing = false;
            }
        }

        currentPhase = 0;

        windDial.SetNodesBackToIdle();

        isHurt = false;
        yield break;
    }
}
