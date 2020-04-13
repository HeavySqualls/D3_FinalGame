using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerFeedback : MonoBehaviour
{
    [Space]
    [Header("Particle Systems:")]
    public Vector3 groundPositionOffset = new Vector3(0, 1.5f, 0);
    public Vector3 attackPositionOffset = new Vector3(1.5f, -1.5f, 0);
    public Vector3 attackPositionOffsetALT = new Vector3(1.5f, 1.5f, 0);
    private bool isRunningSystem = false;
    private bool isWallSlidingSystem = false;
    private bool isSlidingSurfaceSystem = false;
    [SerializeField] ParticleSystem runningPartSyst;
    [SerializeField] ParticleSystem slidingPartSyst;
    [SerializeField] ParticleSystem jumpingTrailPartSyst;
    [SerializeField] ParticleSystem heavyAttackPartSyst;
    GameObject disposablePartSyst;

    [Space]
    [Header("RED FLASH:")]
    [Tooltip("The length of time the object will flash.")]
    public float flashDuration = 0.4f;
    private SpriteRenderer spriteRenderer;
    private float flashDelay = 0.1f;

    [Space]
    [Header("CAMERA SHAKE:")]
    [Tooltip("The length of time the object will shake.")]
    public float shakeDuration = 0.8f;
    [Tooltip("The strength of the shake.")]
    public float shakeStrength = 0.25f;
    [Tooltip("The number of shakes that will occur during the duration of the shake.")]
    public int vibrato = 9;
    [Tooltip("The randomness of the shake postitions.")]
    public float randomness = 2f;
    Camera cam;
    Tween shakeTween;
    PlayerController pCon;

    void Start()
    {
        pCon = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;
    }

    private void Update()
    {
        RunningParticles();
        SlidingSurfaceParticles();
        WallSlidingParticles();
    }


    // <<----------------------------------------------------- PARTICLE SYSTEMS ------------------------------------------- //

    public void AttackParticles(string _particleName)
    {
        disposablePartSyst = Instantiate(Resources.Load(_particleName, typeof(GameObject))) as GameObject;

        if (pCon.accessibleDirection == Vector2.right)
            disposablePartSyst.transform.position = gameObject.transform.position + attackPositionOffset;
        else
            disposablePartSyst.transform.position = gameObject.transform.position - attackPositionOffsetALT;

        Destroy(disposablePartSyst, 1f);
    }

    //public void MediumAttackParticles()
    //{
    //    disposablePartSyst = Instantiate(Resources.Load("AttackParticleSystem-Medium", typeof(GameObject))) as GameObject;

    //    if (pCon.accessibleDirection == Vector2.right)
    //        disposablePartSyst.transform.position = gameObject.transform.position + attackPositionOffset;
    //    else
    //        disposablePartSyst.transform.position = gameObject.transform.position - attackPositionOffsetALT;

    //    Destroy(disposablePartSyst, 1f);
    //}

    private void RunningParticles()
    {
        if (pCon.isMoving && pCon.isGrounded && !isRunningSystem)
        {
            print("start running");
            runningPartSyst.Play();
            isRunningSystem = true;
        }
        else if ((!pCon.isMoving || !pCon.isGrounded) && isRunningSystem)
        {
            print("stop running");
            runningPartSyst.Stop();
            isRunningSystem = false; 
        }
    }

    private void SlidingSurfaceParticles()
    {
        if (pCon.isGroundSliding && !isSlidingSurfaceSystem && !isRunningSystem)
        {
            print("start surface sliding");
            slidingPartSyst.Play();
            isSlidingSurfaceSystem = true;
        }
        else if (!pCon.isGroundSliding && isSlidingSurfaceSystem)
        {
            print("stop surface sliding");
            slidingPartSyst.Stop();
            isSlidingSurfaceSystem = false;
        }
    }

    private void WallSlidingParticles()
    {
        if (pCon.isWallSliding && !isWallSlidingSystem && !isRunningSystem)
        {
            print("start wall sliding");
            slidingPartSyst.Play();
            isWallSlidingSystem = true;
        }
        else if (!pCon.isWallSliding && isWallSlidingSystem)
        {
            print("stop wall sliding");
            slidingPartSyst.Stop();
            isWallSlidingSystem = false;
        }
    }

    public void JumpingParticleEffect()
    {
        jumpingTrailPartSyst.Play();
        disposablePartSyst = Instantiate(Resources.Load("JumpingParticleSystem", typeof(GameObject))) as GameObject;
        disposablePartSyst.transform.position = gameObject.transform.position + groundPositionOffset;
        Destroy(disposablePartSyst, 1f);
    }

    public void LightLandingParticles()
    {
        disposablePartSyst = Instantiate(Resources.Load("LandingParticleSystem-Light", typeof(GameObject))) as GameObject;
        disposablePartSyst.transform.position = gameObject.transform.position + groundPositionOffset;
        Destroy(disposablePartSyst, 1.5f);
    }

    public void HeavyLandingParticles()
    {
        disposablePartSyst = Instantiate(Resources.Load("LandingParticleSystem-Heavy", typeof(GameObject))) as GameObject;
        disposablePartSyst.transform.position = gameObject.transform.position + groundPositionOffset;
        Destroy(disposablePartSyst, 1.5f);
    }


    // <<----------------------------------------------------- SPRITE FLASH ------------------------------------------- //

    public IEnumerator IFlashRed()
    {
        shakeTween = cam.transform.DOShakePosition(0.8f, 0.25f, 9, 2f, false, true);

        for (float i = 0; i < flashDuration; i += flashDelay)
        {
            if (spriteRenderer.color == Color.white)
            {
                spriteRenderer.color = Color.red;
            }
            else if (spriteRenderer.color == Color.red)
            {
                spriteRenderer.color = Color.white;
            }

            yield return new WaitForSeconds(flashDelay);
        }

        spriteRenderer.color = Color.white;

        yield break;
    }
}
