using System.Collections;
using UnityEngine;

public class EnemyHandleInteract : MonoBehaviour
{
    [Header("Red Flash:")]
    [Tooltip("The length of time the object will flash.")]
    public float flashDuration = 0.4f;
    private SpriteRenderer spriteRenderer;
    private float flashDelay = 0.1f;

    [Space]
    [Header("References:")]
    [Tooltip("If this is a moving unit, drag the related controller here.")]
    [SerializeField] Enemy_Base enemyBase;
    [Tooltip("If this is a turret-style unit, drag the related controller here.")]
    [SerializeField] Enemy_Turret_Base enemyTurretBase;

    [SerializeField] protected Transform hitboxPos;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(Vector2 _hitDirection, float _damage, float _knockBack, float _knockUp, float _stunTime)
    {
        if (enemyBase != null)
        {
            enemyBase.currentHP -= _damage;
            //print(gameObject.name + " was damaged!");

            StartCoroutine(enemyBase.ThisUnitHit(_hitDirection, _knockBack, _knockUp, _stunTime));
            StartCoroutine(IFlashRed());
        }
        else if (enemyTurretBase != null)
        {
            enemyTurretBase.currentHP -= _damage;
            //print(gameObject.name + " was damaged!");

            enemyTurretBase.ThisUnitHit(_hitDirection, _knockBack, _knockUp, _stunTime);
            StartCoroutine(IFlashRed());
        }
    }

    public IEnumerator IFlashRed()
    {
        for (float i = 0; i < flashDuration; i += flashDelay)
        {
            if (spriteRenderer.color == Color.white)
            {
                spriteRenderer.color = Color.red;
            }
            else if (spriteRenderer.color == Color.red)
            {
                spriteRenderer.color = Color.white;
            }

            yield return new WaitForSeconds(flashDelay);
        }

        spriteRenderer.color = Color.white;

        yield break;
    }
}
