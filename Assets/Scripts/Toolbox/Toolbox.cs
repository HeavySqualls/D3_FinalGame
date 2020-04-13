﻿using UnityEngine;

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

    private GameManager _gameManager;
    private LevelManager _levelManager;
    private PlayerManager _playerMan;
    private DialogueSystemManager _dialogueManager;
    private CanvasManager _canvasManager;
    //private SpawnManager _spawnManager;  // TODO: is this ok to have spawn manager in the toolbox?

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

        this._gameManager = gameObject.AddComponent<GameManager>();
        this._levelManager = gameObject.AddComponent<LevelManager>();
        this._playerMan = gameObject.AddComponent<PlayerManager>();
        this._dialogueManager = gameObject.AddComponent<DialogueSystemManager>();
        this._canvasManager = gameObject.AddComponent<CanvasManager>();
        //this._spawnManager = gameObject.AddComponent<SpawnManager>();
    }

    public GameManager GetGameManager()
    {
        return this._gameManager;
    }

    public LevelManager GetLevelManager()
    {
        return this._levelManager;
    }

    public PlayerManager GetPlayerManager()
    {
        return this._playerMan;
    }

    public DialogueSystemManager GetDialogueSystemManager()
    {
        return this._dialogueManager;
    }

    public CanvasManager GetCanvasManager()
    {
        return this._canvasManager;
    }

    //public SpawnManager GetSpawnManager()
    //{
    //    return this._spawnManager;
    //}
}
