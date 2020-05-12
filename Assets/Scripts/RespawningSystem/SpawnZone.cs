using System.Collections;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [SerializeField] PlayerHealthSystem pHealthSyst;
    GameObject disposablePartSyst;
    [SerializeField] AudioClip respawnSound;
    [SerializeField] float respawnVolume = 0.3f;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        pHealthSyst = collision.gameObject.GetComponent<PlayerHealthSystem>();

        if (pHealthSyst != null && pHealthSyst.spawnZone != this)
        {
            pHealthSyst.spawnZone = this;
            Toolbox.GetInstance().GetAudioManager().PlayConsistentOneShot(respawnSound, respawnVolume);

            disposablePartSyst = Instantiate(Resources.Load("RespawnParticleSystem", typeof(GameObject))) as GameObject;
            disposablePartSyst.transform.position = gameObject.transform.position;
            Destroy(disposablePartSyst, 2f);
        }
    }

    public virtual void RespawnObject(GameObject _respawnee)
    {
        _respawnee.transform.position = transform.position;
        print("Respawn Player");
    }
}
