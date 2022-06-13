/*  Filename:           Tower.cs
 *  Author:             Han Bi (301176547)
 *  Last Update:        June 8, 2022
 *  Description:        Base abstract class for all towers.
 *  Revision History:   June 8, 2022 (Han Bi): Initial script.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    public enum TowerType
    {
        CrossbowTower,
        BombTower,
        ResourceTower
    }

    [SerializeField]
    [Tooltip("The time tower will wait before firing again")]
    protected float actionDelay;

    protected abstract void TowerBehaviour();

    private void Update()
    {
        TowerBehaviour();
    }

    public virtual void AddToTargets(GameObject gameObject) { }

    public virtual void RemoveFromTargets(GameObject gameObject) { }


    public abstract int GetTowerType();

 


}


