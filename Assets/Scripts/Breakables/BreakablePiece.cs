using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePiece : MonoBehaviour
{
    public bool isHit = false;
    private bool isShake = false;

    [Tooltip("The length of time the object will shake.")]
    public float shakeDuration;
    [Tooltip("The point during the shake at which the shake strength will begin to decrease back to 0.")]
    public float decreasePoint;

    Vector3 startingPos; // TODO: ---- >> for respawning ledges
    Rigidbody2D rb2D;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        startingPos = transform.position;
    }

    void Update()
    {
        if (isHit)
        {
            ShakeGameObject(gameObject, shakeDuration, decreasePoint, false);
        }
    }

    public void DestroyWall(Vector2 _dir, float _dmg, float _knockback, float _knockUp)
    {
        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.AddForce(_dir * Random.Range(100f, 600f));
        Destroy(gameObject, Random.Range(0.5f, 1.5f));
    }

    void ShakeGameObject(GameObject _objectToShake, float _shakeDuration, float _decreasePoint, bool _objectIs2D = false)
    {
        if (isShake)
        {
            StopCoroutine(shakeGameObjectCOR(_objectToShake, _shakeDuration, _decreasePoint, _objectIs2D));
        }

        isShake = true;

        StartCoroutine(shakeGameObjectCOR(_objectToShake, _shakeDuration, _decreasePoint, _objectIs2D));
    }

    IEnumerator shakeGameObjectCOR(GameObject objectToShake, float totalShakeDuration, float decreasePoint, bool objectIs2D = false)
    {
        if (decreasePoint >= totalShakeDuration)
        {
            Debug.LogError("decreasePoint must be less than totalShakeDuration...Exiting");
            yield break; //Exit!
        }

        //Get Original Pos and rot
        Transform objTransform = objectToShake.transform;
        Vector3 defaultPos = objTransform.position;
        Quaternion defaultRot = objTransform.rotation;

        float counter = 0f;

        //Shake Speed
        const float speed = 0.05f;

        //Angle Rotation(Optional)
        const float angleRot = 3;

        //Do the actual shaking
        while (counter < totalShakeDuration)
        {
            counter += Time.deltaTime;
            float decreaseSpeed = speed;
            float decreaseAngle = angleRot;

            //Shake GameObject
            if (objectIs2D)
            {
                //Don't Translate the Z Axis if 2D Object
                Vector3 tempPos = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseSpeed;
                tempPos.z = defaultPos.z;
                objTransform.position = tempPos;

                //Only Rotate the Z axis if 2D
                objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-angleRot, angleRot), new Vector3(0f, 0f, 1f));
            }
            else
            {
                objTransform.position = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseSpeed;
                objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-angleRot, angleRot), new Vector3(1f, 1f, 1f));
            }
            yield return null;


            //Check if we have reached the decreasePoint then start decreasing  decreaseSpeed value
            if (counter >= decreasePoint)
            {
                Debug.Log("Decreasing shake");

                //Reset counter to 0 
                counter = 0f;
                while (counter <= decreasePoint)
                {
                    counter += Time.deltaTime;
                    decreaseSpeed = Mathf.Lerp(speed, 0, counter / decreasePoint);
                    decreaseAngle = Mathf.Lerp(angleRot, 0, counter / decreasePoint);

                    Debug.Log("Decrease Value: " + decreaseSpeed);

                    //Shake GameObject
                    if (objectIs2D)
                    {
                        //Don't Translate the Z Axis if 2D Object
                        Vector3 tempPos = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseSpeed;
                        tempPos.z = defaultPos.z;
                        objTransform.position = tempPos;

                        //Only Rotate the Z axis if 2D
                        objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-decreaseAngle, decreaseAngle), new Vector3(0f, 0f, 1f));
                    }
                    else
                    {
                        objTransform.position = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseSpeed;
                        objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-decreaseAngle, decreaseAngle), new Vector3(1f, 1f, 1f));
                    }
                    yield return null;
                }

                //Break from the outer loop
                break;
            }
        }
        objTransform.position = defaultPos; //Reset to original postion
        objTransform.rotation = defaultRot;//Reset to original rotation

        isShake = false; //So that we can call this function next time
        isHit = false;
        Debug.Log("Done!");
    }
}
