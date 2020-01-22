using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolvingPlatform : MonoBehaviour
{
    [SerializeField] Material mat;
    private MeshRenderer mr;
    private BoxCollider2D col;

    public float flashTimeStart = 0.2f;
    private float flashTime;

    Coroutine dissolve;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        mr = GetComponent<MeshRenderer>();
        col = GetComponent<BoxCollider2D>();
        flashTime = flashTimeStart;
    }

    public void CallEnumerator() // TODO: Make this detect its own collisions - OnCollision check for physics object and if so, dissolve
    {
        print("Hello");

        if (dissolve != null)
        {
            StopCoroutine(Dissolve(mat));
        }

        dissolve = StartCoroutine(Dissolve(mat));
    }

    private IEnumerator Dissolve(Material _thisMat)
    {
        bool isActive = true;

        while (isActive)
        {
            flashTime -= Time.deltaTime * 4;

            _thisMat.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            _thisMat.color = Color.white;
            yield return new WaitForSeconds(0.2f);

            if (flashTime < 0)
            {
                col.enabled = false;
                mr.enabled = false;
                isActive = false;
                StartCoroutine(RespawnPlatform());
                yield return null;
            }
        }
    }

    private IEnumerator RespawnPlatform()
    {
        yield return new WaitForSeconds(5);
        col.enabled = true;
        mr.enabled = true;
        flashTime = flashTimeStart;
    }
}
