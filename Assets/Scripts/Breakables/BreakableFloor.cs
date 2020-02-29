using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableFloor : BreakableObject
{
    [Space]
    [Header("VARIABLES:")]
    [Tooltip("The length of time until the object will be destroyed.")]
    public float destroySeconds;

    [Space]
    [Header("WEIGHT VARIABLES:")]
    [Tooltip("The delay before the items get weighed - to give a brief moment before the floor collapses.")]
    public float weighWaitTime;
    [Tooltip("The max amount of weight allowed on the floor before it collapses.")]
    public float totalAllowableWeight;
    [SerializeField] List<WeightData> objectsOnFloor = new List<WeightData>();
    [SerializeField] private float totalWeight;

    protected override void Start()
    {
        base.Start();
    }

    // When another objects collider interacts with this collider, 
    void OnTriggerEnter2D (Collider2D other)
    {
        WeightData newObj = other.gameObject.GetComponent<WeightData>();

        if (newObj != null)
        {
            objectsOnFloor.Add(newObj);
            hitByHeavyObject = true;
            DetermineWeight();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        WeightData newObj = other.gameObject.GetComponent<WeightData>();

        if (newObj != null)
        {
            objectsOnFloor.Remove(newObj);
            DetermineWeight();
        }
    }

    private void DetermineWeight()
    {
        totalWeight = 0;

        for (int i = 0; i < objectsOnFloor.Count; i++)
        {
            totalWeight += objectsOnFloor[i].objWeight;
        }

        // If the weight exceeds the total allowable weight,
        if (totalWeight >= totalAllowableWeight)
        {
            StartCoroutine(CollapseAndRespawnCounter());
            objectsOnFloor.Clear();
            totalWeight = 0;
        }
    }
}
