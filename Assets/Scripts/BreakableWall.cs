using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    private float currentHP;
    public float startHP;

    public List<Transform> wallPieces = new List<Transform>();

    void Start()
    {
        FindEveryChild(gameObject.transform);
        currentHP = startHP;
    }

    private void FindEveryChild (Transform parent)
    {
        int count = parent.childCount;

        for (int i = 0; i < count; i++)
        {
            wallPieces.Add(parent.GetChild(i));
        }
    }

    public void TakeDamage(Vector2 _hitDir, float _damage, float _x, float _y)
    {
        currentHP -= _damage;

        Debug.Log("Wall is shaking!");

        if (currentHP <= 0f)
        {
            Debug.Log("Wall is broken");
        }
    }
}
