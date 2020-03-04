using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Provides a place for loot crates to add themselves to a list of loot crates 

    [SerializeField] private List<PickUpItem> pickUpItems = new List<PickUpItem>();

    public void AddPickups(PickUpItem _item)
    {
        pickUpItems.Add(_item);
    }
    public void RemovePickups(PickUpItem _item)
    {
        pickUpItems.Remove(_item);
    }

}
