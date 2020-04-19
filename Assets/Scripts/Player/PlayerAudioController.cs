using System.Collections;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [Header("Audio Effects:")]
    public AudioSource effectSource;

    [Header("Jump:")]
    [SerializeField] AudioClip jumpSound;
    [SerializeField] float jumpVolume = 0.5f;

    [Header("Ledge Climb:")]
    [SerializeField] AudioClip climbSound;
    [SerializeField] float climbVolume = 0.35f;

    [Header("Landing:")]
    [SerializeField] AudioClip landSound;
    [SerializeField] float landVolume = 0.3f;
    [SerializeField] float hardLandVolume = 0.7f;
    [SerializeField] float heavyLandVolume = 1f;

    [Header("Skidding:")]
    [SerializeField] AudioClip slideSound;
    [SerializeField] float slideVolume = 0.4f;

    [Header("Combat:")]
    [SerializeField] AudioClip hurtSound;
    [SerializeField] float hurtVolume = 0.4f;
    [SerializeField] AudioClip deathSound;
    [SerializeField] float deathVolume = 0.4f;

    [SerializeField] AudioClip connectSound;

    [SerializeField] AudioClip punch1Sound;
    [SerializeField] float punch1Volume = 0.4f;
    [SerializeField] float connect1Volume = 0.4f;

    [SerializeField] AudioClip punch2Sound;
    [SerializeField] float punch2Volume = 0.4f;
    [SerializeField] float connect2Volume = 0.4f;

    [SerializeField] AudioClip punch3Sound;
    [SerializeField] float punch3Volume = 0.4f;
    [SerializeField] float connect3Volume = 0.4f;

    [Header("Wind Detection:")]
    [SerializeField] AudioClip enterWindWarningSound;
    [SerializeField] AudioClip exitWindWarningSound;
    [SerializeField] float windWarningVolume = 0.4f;
    bool windZoneWarningPlayed = false;

    [Space]
    [Header("Wall/Surface Sliding:")]
    [SerializeField] AudioSource wallSlidingSource;
    [SerializeField] AudioClip wallSurfaceSlideSound;
    [SerializeField] float wallSurfaceSlideVolume = 0.4f;
    bool isSlideSoundPlaying = false;

    [Space]
    [Header("Magnetic Boots:")]
    [SerializeField] AudioSource magBootsSource;
    [SerializeField] AudioClip magBootsSound;
    [SerializeField] AudioClip magBootsSwitchSound;
    [SerializeField] float magBootsVolume = 0.4f;
    [SerializeField] float magBootsSwitchVolume = 0.6f;
    bool isPlayingMagBoots = false;

    [Space]
    [Header("Foot Steps:")]
    [SerializeField] AudioSource footStepsSource;
    [SerializeField] AudioClip footStepsNormal;
    [SerializeField] AudioClip footStepsInWind;
    [SerializeField] float desiredMaxFootVolume = 0.5f;
    [SerializeField] float volumeIncrementation = 0.1f;
    [SerializeField] float incrementTime = 0.1f;
    [SerializeField] float volumeDecrementation = 0.1f;
    [SerializeField] float decrementTime = 0.1f;
    bool isPlayingFootsteps;

    [Space]
    [Header("References:")]
    PlayerController pCon;

    private void Start()
    {
        pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();
        footStepsSource.clip = footStepsNormal;

        magBootsSource.clip = magBootsSound;
        magBootsSource.volume = magBootsVolume;

        wallSlidingSource.clip = wallSurfaceSlideSound;
        wallSlidingSource.volume = wallSurfaceSlideVolume;    
    }

    private void Update()
    {
        //Movement Sounds
        if (pCon.isMoving && !isPlayingFootsteps && (!pCon.isGroundSliding && !pCon.isWallSliding))
        {
            isPlayingFootsteps = true;
            StopAllCoroutines();
            StartCoroutine(FadeInFootsteps());
        }
        else if (!pCon.isMoving && isPlayingFootsteps || pCon.inAir)
        {
            isPlayingFootsteps = false;
            StopAllCoroutines();
            StartCoroutine(FadeOutFootsteps());
        }

        // Mag Boots Sounds
        if (pCon.magBootsOn && !isPlayingMagBoots)
        {
            magBootsSource.Play();
            isPlayingMagBoots = true;
            PlayEffectSource(magBootsSwitchSound, magBootsSwitchVolume);
        }
        else if (!pCon.magBootsOn && isPlayingMagBoots)
        {
            magBootsSource.Stop();
            isPlayingMagBoots = false;
            PlayEffectSource(magBootsSwitchSound, magBootsSwitchVolume);
        }

        // Detect Wind Zone Warning
        if (pCon.inWindZone && !windZoneWarningPlayed)
        {
            PlayWindEnterWarningSound();
            windZoneWarningPlayed = true;
        }
        else if (!pCon.inWindZone && windZoneWarningPlayed)
        {
            PlayWindExitWarningSound();
            windZoneWarningPlayed = false;
        }

        // Detect Surface Sliding
        if ((pCon.isGroundSliding || pCon.isWallSliding) && !isSlideSoundPlaying)
        {
            wallSlidingSource.Play();
            isSlideSoundPlaying = true;
        }
        else if ((!pCon.isGroundSliding && !pCon.isWallSliding) && isSlideSoundPlaying)
        {
            wallSlidingSource.Stop();
            isSlideSoundPlaying = false;
        }
    }


    // <<----------------------------------------------------- PUBLIC SOUND EFFECTS ------------------------------------------- //

    private void PlayEffectSource(AudioClip _clip, float _volume)
    {
        effectSource.volume = _volume;
        effectSource.PlayOneShot(_clip);
    }

    public void PlaySlideSound()
    {
        PlayEffectSource(slideSound, slideVolume);
    }

    public void PlayJumpSound()
    {
        PlayEffectSource(jumpSound, jumpVolume);
    }

    public void PlayClimbSound()
    {
        PlayEffectSource(climbSound, climbVolume);
    }

    public void PlayLandSound(bool _isHardLanding, bool _isHeavyLanding)
    {
        if (_isHeavyLanding)
        {
            PlayEffectSource(landSound, heavyLandVolume);
        }
        else if (_isHardLanding)
        {
            PlayEffectSource(landSound, hardLandVolume);
        }
        else
        {
            PlayEffectSource(landSound, landVolume);
        }
    }

    public void PlayHurtSound()
    {
        PlayEffectSource(hurtSound, hurtVolume);
    }

    public void PlayDeathSound()
    {
        PlayEffectSource(deathSound, deathVolume);
    }

    public void PlayPunchSound(int _punch)
    {
        if (_punch == 1)
            PlayEffectSource(punch1Sound, punch1Volume);
        else if (_punch == 2)
            PlayEffectSource(punch2Sound, punch2Volume);
        else if (_punch == 3)
            PlayEffectSource(punch3Sound, punch3Volume);
    }

    public void PlayConnectSound(int _punch)
    {
        if (_punch == 1)
            PlayEffectSource(connectSound, connect1Volume);
        else if (_punch == 2)
            PlayEffectSource(connectSound, connect2Volume);
        else if (_punch == 3)
            PlayEffectSource(connectSound, connect3Volume);
    }

    public void PlayWindEnterWarningSound()
    {
        PlayEffectSource(enterWindWarningSound, windWarningVolume);
    }

    public void PlayWindExitWarningSound()
    {
        PlayEffectSource(exitWindWarningSound, windWarningVolume);
    }

    // <<----------------------------------------------------- FOOT STEPS ------------------------------------------- //

    public void StopFootSteps()
    {
                    isPlayingFootsteps = false;
            StopAllCoroutines();
        StartCoroutine(FadeOutFootsteps());
    }

    private IEnumerator FadeInFootsteps()
    {
        footStepsSource.volume = 0;

        if (pCon.inWindZone && pCon.backToWind)
            footStepsSource.clip = footStepsInWind;
        else
            footStepsSource.clip = footStepsNormal;

        footStepsSource.Play();

        while (footStepsSource.volume < desiredMaxFootVolume)
        {
            footStepsSource.volume += volumeIncrementation;

            yield return new WaitForSeconds(incrementTime);
        }

        yield break;
    }

    private IEnumerator FadeOutFootsteps()
    {
        while (footStepsSource.volume > 0)
        {
            footStepsSource.volume -= volumeDecrementation;

            yield return new WaitForSeconds(decrementTime);
        }

        footStepsSource.Stop();

        yield break;
    }
}
