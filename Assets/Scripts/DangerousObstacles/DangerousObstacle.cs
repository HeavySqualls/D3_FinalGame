using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousObstacle : Interact_Base
{
    [Tooltip("The delay until the interacted object will be attacked again if still standing on the spikes")]
    public float damageDelay = 0.5f;
    private bool isDamageDelay = false;
    private bool isBloodied = false;

    [SerializeField] Material baseMat;
    [SerializeField] Material bloodiedMat;

    [Tooltip("Spikes and debris that will become bloodied")]
    public HashSet<MeshRenderer> spikeMeshes = new HashSet<MeshRenderer>();

    [Space]
    [Header("Audio:")]
    [SerializeField] AudioClip hitSound;
    [SerializeField] float hitSoundVolume = 0.3f;
    AudioManager AM;

    private void OnEnable()
    {
        SpawnManager.onResetLevelObjects += ResetDangerousSpikes;
    }

    private void OnDisable()
    {
        SpawnManager.onResetLevelObjects -= ResetDangerousSpikes;
    }

    void Start()
    {
        FindEveryChild(gameObject.transform);
        AM = Toolbox.GetInstance().GetAudioManager();
    }


    public void ResetDangerousSpikes()
    {
        if (isBloodied)
        {
            foreach (MeshRenderer spikeMR in spikeMeshes)
            {
                spikeMR.material = baseMat;
            }
        }
    }

    private void FindEveryChild(Transform parent)
    {
        int count = parent.childCount;

        for (int i = 0; i < count; i++)
        {
            if (parent.GetChild(i).gameObject.tag == "Spike")
            {
                spikeMeshes.Add(parent.GetChild(i).transform.GetComponent<MeshRenderer>());
            }
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (pRecieveDamage != null)
        {
            pRecieveDamage.GetHit(hitDirection, damage, knockBack, knockUp, stunTime);
            AM.PlayVariedOneShot(hitSound, hitSoundVolume);

            isBloodied = true;

            foreach (MeshRenderer spikeMR in spikeMeshes)
            {
                spikeMR.material = bloodiedMat;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (pRecieveDamage != null && !isDamageDelay)
        {
            StartCoroutine(DamageDelay());
        }
    }

    IEnumerator DamageDelay()
    {
        isDamageDelay = true;

        yield return new WaitForSeconds(damageDelay);

        if (pRecieveDamage != null)
        {
            pRecieveDamage.GetHit(hitDirection, damage, knockBack, knockUp, stunTime);
        }

        isDamageDelay = false;
    }
}
