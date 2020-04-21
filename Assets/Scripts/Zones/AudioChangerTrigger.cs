using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChangerTrigger : MonoBehaviour
{
    public bool isMusicFadeOut = false;
    public bool isBGFadeOut = false;

    [SerializeField] AudioClip areaMusic;
    [SerializeField] AudioClip areaBG;

    AudioManager AM;

    private void Start()
    {
        AM = Toolbox.GetInstance().GetAudioManager();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerAudioController pAudio = collision.gameObject.GetComponent<PlayerAudioController>();

        if (pAudio != null)
        {
            print("Change music");

            if (isMusicFadeOut)
            {
                AM.PlayAudioFadeOut(true);
            }
            else if (isBGFadeOut)
            {
                AM.PlayAudioFadeOut(false);
            }
            else
            {
                if (areaMusic != null && AM.GetCurrentMusicClip().name != areaMusic.name)
                    AM.PlayBGAudioORMusic(areaMusic, true);

                if (areaBG != null && AM.GetCurrentBGClip().name != areaBG.name)
                    AM.PlayBGAudioORMusic(areaBG, false);
            }
        }
    }
}
