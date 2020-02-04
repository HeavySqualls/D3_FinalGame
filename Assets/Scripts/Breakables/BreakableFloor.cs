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
    [Tooltip("The max amount of weight allowed on the floor.")]
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

// When another objects collider interacts with this collider, 
//void OnCollisionStay2D(Collision2D other)
//{
//    if (!isWeighing)
//    {
//        StartCoroutine(WeighDelay());
//    }
//}

//IEnumerator WeighDelay()
//{
//    isWeighing = true;

//    yield return new WaitForSeconds(weighWaitTime);

//    DetermineWeight();
//    yield break;
//}

//private void DetermineWeight()
//{
//    totalWeight = 0;

//    Collider2D[] objects = Physics2D.OverlapBoxAll(offset, boxCollider.size, 0);

//    for (int i = 0; i < objects.Length; i++)
//    {
//        WeightData objData = objects[i].GetComponent<WeightData>();

//        if (objData != null)
//        {
//            objectsOnFloor.Add(objData.objWeight);
//        }
//    }

//    for (int i = 0; i < objectsOnFloor.Count; i++)
//    {
//        totalWeight += objectsOnFloor[i];
//    }

//    // If the weight exceeds the total allowable weight,
//    if (totalWeight > totalAllowableWeight && !isFallingApart)
//    {
//        StartCoroutine(CollapseAndRespawnCounter());
//        objectsOnFloor.Clear();
//        totalWeight = 0;
//        isWeighing = false;
//    }
//    else
//    {
//        isWeighing = false;
//    }


//private void OnDrawGizmos()
//{
//    Gizmos.color = Color.red;
//    Gizmos.DrawCube(offset, boxCollider.size);
//}
//}
