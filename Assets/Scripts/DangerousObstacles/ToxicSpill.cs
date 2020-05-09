using System.Collections;
using UnityEngine;

public class ToxicSpill : Interact_Base
{
    public bool isTurnedOn = true;
    [SerializeField] bool isPaused = false;
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
    [SerializeField] float partPlayTime = 4f;
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
        if (!partSyst.isEmitting && isTurnedOn && !isPaused)
        {
            StartCoroutine(EmissionPause());
            isPaused = true;
        }
    }

    IEnumerator EmissionPause()
    {
        //print("emission pause");
        while (emmisionPause < maxEmmisionPause)
        {
            emmisionPause += Time.deltaTime;
        }

        FadeSound();

        yield break;
    }

    private void FadeSound()
    {
        partSyst.Play();
        StopAllCoroutines();
        StartCoroutine(FadeInSound());
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
        toxicSpillAudioSource.volume = 0;
        toxicSpillAudioSource.pitch = Random.Range(0.9f, 1.25f);
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
            yield return new WaitForSeconds(partPlayTime);

        while (toxicSpillAudioSource.volume > 0)
        {
            toxicSpillAudioSource.volume -= volumeIncrementation;

            yield return new WaitForSeconds(incrementTime);
        }

        isPaused = false;
        toxicSpillAudioSource.Stop();

        yield break;
    }
}
