using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemAmount
{
    public sScrapItem item;
    [Range(1,999)] // to prevent a negative number from being possible
    public int amount;
}


[CreateAssetMenu(fileName = "Scriptable Objects", menuName = "Crafting/Recipes", order = 1)]
public class sCraftingRecipe : ScriptableObject
{
    public List<ItemAmount> materials;
    public List<ItemAmount> results;

    public bool CanCraft(IItemContainer itemContainer)
    {
        // loop through the materials 
        foreach (ItemAmount itemAmount in materials)
        {
            // check if the item container has the same amount of items or more needed for the crafting recipe
            if (itemContainer.ItemCount(itemAmount.item.ID) < itemAmount.amount)
            {
                // if not return false
                return false;
            }
        }
        // if so, all items were found and are sufficient for crafting the recipe, return true
        return true;
    }

    public void Craft(IItemContainer itemContainer)
    {
        // if we have sufficient materials to craft the item
        if (CanCraft(itemContainer))
        {
            // loop through the materials list and remove the required materials 
            foreach (ItemAmount itemAmount in materials)
            {
                for (int i = 0; i < itemAmount.amount; i++)
                {
                    sScrapItem oldItem = itemContainer.RemoveItem(itemAmount.item.ID);
                    Destroy(oldItem);
                }
            }

            // loop through the results list and add them back to the inventory
            foreach (ItemAmount itemAmount in results)
            {
                for (int i = 0; i < itemAmount.amount; i++)
                {
                    itemContainer.AddItem(Instantiate(itemAmount.item));
                }
            }
        }
    }
}
