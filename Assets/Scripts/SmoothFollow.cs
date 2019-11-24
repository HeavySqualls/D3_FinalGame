using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
    public GameObject objectToFollow;

    public float yOffsetRate = -0.75f;
    public float yOffsetStart = -2.15f;
    private float yOffset;

    public float speed = 2.0f;

    public bool isCamera;

    private PlayerController pCon;

    void Start()
    {
        if (isCamera)
        {
            // TODO: Change this reference to grab the player controller from GameManager once implemented!
            pCon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        yOffset = yOffsetStart;
    }

    void FixedUpdate()
    {
        float interpolation = speed * Time.deltaTime;

        Vector3 position = transform.position;
        position.y = Mathf.Lerp(transform.position.y, objectToFollow.transform.position.y - yOffset, interpolation);
        position.x = Mathf.Lerp(transform.position.x, objectToFollow.transform.position.x, interpolation);

        transform.position = position;

        if (isCamera)
        {
            VerticalMovementOffset();
        }
    }

    void VerticalMovementOffset()
    {
        if (pCon.velocity.y < -2 && !pCon.isGrounded && pCon.canClimbLedge == false)
        {
            yOffset = yOffset - yOffsetRate;
        }
        else
        {
            yOffset = yOffsetStart;
        }
    }
}
