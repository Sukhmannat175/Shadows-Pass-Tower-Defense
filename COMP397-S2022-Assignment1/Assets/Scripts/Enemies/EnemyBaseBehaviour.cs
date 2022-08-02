/*  Filename:           EnemyBaseBehaviour.cs
 *  Author:             Yuk Yee Wong (301234795)
 *                      Sukhmannat Singh (301168420)
 *  Last Update:        June 26, 2022
 *  Description:        Abstract Enemy Base Behaviour Class for all enemies.
 *  Revision History:   June 18, 2022 (Yuk Yee Wong): Initial script extracted from GruntGolemController with modifications on health, projectile, enemy damage etc.
 *                      June 24, 2022 (Sukhmannat Singh): Added logic for deleting destroyed objects from save file 
 *                      June 26, 2022 (Yuk Yee Wong): Adding initialize function using enemy static data and fix bug by adding a death boolean for checking
 *                      Auguest 1, 2022 (Yuk Yee Wong): Reorganised the code and adapted object pooling.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider), typeof(NavMeshAgent))]
public abstract class EnemyBaseBehaviour : Enemy
{
    [Header("Initialize by Static Data")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int goldPerHead;
    [SerializeField] private int enemyDamage = 1;
    [SerializeField] protected int playerDamage = 1;
    [SerializeField] private int scorePerEnemyKilled = 10;
    [Tooltip("Come from nav mesh agent, for calculating distance travelled")]
    [SerializeField] protected float speed;

    [Header("Data that will reset on spawn")]
    [SerializeField] protected int path = 0;
    [SerializeField] protected float distanceTravelled;
    [SerializeField] protected bool death;

    [Header("Debug")]
    [Tooltip("Assigned from game controller")]
    [SerializeField] protected List<Transform> wayPoints;
    [Tooltip("Updated by itself")]
    [SerializeField] protected EnemyState state;

    private string playerProjectileTag = "Projectile";
    private EnemyData removeEnemy;
    protected NavMeshAgent navMeshAgent;

    protected void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void TakeEnemyDamage(int damage)
    {
        healthDisplay.TakeDamage(damage);

        if (healthDisplay.CurrentHealthValue == 0 && !death)
        {
            death = true;
            GameController.instance.KillEnemey(scorePerEnemyKilled);
            InventoryManager.instance.CollectResources(goldPerHead, 0, 0);
            SoundManager.instance.PlayEnemyDeathSfx();
            
            foreach (EnemyData enemyData in GameController.instance.current.enemies)
            {
                if (enemyData.enemyId == this.id)
                {
                    removeEnemy = enemyData;
                }
            }
            GameController.instance.current.enemies.Remove(removeEnemy);

            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerProjectileTag))
        {
            TakeEnemyDamage(enemyDamage);
        }
    }

    protected void UpdateEnemyHpBarRotation()
    {
        healthDisplay.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    }

    protected void UpdateDistanceTravelled()
    {
        distanceTravelled += speed * Time.deltaTime;
    }

    public override void SetWayPoints(Transform wayPointsContainer)
    {
        wayPoints.Clear();

        foreach (Transform wayPoint in wayPointsContainer.transform)
            wayPoints.Add(wayPoint);
    }

    public override float GetDistanceTravelled()
    {
        return distanceTravelled;
    }

    public override void EnemyUpdateBehaviour()
    {
        UpdateEnemyHpBarRotation();
        UpdateDistanceTravelled();
    }

    protected override void RefreshEnemyData()
    {
        id = idPrefix + Random.Range(0, int.MaxValue).ToString();
        enemyData.enemyId = id;
        enemyData.enemyType = enemyType;
        GameController.instance.current.enemies.Add(enemyData);
    }

    public override void Intialize(EnemyStaticData data)
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        maxHealth = data.hp;
        navMeshAgent.speed = data.speed;
        navMeshAgent.stoppingDistance = data.stoppingDistance;
        goldPerHead = data.goldPerHead;
        scorePerEnemyKilled = data.scorePerHead;
        playerDamage = data.ap;

        healthDisplay.Init(maxHealth);
        SetSpeed(navMeshAgent.speed);

        ResetEnemy();

        RefreshEnemyData();
    }

    protected void ResetEnemy()
    {
        path = 0;
        distanceTravelled = 0;
        death = false;
    }

    public override void Walk(Transform position)
    {
        navMeshAgent.destination = position.position;
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    path += 1;
                    if (path == wayPoints.Count)
                    {
                        foreach (EnemyData enemyData in GameController.instance.current.enemies)
                        {
                            if (enemyData.enemyId == this.id)
                            {
                                removeEnemy = enemyData;
                            }
                        }
                        GameController.instance.current.enemies.Remove(removeEnemy);
                        PlayerHealthBarController.instance.TakeDamage(playerDamage);
                        GameController.instance.AddTotalEnemiesDead();
                        SoundManager.instance.PlayPlayerDamageSfx();
                        ReturnToPool();
                    }
                }
            }
        }
    }
}
