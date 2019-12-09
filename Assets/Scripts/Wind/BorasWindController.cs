using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorasWindController : MonoBehaviour
{
    public bool withChildren = true;
    public bool isBlowing = false;
    bool countdown = false;

    public float blowTimeStart = 2f;
    [SerializeField] private float blowTime;

    public float intervalTime = 5f;
    public float speed;
    public Transform startPos;
    private ParticleSystem borasSyst;
    public ParticleSystem detectSyst;
    public GameObject windArea;

    void Start()
    {
        borasSyst = GetComponentInChildren<ParticleSystem>();

        blowTime = blowTimeStart;
    }

    void Update()
    {
        BlowGust();
    }

    void  BlowGust()
    {
        if (!isBlowing)
        {
            borasSyst.Play(withChildren);
            countdown = true;
            isBlowing = true;
        }

        if (countdown && isBlowing)
        {
            blowTime -= Time.deltaTime;
            windArea.transform.Translate(Vector2.right * speed * Time.deltaTime);

            if (blowTime < 0)
            {
                StartCoroutine(Interval());
                countdown = false;
            }
        }
    }

    IEnumerator Interval()
    {
        print("break");
        windArea.transform.position = startPos.position;
        borasSyst.Stop(withChildren);

        yield return new WaitForSeconds(intervalTime);

        blowTime = blowTimeStart;
        isBlowing = false;
    }
}
