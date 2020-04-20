using UnityEngine;

public class SpawnZone_Chase : SpawnZone
{
    [Tooltip("The location that the chaser will respawn at.")]
    public Transform chaserRespawnLocation;
    [Tooltip("The unit that is chasing the player.")]
    public SilentCity chaser;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    public override void RespawnObject(GameObject _respawnee)
    {
        base.RespawnObject(_respawnee);

        if (chaser != null)
        {
            chaser.Respawn(chaserRespawnLocation);
            //chaser.gameObject.transform.position = chaserRespawnLocation.position;
        }
    }
}
