using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeManager : MonoBehaviour
{
    public IEnumerator shakeGameObjectCOR(GameObject _objectToShake, float _totalShakeDuration, float _decreasePoint, float _speed, float _angleRot, bool _objectIs2D = false)
    {
        if (_decreasePoint >= _totalShakeDuration)
        {
            Debug.LogError("decreasePoint must be less than totalShakeDuration...Exiting");
            yield break; //Exit!
        }

        //Get Original Pos and rot
        Transform objTransform = _objectToShake.transform;
        Vector3 defaultPos = objTransform.position;
        Quaternion defaultRot = objTransform.rotation;

        float counter = 0f;

        //Shake Speed
        float speed = _speed;

        //Angle Rotation(Optional)
        float angleRot = _angleRot;

        //Do the actual shaking
        while (counter < _totalShakeDuration)
        {
            counter += Time.deltaTime;
            float decreaseSpeed = speed;
            float decreaseAngle = angleRot;

            //Shake GameObject
            if (_objectIs2D)
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
            if (counter >= _decreasePoint)
            {
                //Debug.Log("Decreasing shake");

                //Reset counter to 0 
                counter = 0f;
                while (counter <= _decreasePoint)
                {
                    counter += Time.deltaTime;
                    decreaseSpeed = Mathf.Lerp(speed, 0, counter / _decreasePoint);
                    decreaseAngle = Mathf.Lerp(angleRot, 0, counter / _decreasePoint);

                    //Debug.Log("Decrease Value: " + decreaseSpeed);

                    //Shake GameObject
                    if (_objectIs2D)
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
        //Debug.Log("Done!");
    }
}
