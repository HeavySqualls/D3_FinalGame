using System.Collections;
using UnityEngine;

public class BorasWindController : MonoBehaviour
{
    public bool withChildren = true;
    public bool isBlowing = false;
    bool countdown = false;

    public float blowTimeStart = 2f;
    [SerializeField] private float blowTime;

    public float enableColliderTime = 1f;
    public float disableColliderTime = 1f;

    public float intervalTime = 5f;
    public float speed;
    public Transform startPos;
    private ParticleSystem borasSyst;
    public ParticleSystem detectSyst;
    public BoxCollider2D windArea;

    public WindColliderController windCollCon;
    void Start()
    {
        borasSyst = GetComponentInChildren<ParticleSystem>();
        windArea.enabled = false;
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

            if (blowTime < enableColliderTime)
            {
                windArea.enabled = true;
            }

            if (blowTime < disableColliderTime)
            {
                windArea.enabled = false;
            }

            if (blowTime < 0)
            {
                StartCoroutine(Interval());
                countdown = false;
            }
        }
    }

    IEnumerator Interval()
    {
        borasSyst.Stop(withChildren);

        yield return new WaitForSeconds(intervalTime);

        blowTime = blowTimeStart;
        isBlowing = false;
    }
}
