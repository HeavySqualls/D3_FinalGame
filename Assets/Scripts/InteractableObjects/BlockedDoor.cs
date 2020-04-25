using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class BlockedDoor : MonoBehaviour
{
    [Tooltip("This field gets auto-assigned and holds the reference to the player while they are in the trigger zone.")]
    private PlayerHandleInteract pInteract;
    [SerializeField] GameObject door;
    //[Tooltip("Assign the normal material assigned to the asset.")]
    //[SerializeField] Material normalMat;
    //[Tooltip("Assign the related highlighed 'Outline' shader for when the player is in the trigger zone.")]
    //[SerializeField] Material highlightMat;

    [Tooltip("Time it will take for the door to open.")]
    public float duration;
    [Tooltip("Add in the inspector the transform position into this variable when the door should be closed.")]
    public Vector3 closedPosition;
    [Tooltip("Add in the inspector the transform position into this variable when the door should be open.")]
    public Vector3 openPosition;

    private bool isOpen;
    public bool isInputBlocked;
    private float timer;
    private Vector3 currentPosition;

    [Space]
    [Header("Audio:")]
    [SerializeField] AudioClip openingCloseSound;
    [SerializeField] float desiredMaxVolume = 0.7f;
    [SerializeField] float volumeIncrementation = 0.05f;
    [SerializeField] float incrementTime = 0.08f;
    AudioSource audioSource;

    CinemachineVirtualCamera cam;
    Tween shakeTween;
    //SpriteRenderer spriteRenderer;

    private void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //spriteRenderer.material = normalMat;

        cam = Toolbox.GetInstance().GetLevelManager().GetVirtualCam();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = openingCloseSound;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        pInteract = other.gameObject.GetComponent<PlayerHandleInteract>();

        if (pInteract != null)
        {
            //spriteRenderer.material = highlightMat;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (pInteract != null)
        {
            //spriteRenderer.material = normalMat;
        }
    }

    public void OpenCloseDoor()
    {
        StopAllCoroutines();
        StartCoroutine(SlideDoor());
        StartCoroutine(FadeInOutDoorSound());
        shakeTween = cam.transform.DOShakePosition(duration, 0.10f, 10, 2f, false, true);
    }

    IEnumerator SlideDoor()
    {
        isInputBlocked = true;
        timer = 0.0f;
        while (timer < duration)
        {
            if (!isOpen)
            {
                currentPosition = Vector3.Lerp(closedPosition, openPosition, timer / duration);
            }
            else if (isOpen)
            {
                currentPosition = Vector3.Lerp(openPosition, closedPosition, timer / duration);
            }

            door.transform.localPosition = currentPosition;
            timer += Time.deltaTime;
            yield return null;
        }

        isOpen = !isOpen;

        if (!isOpen)
        {
            door.transform.localPosition = closedPosition; // makes sure the door is completely closed at the end 
        }
        else if (isOpen)
        {
            door.transform.localPosition = openPosition; // makes sure the door is completely open at the end 
        }

        isInputBlocked = false;
    }

    private IEnumerator FadeInOutDoorSound()
    {
        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < desiredMaxVolume)
        {
            audioSource.volume += volumeIncrementation;

            yield return new WaitForSeconds(incrementTime);
        }

        while (audioSource.volume > 0)
        {
            audioSource.volume -= volumeIncrementation;

            yield return new WaitForSeconds(incrementTime);
        }

        audioSource.Stop();

        yield break;
    }
}
