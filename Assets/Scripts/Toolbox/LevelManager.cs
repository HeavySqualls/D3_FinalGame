using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    // Tracks and provides access to important pieces within the level (such as pickups, enemies, camera etc...)
    [SerializeField] private CinemachineVirtualCamera levelCam;

    [SerializeField] private List<PickUpItem> pickUpItems = new List<PickUpItem>();
    [SerializeField] private List<Enemy_Base> baseEnemies = new List<Enemy_Base>();
    [SerializeField] private List<CrabwormTriggerZones> cwTriggers = new List<CrabwormTriggerZones>(); //TODO: Set trigger zones up to subscribe to an event that will reset them automatically
    [SerializeField] private List<Enemy_Turret_Base> turretBaseEnemies = new List<Enemy_Turret_Base>();
    [SerializeField] private List<BreakableObject> breakableObjects = new List<BreakableObject>();

    public void ClearLists()
    {
        // Clean all lists from the last scene to start fresh for the new scene
        pickUpItems.Clear();
        baseEnemies.Clear();
        turretBaseEnemies.Clear();
        breakableObjects.Clear();
    }


    // < ------------------------------------- TRACK VIRTUAL CAMERA ----------------------------- >> //

    public void AssignVirtualCam(CinemachineVirtualCamera _cam)
    {
        levelCam = _cam;
    }

    public CinemachineVirtualCamera GetVirtualCam()
    {
        return this.levelCam;
    }


    // < ------------------------------------- TRACK BREAKABLE WALLS ----------------------------- >> //

    public void AddBreakableObjects(BreakableObject _breakableObject)
    {
        breakableObjects.Add(_breakableObject);
    }

    // < ------------------------------------- TRACK PICKUPS ------------------------------------- >> //

    public void AddPickups(PickUpItem _item)
    {
        pickUpItems.Add(_item);
    }
    public void RemovePickups(PickUpItem _item)
    {
        pickUpItems.Remove(_item);
    }


    // < ------------------------------------- ENEMY MANAGEMENT ------------------------------------- >> //

    public void PauseAllEnemies() // Loops through each enemy in the game and stops them from moving/attacking
    {
        foreach  (Enemy_Base e in baseEnemies)
        {
            e.isUnitPaused = true;
        }

        foreach (Enemy_Turret_Base et in turretBaseEnemies)
        {
            et.isTurretPaused = true;
        }
    }

    public void UnPauseAllEnemies() // Loops through each enemy in the game and allows them to move/attack again
    {
        foreach (Enemy_Base e in baseEnemies)
        {
            e.isUnitPaused = false;
        }

        foreach (Enemy_Turret_Base et in turretBaseEnemies)
        {
            et.isTurretPaused = false;
        }
    }

    public void AddcwTriggerZone(CrabwormTriggerZones _cwTZ)
    {
        cwTriggers.Add(_cwTZ);
    }

    // Store all Base type Enemies
    public void AddBaseEnemies(Enemy_Base _enemy)
    {
        baseEnemies.Add(_enemy);
    }
    public void RemoveBaseEnemies(Enemy_Base _enemy)
    {
        baseEnemies.Remove(_enemy);
    }

    // Store all Turret type Enemies
    public void AddTurretEnemies(Enemy_Turret_Base _enemy)
    {
        turretBaseEnemies.Add(_enemy);
    }
    public void RemoveTurretEnemies(Enemy_Turret_Base _enemy)
    {
        turretBaseEnemies.Remove(_enemy);
    }
}
