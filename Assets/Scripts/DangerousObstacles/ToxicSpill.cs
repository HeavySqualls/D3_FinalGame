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
    AudioSource audioSource;

    [SerializeField] ParticleSystem partSyst;

    void Start()
    {
        partSyst = GetComponentInChildren<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = sprayingSound;
        audioSource.loop = true;
        StartCoroutine(FadeInSound());
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

        if (pRecieveDamage != null && !isDamageDelay)
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

        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < desiredMaxVolume)
        {
            audioSource.volume += volumeIncrementation;

            yield return new WaitForSeconds(incrementTime);
        }

        StartCoroutine(FadeOutSound(false));

        yield break;
    }

    private IEnumerator FadeOutSound(bool _immediate)
    {
        if (!_immediate)
            yield return new WaitForSeconds(4);

        while (audioSource.volume > 0)
        {
            audioSource.volume -= volumeIncrementation;

            yield return new WaitForSeconds(incrementTime);
        }

        audioSource.Stop();

        yield break;
    }
}
