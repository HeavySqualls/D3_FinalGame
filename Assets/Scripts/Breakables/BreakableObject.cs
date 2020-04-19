using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Space]
    [Header("Type:")]
    [Tooltip("Will this object respawn?")]
    public bool isRottenDoor = false;
    [Tooltip("Is this object a platform?")]
    public bool isPlatform = false;
    [Tooltip("Is this object a crumbling wall?")]
    public bool isCrumblingWall = false;
    [Tooltip("Is this object a breakable floor?")]
    public bool isBreakableFloor = false;
    [Tooltip("Will this object respawn?")]
    public bool isRespawnable = false;
    [HideInInspector]
    public bool isFallingApart = false;
    private bool isBroken = false;

    [Space]
    [Header("Particle System:")]
    [SerializeField] protected ParticleSystem damagePartSyst;
    [SerializeField] protected ParticleSystem brokenPartSyst;

    [Space]
    [Header("Variables:")]
    public float startHP;
    private float currentHP;
    [Tooltip("The length of time the object will shake.")]
    public float shakeDuration;
    [Tooltip("The number of shakes that will occur during the duration of the shake.")]
    public int vibrato;
    [Tooltip("The degree of visual elasticity of the objects being shaken.")]
    public float strength;
    [Tooltip("The time it takes for the platform to respawn.")]
    public float respawnTime;
    public bool hitByHeavyObject = false;
    private bool hitByPlayer = false;
    private Vector2 boulderHitFromDirection;

    [Space]
    [Header("Audio:")]
    [SerializeField] AudioClip hitSound;
    [SerializeField] float hitSoundVolume = 0.3f;
    [SerializeField] AudioClip breakSound;
    [SerializeField] float breakSoundVolume = 0.3f;
    AudioSource audioSource;

    [Space]
    [Header("OBJECT PIECES:")]
    public List<BreakablePiece> objPieces = new List<BreakablePiece>();
    public List<BreakablePiece> earlyBreakPieces = new List<BreakablePiece>();
    protected BoxCollider2D boxCollider;

    private void OnEnable()
    {
        SpawnManager.onResetLevelObjects += ResetObject;
    }

    private void OnDisable()
    {
        SpawnManager.onResetLevelObjects -= ResetObject;
    }

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        currentHP = startHP;

        if (!isRespawnable)
        {
            Toolbox.GetInstance().GetLevelManager().AddBreakableObjects(this);
        }

        FindEveryChild(gameObject.transform);
    }

    private void PlayAudio(AudioClip _clip, float _volume)
    {
        audioSource.volume = _volume;
        audioSource.PlayOneShot(_clip);
    }

    protected virtual void ResetObject()
    {
        if (isBroken)
        {
            RespawnObject();
        }
    }

    // Finds every child in the game object with a BreakablePiece component and adds it to the list 
    private void FindEveryChild (Transform parent)
    {
        int count = parent.childCount;

        for (int i = 0; i < count; i++)
        {
            if (!parent.GetChild(i).transform.GetComponent<BreakablePiece>().isEarlyBreakPiece)
            {
                objPieces.Add(parent.GetChild(i).transform.GetComponent<BreakablePiece>());
            }          
        }
    }

    // Determines if this object has been hit by a rolling object & is somthing other than a breakable floor, if so get direction of the rolling object and destroy this object
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.gameObject.GetComponent<RollingObject>() && !isBreakableFloor && !isCrumblingWall)
        {
            hitByHeavyObject = true;
            boulderHitFromDirection = other.collider.gameObject.GetComponent<RollingObject>().direction;
            StartCoroutine(CollapseAndRespawnCounter());
        }
    }


    // Implements damage to the object with variables passed on by the RecieveDamage component 
    public void TakeDamage(Vector2 _hitDir, float _dmg, float _knockback, float _knockUp, float _stunned)
    {
        currentHP -= _dmg;

        if (currentHP <= 0f) // if object has no more hit points, destroy
        {
            Toolbox.GetInstance().GetPlayerManager().GetPlayerFeedback().BreakShake();
            hitByPlayer = true;
            StartCoroutine(CollapseAndRespawnCounter());
            //Debug.Log("Wall is broken");
            //boxCollider.enabled = false;
            //isBroken = true;
            //BlowOutObject();
        }
        else // if object still has hit points, shake
        {
            ShakeObject();
        }
    }


    // Shakes the floor when the player lands on it
    public void TriggerObjectShake()
    {
        damagePartSyst.Play();
        ShakeObject();
    }


    // Shake platform and then trigger platform collapse & respawn coroutine
    public void TriggerPlatformCollapse()
    {       
        ShakeObject();

        StartCoroutine(CollapseAndRespawnCounter());
    }


    // Drop the pre-selected early drop pieces when the player lands on the platform
    IEnumerator EarlyBreakDrop(BreakablePiece _ebp)
    {
        yield return new WaitForSeconds(0.04f);
        _ebp.DropPiece();

        yield break;
    }


    protected IEnumerator CollapseAndRespawnCounter()
    {
        isFallingApart = true;

        // If the object has not been hit by a rolling object, wait for shake to finish
        if (!hitByHeavyObject && !hitByPlayer)
        {
            print("poo");
            yield return new WaitForSeconds(shakeDuration);
        }

        // Disable box collider on parent object
        boxCollider.enabled = false;
        isBroken = true;
        PlayAudio(breakSound, breakSoundVolume);

        // If the object has been hit by a rolling object
        if ((hitByHeavyObject || hitByPlayer) && !isBreakableFloor /*|| isRottenDoor*/)
        {
            BlowOutObject();
        }
        else // let gravity take its course...
        {
            DropObject();
        }

        yield return new WaitForSeconds(2); // wait for a few seconds - then hide breakable pieces

        HideObject();

        hitByHeavyObject = false;

        // if the object IS respawnable, wait the chosen amount of time, then reset in the original starting position
        if (isRespawnable)
        {
            yield return new WaitForSeconds(respawnTime);

            isFallingApart = false;
            boxCollider.enabled = true;

            RespawnObject();
            yield break;
        }  

        yield break;
    }

    private void ShakeObject()
    {
        damagePartSyst.Play();
        PlayAudio(hitSound, hitSoundVolume);

        foreach (BreakablePiece bp in objPieces)
        {
            bp.ShakePiece(bp.gameObject, shakeDuration, strength, vibrato);
        }

        if (earlyBreakPieces.Count > 0)
        {
            foreach (BreakablePiece ebp in earlyBreakPieces)
            {
                StartCoroutine(EarlyBreakDrop(ebp));
            }
        }
    }

    private void DropObject()
    {
        brokenPartSyst.Play();

        foreach (BreakablePiece bp in objPieces)
        {
            bp.DropPiece();
        }

        if (earlyBreakPieces.Count > 0)
        {
            foreach (BreakablePiece ebp in earlyBreakPieces)
            {
                ebp.DropPiece();
            }
        }
    }

    private void BlowOutObject()
    {
        brokenPartSyst.Play();

        foreach (BreakablePiece bp in objPieces)
        {
            bp.BlowOutPiece(boulderHitFromDirection, isPlatform);
        }

        if (earlyBreakPieces.Count > 0)
        {
            foreach (BreakablePiece ebp in earlyBreakPieces)
            {
                ebp.BlowOutPiece(boulderHitFromDirection, isPlatform);
            }
        }
    }

    private void HideObject()
    {
        Debug.Log("Hide Object");
        foreach (BreakablePiece bp in objPieces)
        {
            bp.HidePiece();
        }

        if (earlyBreakPieces.Count > 0)
        {
            foreach (BreakablePiece ebp in earlyBreakPieces)
            {
                ebp.HidePiece();
            }
        }
    }

    private void RespawnObject()
    {
        Debug.Log("Respawn Object");

        boxCollider.enabled = true;

        foreach (BreakablePiece bp in objPieces)
        {
            bp.RespawnPiece(isCrumblingWall);
        }

        if (earlyBreakPieces.Count > 0)
        {
            foreach (BreakablePiece ebp in earlyBreakPieces)
            {
                ebp.RespawnPiece(isCrumblingWall);
            }
        }

        currentHP = startHP;
        isBroken = false;
        hitByHeavyObject = false;
        hitByPlayer = false;
    }
}
