using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindOnUnit : MonoBehaviour
{
    public bool inWindZone = false;
    private bool affectUnit = true;
    public WindArea windZone;
    public Transform respawnZone;

    private PlayerController pCon;

    void Start()
    {
        pCon = GetComponent<PlayerController>();
    }

    void Update()
    {
        //if (!pCon.isOnWall && inWindZone && pCon.magBootsOn == false)
        //{
        //    pCon.rb2d.velocity = windZone.direction * windZone.strength;
        //    print(pCon.rb2d.velocity);
        //}
        //else if (inWindZone && pCon.isOnWall)
        //{
        //    print("Hit wall");
        //    //pCon.AutoMove();
        //}
        if (affectUnit)
        {
            if (!pCon.isOnWall && inWindZone && pCon.magBootsOn == false)
            {
                pCon.rb2d.velocity = windZone.direction * windZone.strength;
                print(pCon.rb2d.velocity);
            }
            else if (inWindZone && pCon.isOnWall)
            {
                print("Hit wall");
                if (pCon.velocity.x > 0.01f)
                {
                    transform.position = new Vector2(transform.position.x - -0.2f, transform.position.y);
                }
                else
                {
                    transform.position = new Vector2(transform.position.x - 0.2f, transform.position.y);
                }

                //if (pCon.pIsFlipped)
                //{
                //    transform.position = new Vector2(transform.position.x - -0.2f, transform.position.y);
                //}
                //else
                //{
                //    transform.position = new Vector2(transform.position.x - 0.2f, transform.position.y);
                //}
                
                affectUnit = false;
            }
        }

        if (!affectUnit && Input.GetButton("Horizontal"))
        {
            StartCoroutine(Countdown());
        }
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(0.5f);
        affectUnit = true;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "WindZone")
        {
            print("Entered wind zone");
            windZone = coll.gameObject.GetComponent<WindArea>();
            inWindZone = true;
        }

        if (coll.gameObject.tag == "DeathZone")
        {
            print("Dead");
            pCon.gameObject.transform.position = respawnZone.position;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "WindZone")
        {
            pCon.rb2d.velocity = Vector3.zero;
            windZone = null;
            inWindZone = false;
        }
    }
}
