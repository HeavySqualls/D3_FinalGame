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
    [SerializeField] List<Rigidbody2D> objectsOnFloor;
    private float totalWeight;
    private bool isWeighing = false;
    Vector2 offset;

    protected override void Start()
    {
        base.Start();

        offset = new Vector2(transform.position.x, transform.position.y + 1);

        objectsOnFloor = new List<Rigidbody2D>();
    }

    // When another objects collider interacts with this collider, 
    void OnCollisionStay2D (Collision2D other)
    {
        if (!isWeighing)
        {
            // Determine total weight on floor
            StartCoroutine(WeighDelay());
        }

        // If the weight exceeds the total allowable weight,
        if (totalWeight > totalAllowableWeight)
        {
            StartCoroutine(CollapseAndRespawnCounter());
        }
        else
        {
            isWeighing = false;
        }
    }

    IEnumerator WeighDelay()
    {
        isWeighing = true;
        yield return new WaitForSeconds(weighWaitTime);
        DetermineWeight();
        yield break;
    }

    private void DetermineWeight()
    {
        totalWeight = 0;

        Collider2D[] objects = Physics2D.OverlapBoxAll(offset, boxCollider.size, 0);

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].gameObject.GetComponent<Rigidbody2D>())
            {
                objectsOnFloor.Add(objects[i].GetComponent<Rigidbody2D>());
            }
        }

        for (int i = 0; i < objectsOnFloor.Count; i++)
        {
            totalWeight += objectsOnFloor[i].mass;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawCube(offset, boxCollider.size);
    //}
}
