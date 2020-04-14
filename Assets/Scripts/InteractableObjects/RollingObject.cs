using UnityEngine;

public class RollingObject : MonoBehaviour
{
    public Vector2 direction;
    private Vector3 startPos;
    public Vector3 groundPositionOffset = new Vector3(0, 1.5f, 0);
    GameObject disposablePartSyst;
    Rigidbody2D rb2D;
    LayerMask groundLayerMask;
    int groundLayer = 8;
    int breakableFloorsLayer = 17;
    public float groundCheckDistance = 2f;

    private void OnEnable()
    {
        SpawnManager.onResetLevelObjects += ResetRollingObject;
    }

    private void OnDisable()
    {
        SpawnManager.onResetLevelObjects -= ResetRollingObject;
    }

    void Start()
    {
        groundLayerMask = ((1 << groundLayer)) | ((1 << breakableFloorsLayer));
        startPos = gameObject.transform.position;
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckForGround();
    }

    public void TakeDamage(Vector2 _hitDir, float _damage, float _knockBack, float _knockUp, float _stun)
    {
        print("hit");
        rb2D.AddForce(_hitDir * (_damage * 100));
        direction = _hitDir;
    }

    public void ResetRollingObject()
    {
        gameObject.transform.position = startPos;
    }

    bool shakeOnLanding = false;

    private void CheckForGround()
    {
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayerMask);
        Debug.DrawRay(transform.position, Vector2.down * groundCheckDistance, Color.red);

        if (hitGround.collider == null && shakeOnLanding == false)
        {
            shakeOnLanding = true;
        }

        if (hitGround.collider != null && shakeOnLanding == true)
        {
            Toolbox.GetInstance().GetPlayerManager().GetPlayerFeedback().BreakShake();
            disposablePartSyst = Instantiate(Resources.Load("LandingParticleSystem-Heavy", typeof(GameObject))) as GameObject;
            disposablePartSyst.transform.position = gameObject.transform.position - groundPositionOffset;
            Destroy(disposablePartSyst, 1.5f);
            shakeOnLanding = false;
        }
    }
}
