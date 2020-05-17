using UnityEngine;

public class CrabwormTriggerZones : MonoBehaviour
{
    [Tooltip("Drag in the crabworm that will react to this trigger zone here.")]
    [SerializeField] CrabwormLarvaController crabwormCon;

    public bool isActive = true;

    private void Start()
    {
        Toolbox.GetInstance().GetLevelManager().AddcwTriggerZone(this);
    }

    private void OnEnable()
    {
        SpawnManager.onResetLevelObjects += EnableTrigger;
    }

    private void OnDisable()
    {
        SpawnManager.onResetLevelObjects -= EnableTrigger;
    }

    private void EnableTrigger()
    {
        isActive = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive && !crabwormCon.isDead)
        {
            if (other.gameObject.tag == "Player")
            {
                crabwormCon.TriggerZoneAttackCall(other.gameObject);
                isActive = false;
            }
        }
    }
}
