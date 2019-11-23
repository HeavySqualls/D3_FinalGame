using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRayChecks : MonoBehaviour
{
    PlayerController pCon;
    void Start()
    {
        pCon = GetComponent<PlayerController>();
    }

    // TODO: Raycast to check for ground for ghost jumping and quick double jump features

    // TODO: Raycast at head and chest to check for ledge grab/wall grab

    // TODO: Have a timer for above features 

    void Update()
    {
        
    }
}
