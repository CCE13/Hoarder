using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public List<GameObject> arrowPool;
    public List<GameObject> enemyProjectilePool;
    public List<GameObject> enemyMeleePool;
    public List<GameObject> enemyRangedPool;
    public List<GameObject> popUpPool;
    public GameObject popUp;
    public GameObject arrow;
    public GameObject enemyProj;
    public GameObject meleeEnemy;
    public GameObject rangedEnemy;
    public int amountToPool;
    public int amountOfArrows;
    // Start is called before the first frame update
    void Start()
    {
        // instantiates the amound to pool into a new list
        enemyMeleePool = new List<GameObject>();
        enemyRangedPool = new List<GameObject>();
        popUpPool = new List<GameObject>();
        arrowPool = new List<GameObject>();

        for (int i = 0; i < amountToPool; i++)
        {
            if (!meleeEnemy) break;
            GameObject obvToInstantiate = Instantiate(meleeEnemy);
            obvToInstantiate.SetActive(false);
            enemyMeleePool.Add(obvToInstantiate);
        }

        for (int i = 0; i < amountToPool; i++)
        {
            if (!rangedEnemy) break;
            GameObject obvToInstantiate = Instantiate(rangedEnemy);
            obvToInstantiate.SetActive(false);
            enemyRangedPool.Add(obvToInstantiate);
        }

        for (int i = 0; i < amountOfArrows; i++)
        {
            if (!arrow) break;
            GameObject arrowToInstantiate= Instantiate(arrow);
            arrowToInstantiate.SetActive(false);
            arrowPool.Add(arrowToInstantiate);
        }

        for (int i = 0; i < amountOfArrows; i++)
        {
            if (!popUp) break;
            GameObject objToInstantiate = Instantiate(popUp);
            objToInstantiate.SetActive(false);
            popUpPool.Add(objToInstantiate);
        }

        for (int i = 0; i < amountOfArrows; i++)
        {
            if (!enemyProj) break;
            GameObject objToInstantiate = Instantiate(enemyProj);
            objToInstantiate.SetActive(false);
            enemyProjectilePool.Add(objToInstantiate);
        }
    }
    public void Awake()
    {
        SharedInstance = this;
    }

    public GameObject GetArrow()
    {
        for (int i = 0; i < arrowPool.Count; i++)
        {
            if (!arrowPool[i].activeInHierarchy)
            {
                return arrowPool[i];
            }
        }

        GameObject newArrow = Instantiate(arrow);
        newArrow.SetActive(false);
        arrowPool.Add(newArrow);

        return newArrow;
    }

    public GameObject GetEnemyProj()
    {
        for (int i = 0; i < enemyProjectilePool.Count; i++)
        {
            if (!enemyProjectilePool[i].activeInHierarchy)
            {
                return enemyProjectilePool[i];
            }
        }

        GameObject newEnemyProj = Instantiate(enemyProj);
        newEnemyProj.SetActive(false);
        enemyProjectilePool.Add(newEnemyProj);

        return newEnemyProj;
    }

    public GameObject GetPopUp()
    {
        for (int i = 0; i < popUpPool.Count; i++)
        {
            if (!popUpPool[i].activeInHierarchy)
            {
                return popUpPool[i];
            }
        }

        GameObject newPopUp = Instantiate(popUp);

        newPopUp.SetActive(false);
        popUpPool.Add(newPopUp);

        return newPopUp;
    }

    public GameObject GetEnemy(bool typeOfEnemy, int health)
    {

        // False for melee, true for ranged
        // This will automatically implement the required health.

        if(!typeOfEnemy)
        {
            for (int i = 0; i < enemyMeleePool.Count; i++)
            {
                if (!enemyMeleePool[i].activeInHierarchy)
                {
                    var eHealth = enemyMeleePool[i].GetComponent<Health>();

                    eHealth.maxHealth = health;
                    eHealth.ResetHealth();

                    return enemyMeleePool[i];
                }
            }

            GameObject newMeleeEnemy = Instantiate(meleeEnemy);
            var mHealth = newMeleeEnemy.GetComponent<Health>();

            mHealth.maxHealth = health;
            mHealth.ResetHealth();

            newMeleeEnemy.SetActive(false);
            enemyMeleePool.Add(newMeleeEnemy);

            return newMeleeEnemy;
        }

        for (int i = 0; i < enemyRangedPool.Count; i++)
        {
            if (!enemyRangedPool[i].activeInHierarchy)
            {
                var eHealth = enemyRangedPool[i].GetComponent<Health>();

                eHealth.maxHealth = health;
                eHealth.ResetHealth();

                return enemyRangedPool[i];
            }
        }

        GameObject newRangedEnemy = Instantiate(meleeEnemy);
        var rHealth = newRangedEnemy.GetComponent<Health>();

        rHealth.maxHealth = health;
        rHealth.ResetHealth();

        newRangedEnemy.SetActive(false);
        enemyRangedPool.Add(newRangedEnemy);

        return newRangedEnemy;

    }
}