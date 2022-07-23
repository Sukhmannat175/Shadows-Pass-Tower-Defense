/*  Filename:           InventoryManager.cs
 *  Author:             Sukhmannat Singh (301168420)
 *  Last Update:        June 26, 2022
 *  Description:        Inventory Manager.
 *  Revision History:   June 26, 2022 (Sukhmannat Singh): Initial script.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourseStealerController : EnemyBaseBehaviour
{
    [Header("Resource Stealer")]
    [SerializeField] private int gold;
    [SerializeField] private int stone;
    [SerializeField] private int wood;
    [SerializeField] private string animationStateParameterName;
    [SerializeField] private int walkState;
    [SerializeField] private int digState;
    [SerializeField] private int actionDelay;
    [SerializeField] private int stealTime;
    [SerializeField] private int actionTime;

    [Header("Debug")]
    [SerializeField] private EnemyState state;

    [HideInInspector] public EnemyData enemyData;

    private Animator animator;
    private int steal;

    public IEnumerator Dig()
    {
        while (true)
        {
            if (state == EnemyState.WALK)
            {
                yield return new WaitForSeconds(actionDelay);
                navMeshAgent.speed = 0;
                animator.SetInteger(animationStateParameterName, digState);
                yield return new WaitForSeconds(2);
                state = EnemyState.DIG;
            }
            yield return null;
        }
    }

    public IEnumerator StealResources()
    {
        while (true)
        {
            steal = stealTime;
            while (steal > 0 && state == EnemyState.DIG)
            {
                yield return new WaitForSeconds(actionTime);

                navMeshAgent.speed = speed;
                InventoryManager.instance.DecreaseResources(gold, stone, wood);
                steal--;
                if (steal == 1)
                {
                    state = EnemyState.WALK;
                }
            }

            yield return null;
        }
    }

    public override void EnemyStartBehaviour()
    {
        base.EnemyStartBehaviour();
        animator = GetComponent<Animator>();

        id = "ResourceStealer" + Random.Range(0, int.MaxValue).ToString();

        if (string.IsNullOrEmpty(enemyData.enemyId))
        {
            enemyData.enemyId = id;
            enemyData.enemyType = EnemyType.RESOURCESTEALER;
            GameController.instance.current.enemies.Add(enemyData);
        }

        StartCoroutine(Dig());
        StartCoroutine(StealResources());
    }

    public override void EnemyUpdateBehaviour()
    {
        base.EnemyUpdateBehaviour();

        switch (state)
        {
            case EnemyState.WALK:
                
                this.gameObject.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                animator.SetInteger(animationStateParameterName, walkState);
                Walk(wayPoints[path]);
                
                break;

            case EnemyState.DIG:
                navMeshAgent.speed = speed;
                this.gameObject.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                
                break;

            default:
                Debug.Log(state + " does not support by code.");
                break;
        }

        enemyData.health = healthDisplay.CurrentHealthValue;
        enemyData.enemyPosition = transform.position;
        enemyData.enemyRotation = transform.rotation;
    }

    public void DigDown()
    {
        
        //this.gameObject.SetActive(false);
    }
}
