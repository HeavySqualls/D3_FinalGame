﻿using UnityEngine;

public class SceneChangeController : MonoBehaviour
{
    [SerializeField] AudioClip winSound;
    [SerializeField] float winVolume = 0.4f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Change Scene");

        ChangeScene();
    }

    public void ChangeScene()
    {
        if (winSound != null)
        {
            Toolbox.GetInstance().GetAudioManager().PlayConsistentOneShot(winSound, winVolume);
        }

        Toolbox.GetInstance().GetGameManager().LoadNextScene();
    }
}
