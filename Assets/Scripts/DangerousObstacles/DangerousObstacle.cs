using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousObstacle : Interact_Base
{
    [Tooltip("The delay until the interacted object will be attacked again if still standing on the spikes")]
    public float damageDelay = 0.5f;
    private bool isDamageDelay = false;

    [Tooltip("Spikes and debris that will become bloodied")]
    public HashSet<MeshRenderer> spikeMeshes = new HashSet<MeshRenderer>();

    [Space]
    [Header("Audio:")]
    [SerializeField] AudioClip hitSound;
    [SerializeField] float hitSoundVolume = 0.3f;
    AudioManager AM;

    void Start()
    {
        FindEveryChild(gameObject.transform);
        AM = Toolbox.GetInstance().GetAudioManager();
    }

    //private void PlayAudio(AudioClip _clip, float _volume)
    //{
    //    audioSource.volume = _volume;
    //    audioSource.PlayOneShot(_clip);
    //}

    private void FindEveryChild(Transform parent)
    {
        int count = parent.childCount;

        for (int i = 0; i < count; i++)
        {
            spikeMeshes.Add(parent.GetChild(i).transform.GetComponent<MeshRenderer>());
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (pRecieveDamage != null)
        {
            pRecieveDamage.GetHit(hitDirection, damage, knockBack, knockUp, stunTime);
            AM.PlayVariedOneShot(hitSound, hitSoundVolume);

            foreach (MeshRenderer spikeMR in spikeMeshes)
            {
                spikeMR.material.SetColor("_Color", Color.red);
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
