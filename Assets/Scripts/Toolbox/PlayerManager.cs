﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Store a reference to the player for other classes to access it. 

    // Hold information regarding the players stats, current progress etc...

    InventoryManager inventoryManager;
    PlayerController pCon;

    public void SetPlayerController(PlayerController _pCon)
    {
        pCon = _pCon;
    }

    public void SetInventoryManager(InventoryManager _inMan)
    {
        inventoryManager = _inMan;
    }

    public InventoryManager GetInventoryManager()
    {
        return inventoryManager;
    }

}