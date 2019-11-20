using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{
    public GameObject objectToFollow;

    public float yOffset;
    public float speed = 2.0f;

    void Update()
    {
        float interpolation = speed * Time.deltaTime;

        Vector3 position = transform.position;
        position.y = Mathf.Lerp(transform.position.y, objectToFollow.transform.position.y - yOffset, interpolation);
        position.x = Mathf.Lerp(transform.position.x, objectToFollow.transform.position.x, interpolation);

        transform.position = position;
    }
}
