using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAudio : MonoBehaviour
{
    [SerializeField] AudioClip sceneTrack;
    [SerializeField] AudioClip sceneBG1;

    AudioManager AM;

    private void Start()
    {
        AM = Toolbox.GetInstance().GetAudioManager();
        AM.PlayLevelMusic(sceneTrack);
        AM.PlayLevelBG(sceneBG1);
    }


}
