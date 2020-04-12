using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    //[SerializeField] int spawnLocationID = 1;

    [SerializeField] PlayerHealthSystem pHealthSyst;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        pHealthSyst = collision.gameObject.GetComponent<PlayerHealthSystem>();

        if (pHealthSyst != null)
        {
            //Toolbox.GetInstance().GetLevelManager().AddSpawnLocations(this);
            pHealthSyst.spawnZone = this; 
        }
    }

    public virtual void RespawnObject(GameObject _respawnee)
    {
        _respawnee.transform.position = transform.position;
        Toolbox.GetInstance().GetLevelManager().ResetLevelObjects();
        print("Respawn Player");
    }
}
