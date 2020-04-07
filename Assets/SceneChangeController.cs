using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Toolbox.GetInstance().GetGameManager().LoadNextScene();
    }
}
