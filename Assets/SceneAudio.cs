using UnityEngine;

public class SceneAudio : MonoBehaviour
{
    [SerializeField] AudioClip sceneTrack;
    [SerializeField] AudioClip sceneBG1;
    [SerializeField] AudioClip sceneBG2;

    AudioManager AM;

    private void Start()
    {
        AM = Toolbox.GetInstance().GetAudioManager();
        AM.PlayBGAudioORMusic(sceneBG1, false);
        AM.PlayBGAudioORMusic(sceneTrack, true);
    }
}
