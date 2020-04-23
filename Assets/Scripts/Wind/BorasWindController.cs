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
    [SerializeField] private float blowIncrementValue = 0.01f;
    [SerializeField] private float blowIncrementTime = 1f;
    public bool isBlowTime = true;

    [Space]
    [Header("Wind Collider Variables:")]
    [Tooltip("The box collider where the wind affect will be applied.")]
    [SerializeField] BoxCollider2D[] windAreas;
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
    public bool isBlowing = false;
    private ParticleSystem borasSyst;
    private bool withChildren = true;

    void Start()
    {
        borasSyst = GetComponentInChildren<ParticleSystem>();

        foreach (BoxCollider2D wA in windAreas)
            wA.enabled = false;

        blowTime = blowTimeStart;
    }

    void Update()
    {
        if (!isBlowTime)
        {
            StartCoroutine(BlowGust());
            isBlowTime = true;
        }
    }

    IEnumerator BlowGust()
    {
        isBlowing = true;
        blowTime = blowTimeStart;
        borasSyst.Play(withChildren);

        while (blowTime > enableColliderTime)
        {
            blowTime -= blowIncrementValue;
            yield return new WaitForSeconds(blowIncrementTime);
        }

        StartCoroutine(EnableWindColliders());

        while (blowTime > disableColliderTime)
        {
            blowTime -= blowIncrementValue;
            yield return new WaitForSeconds(blowIncrementTime);
        }

        StartCoroutine(DisableWindColliders());

        while (blowTime > 0)
        {
            blowTime -= blowIncrementValue;
            yield return new WaitForSeconds(blowIncrementTime);
        }

        StartCoroutine(Interval());
        borasSyst.Stop(withChildren);
        isBlowing = false;

        yield break;
    }

    IEnumerator Interval()
    {
        print("Start boras interval");

        yield return new WaitForSeconds(intervalTime);
        isBlowTime = false;

        print("Stop boras interval");
        yield break;
    }

    //void  BlowGust()
    //{
    //    // Check to play the particle system once
    //    if (!isBlowing)
    //    {
    //        print("Blow Boras Wind");
    //        borasSyst.Play(withChildren);
    //        isBlowing = true;
    //    }

    //    if (isBlowing)
    //    {
    //        blowTime -= Time.deltaTime;

    //        // Start to enable the colliders one by one in sequence
    //        if (!enableColliders && blowTime < enableColliderTime && blowTime > disableColliderTime)
    //        {
    //            StartCoroutine(EnableWindColliders());
    //            enableColliders = true;
    //        }
    //        // Start to disable the colliders one by one in sequence when the blowing time has reached its peak point
    //        else if (enableColliders && blowTime < disableColliderTime)
    //        {
    //            StartCoroutine(DisableWindColliders());
    //            enableColliders = false;
    //        }
    //        // If we have reached the end of the blow time, start the interval countdown and set is blowing countdown to false
    //        else if (blowTime <= 0)
    //        {
    //            StartCoroutine(Interval());
    //            isBlowTime = false;
    //        }
    //    }
    //}

    IEnumerator EnableWindColliders()
    {
        print("Enable wind colliders");
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
        print("Disable wind colliders");
        int colliderCount = 0;

        while (colliderCount < windAreas.Length)
        {
            yield return new WaitForSeconds(enableDelay);

            windAreas[colliderCount].enabled = false;

            colliderCount++;          
        }

        yield break;
    }
}
