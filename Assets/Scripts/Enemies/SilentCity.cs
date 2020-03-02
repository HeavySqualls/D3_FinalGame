using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SilentCity : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float levelDuration;
    [SerializeField] float shakeStrength;
    [SerializeField] int virbrato;
    [SerializeField] float randomness;

    [SerializeField] float waitToStartTime;

    bool isMoving = false;

    Camera cam;
    Tween shakeTween;

    private void Start()
    {
        cam = Camera.main;

        StartCoroutine(WaitToStart());
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position += Vector3.left * Time.deltaTime * moveSpeed;
        }
    }

    private void StartMoving()
    {
        shakeTween = cam.transform.DOShakePosition(levelDuration, shakeStrength, virbrato, randomness, false, false);
        isMoving = true;
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(waitToStartTime);
        StartMoving();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        RecieveDamage objectHit = col.gameObject.GetComponent<RecieveDamage>();

        if (objectHit != null)
        {
            print("Ran over " + objectHit.name);
        }
    }
}
