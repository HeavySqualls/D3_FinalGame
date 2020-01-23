using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public float startHP;
    private float currentHP;

    public List<Transform> wallPieces = new List<Transform>();

    BoxCollider2D boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

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

    public void TakeDamage(Vector2 _hitDir, float _dmg, float _knockback, float _knockUp)
    {
        currentHP -= _dmg;

        Debug.Log("Wall is shaking!");

        if (currentHP <= 0f)
        {
            Debug.Log("Wall is broken");
            boxCollider.enabled = false;
            foreach (Transform breakablePiece in wallPieces)
            {
                breakablePiece.GetComponent<BreakablePiece>().DestroyWall(_hitDir, _dmg, _knockback, _knockUp);
            }

            Destroy(gameObject, 3f);
        }
        else
        {
            foreach (Transform breakablePiece in wallPieces)
            {
                breakablePiece.GetComponent<BreakablePiece>().isHit = true;
            }
        }


    }
}
