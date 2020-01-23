using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbox : MonoBehaviour
{
    private static Toolbox _instance;
    public static Toolbox GetInstance()
    {
        if (Toolbox._instance == null)
        {
            var go = new GameObject("Toolbox");
            DontDestroyOnLoad(go);
            Toolbox._instance = go.AddComponent<Toolbox>();
        }

        return Toolbox._instance;
    }

    private ShakeManager _shakeMan;

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }

        this._shakeMan = gameObject.AddComponent<ShakeManager>();

    }

    public ShakeManager GetShakeManager()
    {
        return this._shakeMan;
    }
}
