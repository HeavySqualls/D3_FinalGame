using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Handles all the 2D PlayOneShot audio in the scene & scene music 
    AudioSource oneShotVariedAudioSource;
    AudioSource oneShotConsistentAudioSource;

    [SerializeField] float lowPitchRange = 0.95f;
    [SerializeField] float highPitchRange = 1.05f;

    // Music Tracks
    [SerializeField] float levelMusicVolume = 0.375f;
    [SerializeField] float musicVolIncrementation = 0.005f;
    [SerializeField] float musicIncrementTime = 0.05f;
    AudioClip currentMusicClip;
    AudioSource currentMusicAudioSource;
    AudioSource musicAudioSource1;
    AudioSource musicAudioSource2;

    // Back Ground Tracks
    [SerializeField] float BGVolume = 0.175f;
    [SerializeField] float BGVolIncrementation = 0.005f;
    [SerializeField] float BGIncrementTime = 0.05f;
    AudioClip currentBGClip;
    AudioSource currentBGAudioSource;
    AudioSource BGAudioSource1;
    AudioSource BGAudioSource2;

    // Audio Dampening
    [SerializeField] float dampenedAudioVol = 0.05f;
    [SerializeField] float dampenVolIncrementation = 0.1f;
    [SerializeField] float dampenIncrementTime = 0.02f;

    // Reference all Audio Managers in the current scene
    [SerializeField] List<AudioSource> sceneSources = new List<AudioSource>();

    private void Awake()
    {
        oneShotVariedAudioSource = gameObject.AddComponent<AudioSource>();
        oneShotConsistentAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource1 = gameObject.AddComponent<AudioSource>();
        musicAudioSource1.loop = true;
        musicAudioSource2 = gameObject.AddComponent<AudioSource>();
        musicAudioSource2.loop = true;
        BGAudioSource1 = gameObject.AddComponent<AudioSource>();
        BGAudioSource1.loop = true;
        BGAudioSource2 = gameObject.AddComponent<AudioSource>();
        BGAudioSource2.loop = true;
    }

    public AudioClip GetCurrentBGClip()
    {
        return currentBGClip;
    }

    public AudioClip GetCurrentMusicClip()
    {
        return currentMusicClip;
    }


    // << ------------------------------------- TRACK/AFFECT SCENE SOURCES -------------------------------- >> //

    public void ClearAudioSources()
    {
        // Clear all audio sources from the list
        sceneSources.Clear();

        // Add back audio sources from this class
        //AddAudioSources(oneShotConsistentAudioSource);
        AddAudioSources(oneShotVariedAudioSource);
    }

    public void AddAudioSources(AudioSource _source)
    {
        sceneSources.Add(_source);
    }

    public void MuteAudioSources(bool _mute)
    {
        foreach (AudioSource audioS in sceneSources)
        {
            audioS.mute = _mute;
        }
    }

    // << ------------------------------------- PLAY ONE SHOTS -------------------------------- >> //

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


    // << ------------------------------------- PLAY AUDIO -------------------------------- >> //

    public void PlayAudioFadeOut(bool _isMusicTrack)
    {
        AudioSource source;
        float volInc;
        float incTime;

        if (_isMusicTrack)
        {
            if (musicAudioSource1.isPlaying)
                source = musicAudioSource1;
            else
                source = musicAudioSource2;

            volInc = musicVolIncrementation;
            incTime = musicIncrementTime;
        }
        else
        {
            if (BGAudioSource1.isPlaying)
                source = BGAudioSource1;
            else
                source = BGAudioSource2;

            volInc = BGVolIncrementation;
            incTime = BGIncrementTime;
        }

        StartCoroutine(FadeOutAudio(source, volInc, incTime));
    }

    public void PlayBGAudioORMusic(AudioClip _newAudioClip, bool _isMusicTrack)
    {
        // Check to find the free music audio source to assign the incoming track to for cross fade
        AudioSource freeSource;
        // Get a reference to the audio source that is currently playing, or that will NOT be assigned the new audio clip
        AudioSource playingSource;

        // If we are wanting to play level music, reference the music audio sources
        if (_isMusicTrack)
        {
            if (!musicAudioSource1.isPlaying)
            {
                freeSource = musicAudioSource1; // the source we will be assigning the new audio clip
                playingSource = musicAudioSource2; 
            }
            else
            {
                freeSource = musicAudioSource2; // the source we will be assigning the new audio clip
                playingSource = musicAudioSource1;
            }

            // if we do not have a currentMusicClip assigned yet, this is the first clip that will be getting assigned, 
            // so we do not need a crossfade. Go straight to fade in.
            if (currentMusicClip == null) 
            {
                currentMusicClip = _newAudioClip;
                currentMusicAudioSource = freeSource;
                StartCoroutine(FadeInAudio(freeSource, _newAudioClip, musicVolIncrementation, musicIncrementTime, levelMusicVolume));
            }
            // If we already have a currentMusicClip assigned, we will need to crossfade that one out with the new clip, 
            // and replace the currentMusicClip variable with this new one. 
            else if(currentMusicClip != _newAudioClip)
            {
                StartCoroutine(CrossFadeAudio(freeSource, playingSource, currentMusicClip, _newAudioClip, musicVolIncrementation, musicIncrementTime, levelMusicVolume, true));
            }
        }
        else // if we are wanting to play the background audio, reference the background audio sources and repeat the process above ^^^
        {
            print("Play BG audio");
            if (!BGAudioSource1.isPlaying)
            {
                freeSource = BGAudioSource1;
                playingSource = BGAudioSource2;
            }
            else
            {
                freeSource = BGAudioSource2;
                playingSource = BGAudioSource1;
            }

            if (currentBGClip == null)
            {
                currentBGClip = _newAudioClip;
                currentBGAudioSource = freeSource;
                StartCoroutine(FadeInAudio(freeSource, _newAudioClip, BGVolIncrementation, BGIncrementTime, BGVolume));
            }
            else if(currentBGClip != _newAudioClip)
            {
                StartCoroutine(CrossFadeAudio(freeSource, playingSource, currentBGClip, _newAudioClip, BGVolIncrementation, BGIncrementTime, BGVolume, false));
            }
        }
    }


    // << ------------------------------------- FADES -------------------------------- >> //

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

    private IEnumerator FadeOutAudio(AudioSource _source, float _incVol, float _incTime)
    {
        while (_source.volume > 0)
        {
            _source.volume -= _incVol;
            yield return new WaitForSeconds(_incTime);
        }

        yield break;
    }

    private IEnumerator CrossFadeAudio(AudioSource _freeSource, AudioSource _playingSource, AudioClip _currentClip, AudioClip _newClip, 
        float _incVol, float _incTime, float _maxVolume, bool isMusic)
    {
        Debug.Log("Cross-fade tracks " + _currentClip.name + " to " + _newClip.name);

        if (isMusic)
        {
            currentMusicClip = _newClip;
            currentMusicAudioSource = _freeSource;
        }
        else
        {
            currentBGClip = _newClip;
            currentBGAudioSource = _freeSource;
        }

        // Set up free audio source
        _freeSource.clip = _newClip;
        _freeSource.volume = 0;
        _freeSource.Play();

        // Bring in the volume of audioSource1 while decreasing the volume of audioSource2
        while (_playingSource.volume > 0)
        {
            _freeSource.volume += _incVol;
            _playingSource.volume -= _incVol;

            yield return new WaitForSeconds(_incTime);
        }

        _freeSource.volume = _maxVolume;
        _playingSource.volume = 0;
        _playingSource.Stop();
        _playingSource.clip = null;

        yield break;
    }


    // << ------------------------------------- DAMPENERS -------------------------------- >> //

    public void DampenBGMusic(bool _isReducingVol)
    {
        if (currentBGAudioSource.isPlaying)
        {
            if (_isReducingVol)
                currentBGAudioSource.volume = dampenedAudioVol;
            //StartCoroutine(DampenAudio(currentBGAudioSource, dampenedAudioVol, _isReducingVol));
            else
                currentMusicAudioSource.volume = BGVolume;
                //StartCoroutine(DampenAudio(currentBGAudioSource, BGVolume, _isReducingVol));
        }

        if (currentMusicAudioSource.isPlaying)
        {
            if (_isReducingVol)
                currentMusicAudioSource.volume = dampenedAudioVol;
            //StartCoroutine(DampenAudio(currentMusicAudioSource, dampenedAudioVol, _isReducingVol));
            else
                currentMusicAudioSource.volume = levelMusicVolume;
                //StartCoroutine(DampenAudio(currentMusicAudioSource, levelMusicVolume, _isReducingVol));
        }
    }

    public void DampenAllAudio(bool _isReducingVol)
    {
        if (_isReducingVol)
        {
            foreach (AudioSource AS in sceneSources)
                AS.volume = AS.volume / 4;
        }
        else
            foreach (AudioSource AS in sceneSources)
                AS.volume = AS.volume * 4;
    }

    //private IEnumerator DampenAudio(AudioSource _source, float _desiredVol, bool _isReducingVol)
    //{
    //    // If we are reducing the sources volume...
    //    if (_isReducingVol)
    //    {
    //        print("damped");
    //        while (_source.volume > _desiredVol)
    //        {
    //            _source.volume -= dampenVolIncrementation;

    //            yield return new WaitForSeconds(dampenIncrementTime);
    //        }
    //    }
    //    // if we are increasing the sources volume...
    //    else
    //    {
    //        print("undamped");
    //        while (_source.volume < _desiredVol)
    //        {
    //            _source.volume += dampenVolIncrementation;

    //            yield return new WaitForSeconds(dampenIncrementTime);
    //        }
    //    }


    //    yield break;
    //}
}
