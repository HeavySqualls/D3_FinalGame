using System.Collections;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class SilentCity : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float levelDuration;
    [SerializeField] float shakeStrength;
    [SerializeField] int virbrato;
    [SerializeField] float randomness;

    [SerializeField] float waitToStartTime;

    bool isMoving = false;

    public CinemachineVirtualCamera cam;
    Tween shakeTween;

    private void Start()
    {
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

            if (objectHit.gameObject.GetComponent<PlayerController>())
            {
                objectHit.GetHit(Vector2.left, 100f, 0, 0, 0);
                StartCoroutine(WaitAndRestart());
            }
        }
    }

    IEnumerator WaitAndRestart()
    {
        yield return new WaitForSeconds(2);

        Toolbox.GetInstance().GetGameManager().RestartLevel();
    }
}
