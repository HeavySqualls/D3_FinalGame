using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabwormGroupManager : MonoBehaviour
{
    public List<CrabwormLarvaController> crabwormGroup = new List<CrabwormLarvaController>();
    CrabwormLarvaController attackingCrabworm;
    public PlayerController pCon;

    private bool isAttacking = false;
    public bool isPlayerInZone;

    private void Update()
    {
        if (isPlayerInZone && crabwormGroup.Count > 0) // Track and coordinate attacks 
        {
            for (int i = 0; i < crabwormGroup.Count; i++)
            {
                if (crabwormGroup[i].isDead)
                {
                    crabwormGroup.Remove(crabwormGroup[i]);
                }
                else if (crabwormGroup[i] != attackingCrabworm && crabwormGroup[i].currentState != Enemy_Base.State.DoNothing)
                {
                    crabwormGroup[i].currentState = Enemy_Base.State.DoNothing;

                    if (crabwormGroup[i].target == null)
                    {
                        crabwormGroup[i].target = pCon.gameObject;
                    }

                    crabwormGroup[i].isUnitPaused = true;
                    crabwormGroup[i].FaceTarget();
                }
            }

            if (!isAttacking)
            {
                ChooseEnemyToAttack();
            }
        }
    }

    private void ChooseEnemyToAttack()
    {
        if (crabwormGroup.Count <= 0f) return;

        isAttacking = true;
        float shortestDist = 0;

        // Choosing closest enemy
        for (int i = 0; i < crabwormGroup.Count; i++)
        {
            float currentDist = Vector3.Distance(crabwormGroup[i].transform.position, pCon.transform.position);

            if (crabwormGroup[i] == crabwormGroup[0] || currentDist < shortestDist)
            {
                shortestDist = currentDist;
                attackingCrabworm = crabwormGroup[i];
            }
        }

        attackingCrabworm.isUnitPaused = false;
        attackingCrabworm.currentState = Enemy_Base.State.Hunting;

        StartCoroutine(AttackAndCooldown());
    }

    public float attackDelay = 0.3f;

    private IEnumerator AttackAndCooldown()
    {
        float currentDist = Vector3.Distance(attackingCrabworm.transform.position, pCon.transform.position);//3f // based on range of 0-5
        float randomAdd = Random.Range(-0.8f, 0.3f); // maKE VARS

        bool condition = currentDist > 1f || true;


        float randomFactor = condition ?  1f : currentDist / 3f;

        // randomAdd

        yield return new WaitForSeconds(attackDelay + randomAdd * randomFactor);//2 +-1

        //attackingCrabworm.isUnitPaused = true;
        //attackingCrabworm.currentState = Enemy_Base.State.DoNothing;
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CrabwormLarvaController crabCon = collision.gameObject.GetComponent<CrabwormLarvaController>();

        if (crabCon != null)
        {
            if (!crabwormGroup.Contains(crabCon) && !crabCon.isDead)
            {
                crabwormGroup.Add(crabCon);
            }
        }

        if (collision.gameObject.GetComponent<PlayerController>())
        {
            print("fuck");
            isPlayerInZone = true;
            pCon = collision.gameObject.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CrabwormLarvaController>())
        {
            CrabwormLarvaController crabCon = collision.GetComponent<CrabwormLarvaController>();
            crabwormGroup.Remove(crabCon);
        }
        else if (collision.gameObject.GetComponent<PlayerController>())
        {
            isPlayerInZone = false;
            print("why");
            foreach (CrabwormLarvaController waitingcrabCon in crabwormGroup)
            {
                waitingcrabCon.isUnitPaused = false;
                waitingcrabCon.currentState = Enemy_Base.State.Patrolling;
            }
        }
    }
}
