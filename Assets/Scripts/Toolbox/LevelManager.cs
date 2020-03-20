using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Provides a place for loot crates to add themselves to a list of loot crates 

    [SerializeField] private List<PickUpItem> pickUpItems = new List<PickUpItem>();
    [SerializeField] private List<Enemy_Base> baseEnemies = new List<Enemy_Base>();
    [SerializeField] private List<Enemy_Turret_Base> turretBaseEnemies = new List<Enemy_Turret_Base>();

    public void ClearLists()
    {
        // Clean all lists from the last scene to start fresh for the new scene
        pickUpItems.Clear();
        baseEnemies.Clear();
        turretBaseEnemies.Clear();
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
