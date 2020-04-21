using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicSpill : Interact_Base
{
    public bool isTurnedOn = true;
    public float damageDelay;
    private bool isDamageDelay = false;

    public float maxEmmisionPause = 3f;
    private float emmisionPause = 0f;

    [Space]
    [Header("Audio:")]
    [SerializeField] AudioClip sprayingSound;
    [SerializeField] float desiredMaxVolume = 1f;
    [SerializeField] float volumeIncrementation = 0.08f;
    [SerializeField] float incrementTime = 0.02f;
    bool isSoundPlaying = false;
    AudioSource toxicSpillAudioSource;

    [SerializeField] ParticleSystem partSyst;

    void Start()
    {
        partSyst = GetComponentInChildren<ParticleSystem>();
        toxicSpillAudioSource = GetComponent<AudioSource>();
        toxicSpillAudioSource.clip = sprayingSound;
        toxicSpillAudioSource.loop = true;
        StartCoroutine(FadeInSound());

        Toolbox.GetInstance().GetAudioManager().AddAudioSources(toxicSpillAudioSource);
    }

    void Update()
    {
        //print(partSyst.isEmitting);
        if (!partSyst.isEmitting && isTurnedOn)
        {
            emmisionPause += Time.deltaTime;

            if (isSoundPlaying)
            {
                //StopAllCoroutines();
                //StartCoroutine(FadeOutSound());
                isSoundPlaying = false;
            }

            if (emmisionPause >= maxEmmisionPause)
            {
                partSyst.Play();

                if (!isSoundPlaying)
                {
                    StopAllCoroutines();
                    StartCoroutine(FadeInSound());
                    isSoundPlaying = true;
                }
            }
        }
    }

    void OnParticleCollision(GameObject other)
    {
        pRecieveDamage = other.GetComponent<RecieveDamage>();

        if (pRecieveDamage != null && !isDamageDelay && pRecieveDamage.gameObject.GetComponent<PlayerController>())
        {
            isDamageDelay = true;
            pRecieveDamage.GetHit(hitDirection, damage, knockBack, knockUp, stunTime);
            StartCoroutine(DamageDelay());
        }
    }

    IEnumerator DamageDelay()
    {       
        yield return new WaitForSeconds(damageDelay);

        isDamageDelay = false;
    }

    public void TurnOffSpillPipe()
    {
        isTurnedOn = false;
        StopAllCoroutines();
        StartCoroutine(FadeOutSound(true));
        var main = partSyst.main;
        main.loop = false;
        partSyst.Stop();
    }

    private IEnumerator FadeInSound()
    {
        //yield return new WaitForSeconds(soundDelay);

        toxicSpillAudioSource.volume = 0;
        toxicSpillAudioSource.Play();

        while (toxicSpillAudioSource.volume < desiredMaxVolume)
        {
            toxicSpillAudioSource.volume += volumeIncrementation;

            yield return new WaitForSeconds(incrementTime);
        }

        StartCoroutine(FadeOutSound(false));

        yield break;
    }

    private IEnumerator FadeOutSound(bool _immediate)
    {
        if (!_immediate)
            yield return new WaitForSeconds(4);

        while (toxicSpillAudioSource.volume > 0)
        {
            toxicSpillAudioSource.volume -= volumeIncrementation;

            yield return new WaitForSeconds(incrementTime);
        }

        toxicSpillAudioSource.Stop();

        yield break;
    }
}
