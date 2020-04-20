using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Handles all the 2D PlayOneShot audio in the scene & scene music 

    AudioSource oneShotVariedAudioSource;
    AudioSource oneShotConsistentAudioSource;

    [SerializeField] float lowPitchRange = 0.95f;
    [SerializeField] float highPitchRange = 1.05f;

    [SerializeField] float levelMusicVolume = 0.45f;
    [SerializeField] float musicVolIncrementation = 0.005f;
    [SerializeField] float musicIncrementTime = 0.05f;
    AudioSource musicAudioSource;

    [SerializeField] float BGVolume = 0.10f;
    [SerializeField] float BGVolIncrementation = 0.05f;
    [SerializeField] float BGIncrementTime = 0.02f;
    AudioSource BGAudioSource;

    private void Awake()
    {
        oneShotVariedAudioSource = gameObject.AddComponent<AudioSource>();
        oneShotConsistentAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        BGAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayVariedOneShot(AudioClip _clip, float _volume)
    {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        oneShotVariedAudioSource.pitch = randomPitch;
        oneShotVariedAudioSource.volume = _volume;
        oneShotVariedAudioSource.PlayOneShot(_clip);
    }

    public void PlayConsistentOneShot(AudioClip _clip, float _volume)
    {
        oneShotConsistentAudioSource.pitch = 1f;
        oneShotConsistentAudioSource.volume = _volume;
        oneShotConsistentAudioSource.PlayOneShot(_clip);
    }

    public void PlayLevelMusic(AudioClip _levelMusic)
    {
        StartCoroutine(FadeInAudio(musicAudioSource, _levelMusic, musicVolIncrementation, musicIncrementTime, levelMusicVolume));
    }

    public void PlayLevelBG(AudioClip _levelBG)
    {
        StartCoroutine(FadeInAudio(BGAudioSource, _levelBG, BGVolIncrementation, BGIncrementTime, BGVolume));
    }

    public void ChangeLevelMusic(AudioClip _newLevelTrack)
    {
        // Fade out previous track
        // Fade in new track
    }

    private IEnumerator FadeInAudio(AudioSource _source, AudioClip _clip, float _incVol, float _incTime, float _maxVolume)
    {
        _source.clip = _clip;
        _source.volume = 0;
        _source.Play();

        while (_source.volume < _maxVolume)
        {
            _source.volume += _incVol;

            yield return new WaitForSeconds(_incTime);
        }

        yield break;
    }
}
