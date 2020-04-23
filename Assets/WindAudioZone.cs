using System.Collections;
using UnityEngine;

public class WindAudioZone : MonoBehaviour
{
    [SerializeField] BorasWindController borasWindCon;

    [Space]
    [Header("Audio:")]
    private bool isPlayerInAudioZone = false;
    private bool isWindSoundPlaying = false;
    [SerializeField] AudioClip windSound;
    [SerializeField] float windMaxVolume = 0.3f;
    [SerializeField] float windVolIncrementation = 0.01f;
    [SerializeField] float windIncrementTime = 0.05f;
    [SerializeField] AudioSource borasWindAudioSource;

    private void Start()
    {
        Toolbox.GetInstance().GetAudioManager().AddAudioSources(borasWindAudioSource);
        borasWindAudioSource.clip = windSound;
    }

    bool startStoppin = false;

    private void Update()
    {
        if (borasWindCon.isBlowing && !isWindSoundPlaying)
        {
            print("START boras wind Audio");
            StartCoroutine(AudioFade(true));
            isWindSoundPlaying = true;
        }
        else if (!borasWindCon.isBlowing && isWindSoundPlaying && !startStoppin)
        {
            startStoppin = true;
            print("STOP boras wind Audio");
            StopAllCoroutines();
            StartCoroutine(AudioFade(false));
        }
    }

    IEnumerator AudioFade(bool _isFadeIn)
    {
        if (_isFadeIn)
        {
            borasWindAudioSource.volume = 0;

            if (isPlayerInAudioZone)
            {
                borasWindAudioSource.Play();
            }

            while (borasWindAudioSource.volume < windMaxVolume)
            {
                borasWindAudioSource.volume += windVolIncrementation;
                yield return new WaitForSeconds(windIncrementTime);
            }
        }
        else
        {
            while (borasWindAudioSource.volume > 0)
            {
                borasWindAudioSource.volume -= windVolIncrementation;
                yield return new WaitForSeconds(windIncrementTime);
            }

            if (borasWindAudioSource.isPlaying)
                borasWindAudioSource.Stop();

            isWindSoundPlaying = false;
            startStoppin = false;
        }

        yield break;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerAudioController pAudioCon = collision.gameObject.GetComponent<PlayerAudioController>();

        if (pAudioCon != null)
        {
            isPlayerInAudioZone = true;

            if (isWindSoundPlaying)
            {
                borasWindAudioSource.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerAudioController pAudioCon = collision.gameObject.GetComponent<PlayerAudioController>();

        if (pAudioCon != null)
        {
            isPlayerInAudioZone = false;

            if (isWindSoundPlaying)
            {
                StopAllCoroutines();
                StartCoroutine(AudioFade(false));
            }
        }
    }
}
