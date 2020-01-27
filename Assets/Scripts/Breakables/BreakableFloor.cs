using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableFloor : MonoBehaviour
{
    [Space]
    [Header("VARIABLES:")]
    [Tooltip("The length of time until the object will be destroyed.")]
    public float destroySeconds;
    [Tooltip("The length of time the object will shake.")]
    public float shakeDuration;
    [Tooltip("The point during the shake at which the shake strength will begin to decrease back to 0.")]
    public float decreasePoint;
    [Tooltip("The speed at which the objects will shake.")]
    public float shakeSpeed;
    [Tooltip("The max angle at which the objects will rotate when shaken.")]
    public float rotationAngle;

    [Space]
    [Header("WEIGHT VARIABLES:")]
    [Tooltip("The max amount of weight allowed on the floor.")]
    public float totalAllowableWeight;
    [SerializeField] List<Rigidbody2D> objectsOnFloor;
    private float totalWeight;
    Vector2 offset;

    [Space]
    [Header("OBJECT PIECES:")]
    [SerializeField] List<BreakablePiece> objPieces;

    BoxCollider2D boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        offset = new Vector2(transform.position.x, transform.position.y + 1);

        objectsOnFloor = new List<Rigidbody2D>();
        objPieces = new List<BreakablePiece>();

        FindEveryChild(gameObject.transform);
    }

    // Finds every child in the game object with a BreakablePiece component and adds it to the list 
    private void FindEveryChild(Transform parent)
    {
        int count = parent.childCount;

        for (int i = 0; i < count; i++)
        {
            if (!parent.GetChild(i).transform.GetComponent<BreakablePiece>().isEarlyBreakPiece)
            {
                objPieces.Add(parent.GetChild(i).transform.GetComponent<BreakablePiece>());
            }
        }
    }

    // Shakes the floor when the player lands on it
    public void TriggerFloorShake()
    {
        Debug.Log("Platform is collapsing!");

        foreach (BreakablePiece bp in objPieces)
        {
            bp.ShakeGameObject(bp.gameObject, shakeDuration, decreasePoint, shakeSpeed, rotationAngle, false);
        }
    }

    // When another objects collider interacts with this collider, 
    void OnCollisionEnter2D (Collision2D other)
    {
        // Determine total weight on floor
        DetermineWeight();

        // If the weight exceeds the total allowable weight,
        if (totalWeight > totalAllowableWeight)
        {
            boxCollider.enabled = false;

            foreach (BreakablePiece bp in objPieces)
            {
                bp.rb2D.bodyType = RigidbodyType2D.Dynamic;
                bp.rb2D.gravityScale = 2f;
                bp.boxColl.enabled = true;
            }

            Destroy(gameObject, destroySeconds);
        }
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
