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
    [Header("Particle Effects:")]
    [SerializeField] GameObject disposablePartSyst;

    [Space]
    [Header("Audio")]
    [SerializeField] AudioClip smashSound;
    [SerializeField] float smashVolume = 0.5f;
    [SerializeField] AudioClip crushSound;
    [SerializeField] float crushVolume = 0.5f;
    [SerializeField] float lowPitchRange = 0.7f;
    [SerializeField] float highPitchRange = 1.5f;
    [SerializeField] float lowVolumeRange = 0.5f;
    [SerializeField] float highVolumeRange = 1f;
    [SerializeField] AudioSource smashSource;
    [SerializeField] AudioSource tankSource;
    [SerializeField] AudioSource debrisSource;

    [Space]
    [Header("Screen Shake:")]
    CinemachineVirtualCamera cam;
    Tween shakeTween;
    [SerializeField] float levelDuration;
    [SerializeField] float shakeStrength;
    [SerializeField] int virbrato;
    [SerializeField] float randomness;

    private void Start()
    {
        StartCoroutine(WaitToStart());
        currentMoveSpeed = baseMoveSpeed;
        cam = Toolbox.GetInstance().GetLevelManager().GetVirtualCam();

        Toolbox.GetInstance().GetAudioManager().AddAudioSources(smashSource);
        Toolbox.GetInstance().GetAudioManager().AddAudioSources(tankSource);
        Toolbox.GetInstance().GetAudioManager().AddAudioSources(debrisSource);
    }

    private void PlayVariedOneShot(AudioClip _clip, float _volume, bool playRandomVolume)
    {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        float randomVolume = Random.Range(lowVolumeRange, highVolumeRange);

        smashSource.pitch = randomPitch;

        if (playRandomVolume)
            smashSource.volume = randomVolume;
        else
            smashSource.volume = _volume;

        smashSource.PlayOneShot(_clip);
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

    public void Respawn(Transform _respawnLocation)
    {
        transform.position = _respawnLocation.position;
        StartCoroutine(WaitToStart());
        endchaseSpeed = false;
    }

    private void TrackDistanceBetweenObject()
    {
        distance = Vector3.Distance(transform.position, target.transform.position);
    }

    bool endchaseSpeed = false;

    public void IncreaseSpeed()
    {
        endchaseSpeed = true;
    }

    private void AdjustSpeedBasedOnDistance()
    {
        if (endchaseSpeed)
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, farMoveSpeed, currentAccelDecelTime * Time.deltaTime);
        }
        else
        {
            if (distance < closeDistance)
                currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, closeMoveSpeed, currentAccelDecelTime * Time.deltaTime);
            else if (distance > closeDistance && distance < baseDistance)
                currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, nearMoveSpeed, currentAccelDecelTime * Time.deltaTime);
            else if (distance > baseDistance && distance < distantDistance)
                currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, baseMoveSpeed, currentAccelDecelTime * Time.deltaTime);
            else if (distance > distantDistance && distance < farDistance)
                currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, distantMoveSpeed, currentAccelDecelTime * Time.deltaTime);
            else if (distance > farDistance)
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
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        GameObject objectHit = col.gameObject;       

        if (objectHit != null)
        {
            RecieveDamage objectRecieveDamage = objectHit.GetComponent<RecieveDamage>();

            if (objectRecieveDamage != null)
            {
                print("City killed " + objectHit.name);

                PlayerHealthSystem pHeathSyst = objectHit.gameObject.GetComponent<PlayerHealthSystem>();

                if (pHeathSyst != null && !pHeathSyst.isDead)
                {
                    pHeathSyst.KillPlayer();
                }
                else
                {
                    objectRecieveDamage.GetHit(Vector2.left, 100f, 0, 0, 0);
                }

                PlayVariedOneShot(crushSound, crushVolume, false);
            }
            else
            {
                print("City ran over " + objectHit.name);
                SmashingWallEffect(objectHit.transform.position);
                PlayVariedOneShot(smashSound, smashVolume, true);
            }  
        }
    }

    private void SmashingWallEffect(Vector3 _position)
    {
        disposablePartSyst = Instantiate(Resources.Load("CitySmash-Particle System", typeof(GameObject))) as GameObject;
        disposablePartSyst.transform.position = _position;
        Destroy(disposablePartSyst, 1.5f);
    }
}
