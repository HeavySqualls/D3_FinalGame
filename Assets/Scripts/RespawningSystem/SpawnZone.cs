using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [SerializeField] PlayerHealthSystem pHealthSyst;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        pHealthSyst = collision.gameObject.GetComponent<PlayerHealthSystem>();

        if (pHealthSyst != null)
        {
            pHealthSyst.spawnZone = this; 
        }
    }

    public virtual void RespawnObject(GameObject _respawnee)
    {
        //SpawnManager.ResetLevelObjects();

        _respawnee.transform.position = transform.position;
        //Toolbox.GetInstance().GetLevelManager().ResetLevelObjects();
        print("Respawn Player");
    }
}
