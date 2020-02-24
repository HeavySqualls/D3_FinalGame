using UnityEngine;

public class CrumblingWall : BreakableObject
{
    protected override void Start()
    {
        base.Start();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider.gameObject.GetComponent<PlayerController>())
        {
            TriggerObjectShake();
        }
    }
}
