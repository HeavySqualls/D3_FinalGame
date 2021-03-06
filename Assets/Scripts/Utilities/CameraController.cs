﻿using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public GameObject objectToFollow;

    public float yOffsetRate = -0.75f;
    public float yOffsetStart = -2.15f;
    private float yOffset;

    public float speed = 2.0f;

    public bool isCamera;

    CinemachineVirtualCamera cam;
    PlayerController pCon;

    private void Awake()
    {
        cam = GetComponentInChildren<CinemachineVirtualCamera>();
        Toolbox.GetInstance().GetLevelManager().AssignVirtualCam(cam);
    }

    void Start()
    {
        if (isCamera)
        {
            pCon = Toolbox.GetInstance().GetPlayerManager().GetPlayerController();
        }

        //yOffset = yOffsetStart;
    }

    void FixedUpdate()
    {
        //if (objectToFollow == null)
        //{
        //    Debug.Log("Assign a target to the Smooth Follow component on the Camera Anchor.");
        //}
        //else
        //{
        //    float interpolation = speed * Time.deltaTime;

        //    Vector3 position = transform.position;
        //    position.y = Mathf.Lerp(transform.position.y, objectToFollow.transform.position.y - yOffset, interpolation);
        //    position.x = Mathf.Lerp(transform.position.x, objectToFollow.transform.position.x, interpolation);

        //    transform.position = position;
        //}
    }
}
