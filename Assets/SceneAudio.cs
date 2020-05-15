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

    private void OnEnable()
    {
        SpawnManager.onResetLevelObjects += PlayLevelAudioOnRespawn;
    }

    private void OnDisable()
    {
        SpawnManager.onResetLevelObjects -= PlayLevelAudioOnRespawn;
    }

    public void PlayLevelAudioOnRespawn()
    {
        if (AM.currentMusicClip != sceneTrack)
        {
            AM.PlayBGAudioORMusic(sceneTrack, true);
        }
    }
}
