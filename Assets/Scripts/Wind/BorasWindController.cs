using System.Collections;
using UnityEngine;

public class BorasWindController : MonoBehaviour
{
    [Header("Boras Variables:")]
    [Tooltip("The time in between gusts of wind.")]
    [SerializeField] float intervalTime = 5f;
    [Tooltip("The duration that the wind will blow for.")]
    [SerializeField] float blowTimeStart = 2f;
    [Tooltip("The current duration that the wind is blowing.")]
    [SerializeField] private float blowTime;
    [SerializeField] private bool countdown = false;

    [Space]
    [Header("Wind Collider Variables:")]
    [Tooltip("The box collider where the wind affect will be applied.")]
    [SerializeField] BoxCollider2D[] windAreas;
    //public BoxCollider2D windArea;
    [Tooltip("The time that the box collider will be enabled at (check blowTime to set the enable time).")]
    [SerializeField] float enableDelay;
    [Tooltip("The time that the box collider will be enabled at (check blowTime to set the enable time).")]
    [SerializeField] float enableColliderTime = 1f;
    [Tooltip("The time that the box collider will be disabled at (check blowTime to set the disable time).")]
    [SerializeField] float disableColliderTime = 1f;
    bool enableColliders;

    [Space]
    [Header("Particle System:")]
    public ParticleSystem detectSyst;
    [SerializeField] bool isBlowing = false;
    private ParticleSystem borasSyst;
    private bool withChildren = true;

    //OLD MOVING COLLIDER SYSTEM VARS:
    //public WindColliderController windCollCon;
    //public float speed;
    //public Transform startPos;

    void Start()
    {
        borasSyst = GetComponentInChildren<ParticleSystem>();
        //windArea.enabled = false;

        foreach (BoxCollider2D wA in windAreas)
        {
            wA.enabled = false;
        }

        blowTime = blowTimeStart;
    }

    void Update()
    {
        BlowGust();
    }

    void  BlowGust()
    {
        if (!isBlowing)
        {
            borasSyst.Play(withChildren);
            countdown = true;
            isBlowing = true;
        }

        if (countdown && isBlowing)
        {
            blowTime -= Time.deltaTime;

            if (!enableColliders && blowTime < enableColliderTime)
            {
                //windArea.enabled = true;
                StartCoroutine(EnableWindColliders());
                enableColliders = true;
            }

            if (enableColliders && blowTime < disableColliderTime)
            {
                //windArea.enabled = false;
                StartCoroutine(DisableWindColliders());
                enableColliders = false;
            }

            if (blowTime < 0)
            {
                StartCoroutine(Interval());
                countdown = false;
            }
        }
    }

    IEnumerator EnableWindColliders()
    {
        int colliderCount = 0;

        while (colliderCount < windAreas.Length)
        {
            yield return new WaitForSeconds(enableDelay);

            windAreas[colliderCount].enabled = true;

            colliderCount++;
        }

        yield break;
    }

    IEnumerator DisableWindColliders()
    {
        int colliderCount = 0;

        while (colliderCount < windAreas.Length)
        {
            yield return new WaitForSeconds(enableDelay);

            windAreas[colliderCount].enabled = false;

            colliderCount++;          
        }

        yield break;
    }

    IEnumerator Interval()
    {
        borasSyst.Stop(withChildren);

        yield return new WaitForSeconds(intervalTime);

        blowTime = blowTimeStart;
        isBlowing = false;
    }
}
