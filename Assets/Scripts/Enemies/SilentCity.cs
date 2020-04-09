using System.Collections;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class SilentCity : MonoBehaviour
{
    [Header("Chase Variables:")]
    public Transform target;
    [SerializeField] float waitToStartTime;
    [SerializeField] float distance;

    [SerializeField] float accelDecelTimeStart = 0.1f;
    [SerializeField] float accelDecelTimeMax = 2f;
    [SerializeField] float currentAccelDecelTime;
    [SerializeField] float currentMoveSpeed;
    [SerializeField] float closeMoveSpeed = 8f;
    [SerializeField] float nearMoveSpeed = 8f;
    [SerializeField] float baseMoveSpeed = 13f;
    [SerializeField] float distantMoveSpeed = 8f;
    [SerializeField] float farMoveSpeed = 16f;

    [SerializeField] float closeDistance = 47f;
    [SerializeField] float nearDistance = 47f;
    [SerializeField] float baseDistance = 52f;
    [SerializeField] float distantDistance = 47f;
    [SerializeField] float farDistance = 58f;
    bool isMoving = false;

    [Space]
    [Header("Screen Shake:")]
    public CinemachineVirtualCamera cam;
    Tween shakeTween;
    [SerializeField] float levelDuration;
    [SerializeField] float shakeStrength;
    [SerializeField] int virbrato;
    [SerializeField] float randomness;

    private void Start()
    {
        StartCoroutine(WaitToStart());
        currentMoveSpeed = baseMoveSpeed;
    }

    private void Update()
    {
        TrackDistanceBetweenObject();
        AdjustSpeedBasedOnDistance();

        if (isMoving)
        {
            transform.position += Vector3.left * Time.deltaTime * currentMoveSpeed;
        }
    }

    private void TrackDistanceBetweenObject()
    {
        distance = Vector3.Distance(transform.position, target.transform.position);
    }

    private void AdjustSpeedBasedOnDistance()
    {
        if (distance < closeDistance)
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, closeMoveSpeed, currentAccelDecelTime * Time.deltaTime);
        }
        else if (distance > closeDistance && distance < baseDistance)
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, nearMoveSpeed, currentAccelDecelTime * Time.deltaTime);
        }
        else if (distance > baseDistance && distance < distantDistance)
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, baseMoveSpeed, currentAccelDecelTime * Time.deltaTime);
        }
        else if (distance > distantDistance && distance < farDistance)
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, distantMoveSpeed, currentAccelDecelTime * Time.deltaTime);
        }
        else if (distance > farDistance)
        {
            //if (currentAccelDecelTime == accelDecelTimeMax)
            //    currentAccelDecelTime = accelDecelTimeStart;

            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, farMoveSpeed, currentAccelDecelTime * Time.deltaTime);
        }

        currentAccelDecelTime += 0.5f * Time.deltaTime;

        if (currentAccelDecelTime > accelDecelTimeMax)
            currentAccelDecelTime = accelDecelTimeStart;
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
